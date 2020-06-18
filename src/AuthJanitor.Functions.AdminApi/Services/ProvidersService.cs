// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
using AuthJanitor.UI.Shared;
using AuthJanitor.UI.Shared.MetaServices;
using AuthJanitor.UI.Shared.ViewModels;
using AuthJanitor.EventSinks;
using AuthJanitor.IdentityServices;
using AuthJanitor.Providers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace AuthJanitor.Services
{
    /// <summary>
    /// API functions to describe the loaded Providers and their configurations.
    /// A Provider is a library containing logic to either rekey an object/service or manage the lifecycle of an application.
    /// </summary>
    public class ProvidersService
    {
        private readonly IIdentityService _identityService;
        private readonly EventDispatcherMetaService _eventDispatcher;
        private readonly IProviderStore _providerManager;

        private readonly Func<AuthJanitorProviderConfiguration, ProviderConfigurationViewModel> _configViewModel;
        private readonly Func<LoadedProviderMetadata, LoadedProviderViewModel> _providerViewModel;

        public ProvidersService(
            IIdentityService identityService,
            EventDispatcherMetaService eventDispatcher,
            IProviderStore providerManager,
            Func<AuthJanitorProviderConfiguration, ProviderConfigurationViewModel> configViewModelDelegate,
            Func<LoadedProviderMetadata, LoadedProviderViewModel> providerViewModelDelegate)
        {
            _identityService = identityService;
            _eventDispatcher = eventDispatcher;
            _providerManager = providerManager;

            _configViewModel = configViewModelDelegate;
            _providerViewModel = providerViewModelDelegate;
        }

        public IActionResult List()
        {
            return new OkObjectResult(_providerManager.LoadedProviders.Select(p => _providerViewModel(p)));
        }

        public async Task<IActionResult> GetBlankConfiguration(ProviderIdentifier providerId)
        {
            var provider = _providerManager.LoadedProviders.FirstOrDefault(p => p.Id == providerId);
            if (provider == null)
            {
                await _eventDispatcher.DispatchEvent(AuthJanitorSystemEvents.AnomalousEventOccurred, nameof(ProvidersService.GetBlankConfiguration), "Invalid Provider specified");
                return new NotFoundResult();
            }
            return new OkObjectResult(_configViewModel(_providerManager.GetProviderConfiguration(provider.Id)));
        }

        public async Task<IActionResult> TestConfiguration(
            string providerConfiguration,
            ProviderIdentifier providerId,
            string testContext)
        {
            Enum.TryParse<TestAsContexts>(testContext, true, out TestAsContexts testContextEnum);
            AccessTokenCredential credential = null;
            try
            {
                switch (testContextEnum)
                {
                    case TestAsContexts.AsApp:
                        credential = await _identityService.GetAccessTokenForApplicationAsync();
                        break;
                    case TestAsContexts.AsUser:
                        credential = await _identityService.GetAccessTokenOnBehalfOfCurrentUserAsync();
                        break;
                    default:
                        return new BadRequestErrorMessageResult("Invalid test context");
                }
                if (credential == null || string.IsNullOrEmpty(credential.AccessToken))
                    throw new Exception("Credential was empty!");
            }
            catch (Exception ex)
            {
                return new BadRequestErrorMessageResult(
                    "Error retrieving Access Token: " + Environment.NewLine +
                    ex.Message + Environment.NewLine +
                    ex.StackTrace);
            }

            var provider = _providerManager.LoadedProviders.FirstOrDefault(p => p.Id == providerId);
            if (provider == null)
            {
                await _eventDispatcher.DispatchEvent(AuthJanitorSystemEvents.AnomalousEventOccurred, nameof(ProvidersService.GetBlankConfiguration), "Invalid Provider specified");
                return new NotFoundResult();
            }

            try
            {
                var instance = _providerManager.GetProviderInstance(provider.Id, providerConfiguration);
                if (instance == null)
                    return new BadRequestErrorMessageResult("Provider configuration is invalid!");
                instance.Credential = credential;
                await instance.Test();
            }
            catch (Exception ex)
            {
                return new BadRequestErrorMessageResult(ex.Message);
            }

            return new OkResult();
        }
    }
}
