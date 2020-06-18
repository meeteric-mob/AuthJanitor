// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
using AuthJanitor.IdentityServices;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace AuthJanitor.Integrations.IdentityServices.AzureActiveDirectory
{
    public static class Extensions
    {
        public static void AddAJAzureActiveDirectory(this IServiceCollection serviceCollection, Action<AzureADIdentityServiceConfiguration> configureOptions)
        {
            serviceCollection.Configure(configureOptions);

            serviceCollection.AddSingleton(p =>
            {
                IIdentityService service = p.GetService<AzureADIdentityService>(); ;

#if DEBUG
                    service = new LocalDebugIdentityServiceDecorator(service);
#endif
                return service;
            });
        }
    }
}
