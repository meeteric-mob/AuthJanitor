using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AuthJanitor.Providers
{
    public interface IProviderStore
    {
        IReadOnlyList<LoadedProviderMetadata> LoadedProviders { get; }

        IAuthJanitorProvider GetProviderInstance(ProviderIdentifier providerId, string serializedProviderConfiguration);
        IAuthJanitorProvider GetProviderInstance(ProviderIdentifier providerId);
        LoadedProviderMetadata GetProviderMetadata(ProviderIdentifier providerId);
        AuthJanitorProviderConfiguration GetProviderConfiguration(ProviderIdentifier providerId);
    }
}
