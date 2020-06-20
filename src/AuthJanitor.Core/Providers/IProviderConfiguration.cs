using System;
using System.Collections.Generic;
using System.Text;

namespace AuthJanitor.Providers
{
    public interface IProviderConfiguration
    {
        //Todo: IsProviderConfigurationValid seems to be a better name
        bool TestProviderConfiguration(LoadedProviderMetadata metadata, string serializedConfiguration);
    }
}
