using AuthJanitor.Providers;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthJanitor.Workflows
{
    public class RekeyWorkflow : IWorkflow
    {
        public async Task ExecuteWorkflow(WorkflowAttemptLogger logger, TimeSpan validPeriod, IEnumerable<IAuthJanitorProvider> providers)
        {
            logger.LogInformation("########## BEGIN REKEYING WORKFLOW ##########");
            var rkoProviders = providers.OfType<IRekeyableObjectProvider>().ToList();
            var alcProviders = providers.OfType<IApplicationLifecycleProvider>().ToList();

            // NOTE: avoid costs of generating list of providers if information logging not turned on
            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("RKO: {ProviderTypeNames}", string.Join(", ", rkoProviders.Select(p => p.GetType().Name)));
                logger.LogInformation("ALC: {ProviderTypeNames}", string.Join(", ", alcProviders.Select(p => p.GetType().Name)));
            }

            // -----

            logger.LogInformation("### Performing Provider Tests.");

            await PerformProviderActions(
                logger,
                providers,
                p => p.Test(),
                "Error running sanity test on provider '{ProviderName}'",
                "Error running one or more sanity tests!");

            logger.LogInformation("### Retrieving/generating temporary secrets.");

            var temporarySecrets = new List<RegeneratedSecret>();
            await PerformProviderActions(
                logger,
                rkoProviders,
                p => p.GetSecretToUseDuringRekeying()
                        .ContinueWith(t =>
                        {
                            if (t.Result != null)
                            {
                                temporarySecrets.Add(t.Result);
                            }
                        }),
                "Error getting temporary secret from provider '{ProviderName}'",
                "Error retrieving temporary secrets from one or more Rekeyable Object Providers!");

            logger.LogInformation("{SecretCount} temporary secrets were created/read to be used during operation.", temporarySecrets.Count);

            // ---

            logger.LogInformation("### Preparing {ProviderCount} Application Lifecycle Providers for rekeying...", alcProviders.Count);
            await PerformProviderActions(
                logger,
                alcProviders,
                p => p.BeforeRekeying(temporarySecrets),
                "Error preparing ALC provider '{ProviderName}'",
                "Error preparing one or more Application Lifecycle Providers for rekeying!");

            // -----

            logger.LogInformation("### Rekeying {ProviderCount} Rekeyable Object Providers...", rkoProviders.Count);
            var newSecrets = new List<RegeneratedSecret>();
            await PerformProviderActions(
                logger,
                rkoProviders,
                p => p.Rekey(validPeriod)
                        .ContinueWith(t =>
                        {
                            if (t.Result != null)
                            {
                                newSecrets.Add(t.Result);
                            }
                        }),
                "Error rekeying provider '{ProviderName}'",
                "Error rekeying one or more Rekeyable Object Providers!");

            logger.LogInformation("{SecretCount} secrets were regenerated.", newSecrets.Count);

            // -----

            logger.LogInformation("### Committing {SecretCount} regenerated secrets to {ProviderCount} Application Lifecycle Providers...",
                newSecrets.Count,
                alcProviders.Count);

            await PerformProviderActions(
                logger,
                alcProviders,
                p => p.CommitNewSecrets(newSecrets),
                "Error committing to provider '{ProviderName}'",
                "Error committing regenerated secrets!");

            // -----

            logger.LogInformation("### Completing post-rekey operations on Application Lifecycle Providers...");

            await PerformProviderActions(
                logger,
                alcProviders,
                p => p.AfterRekeying(),
                "Error running post-rekey operations on provider '{ProviderName}'",
                "Error running post-rekey operations on one or more Application Lifecycle Providers!");

            // -----

            logger.LogInformation("### Completing finalizing operations on Rekeyable Object Providers...");

            await PerformProviderActions(
                logger,
                rkoProviders,
                p => p.OnConsumingApplicationSwapped(),
                "Error running after-swap operations on provider '{ProviderName}'",
                "Error running after-swap operations on one or more Rekeyable Object Providers!");

            logger.LogInformation("########## END REKEYING WORKFLOW ##########");
        }

        private static async Task PerformProviderActions<TProviderType>(
            ILogger logger,
            IEnumerable<TProviderType> providers,
            Func<TProviderType, Task> providerAction,
            string individualFailureErrorLogMessageTemplate,
            string anyFailureExceptionMessage)
            where TProviderType : IAuthJanitorProvider
        {
            var providerActions = providers.Select(async p =>
            {
                try
                {
                    await providerAction(p);
                }
                catch (Exception exception)
                {
                    logger.LogError(exception, individualFailureErrorLogMessageTemplate, p.GetType().Name);

                    throw;
                }
            });

            try
            {
                await Task.WhenAll(providerActions);
            }
            catch (Exception exception)
            {
                throw new Exception(anyFailureExceptionMessage, exception);
            }
        }
    }
}
