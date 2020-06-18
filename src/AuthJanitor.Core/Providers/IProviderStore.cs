using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AuthJanitor.Providers
{
    public interface IProviderStore
    {
        IReadOnlyList<LoadedProviderMetadata> LoadedProviders { get; }

        IAuthJanitorProvider GetProviderInstance(string providerName, string serializedProviderConfiguration);
        IAuthJanitorProvider GetProviderInstance(string providerName);
        LoadedProviderMetadata GetProviderMetadata(string providerName);
        AuthJanitorProviderConfiguration GetProviderConfiguration(string name);

        // Should be moved to antoher type
        Task ExecuteRekeyingWorkflow(RekeyingAttemptLogger logger, TimeSpan validPeriod, IEnumerable<IAuthJanitorProvider> providers);
        bool TestProviderConfiguration(string name, string serializedConfiguration);
    }
}
