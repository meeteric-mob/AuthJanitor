using AuthJanitor.Providers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace AuthJanitor
{
    public static class ConfigurationExtensions
    {
        public static void ConfigureProviderServices(this IServiceCollection serviceCollection, params Type[] loadedProviderTypes)
        {
            var loadedProviders = GetLoadedProviderMetadataList(loadedProviderTypes);
            serviceCollection.AddSingleton<IProviderStore>((s) => new ProviderManagerService(s, loadedProviders));
        }

        private static IReadOnlyList<LoadedProviderMetadata> GetLoadedProviderMetadataList(params Type[] providerTypes)
        {
            return providerTypes
                .Where(type => !type.IsAbstract && typeof(IAuthJanitorProvider).IsAssignableFrom(type))
                .Select(type => new LoadedProviderMetadata()
                {
                    Id = ProviderIdentifier.FromString(type.AssemblyQualifiedName),
                    OriginatingFile = Path.GetFileName(type.Assembly.Location),
                    AssemblyName = type.Assembly.GetName(),
                    ProviderTypeName = type.AssemblyQualifiedName,
                    ProviderType = type,
                    ProviderConfigurationType = type.BaseType.GetGenericArguments()[0],
                    Details = type.GetCustomAttribute<ProviderAttribute>(),
                    SvgImage = type.GetCustomAttribute<ProviderImageAttribute>()?.SvgImage
                })
                .ToList()
                .AsReadOnly();
        }
    }
}
