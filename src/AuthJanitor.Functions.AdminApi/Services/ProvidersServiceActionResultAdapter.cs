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
    public class ProvidersServiceActionResultAdapter
    {
        private readonly IIdentityService _identityService;
        private readonly ProvidersService _providers;

        public ProvidersServiceActionResultAdapter(
            IIdentityService identityService,
            ProvidersService providers)
        {
            _identityService = identityService;
            _providers = providers;
        }

        public async Task<IActionResult> List()
        {
            return await ValidateLogin(() => Task.FromResult(_providers.List()));
        }

        public async Task<IActionResult> GetBlankConfiguration(ProviderIdentifier providerId)
        {
            return await ValidateLogin(async () => await _providers.GetBlankConfiguration(providerId));
        }

        public async Task<IActionResult> TestConfiguration(
            string providerConfiguration,
            ProviderIdentifier providerId,
            string testContext)
        {
            return await ValidateLogin(async () => await _providers.TestConfiguration(providerConfiguration, providerId, testContext));
        }

        private async Task<IActionResult> ValidateLogin(Func<Task<IActionResult>> func)
        {
            IActionResult result;

            if (_identityService.IsUserLoggedIn)
            {
                result = await func();
            }
            else
            {
                result = new UnauthorizedResult();
            }

            return result;
        }
    }
}
