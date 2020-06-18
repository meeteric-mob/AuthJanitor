using AuthJanitor.Providers;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace AuthJanitor
{
    public static class ConfigurationExtensions
    {
        public static void ConfigureProviderServices(this IServiceCollection serviceCollection, params Type[] loadedProviderTypes)
        {
            serviceCollection.AddSingleton<IProviderStore>((s) => new ProviderManagerService(s, loadedProviderTypes));
        }
    }
}
