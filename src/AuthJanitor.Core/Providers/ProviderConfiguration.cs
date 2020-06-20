using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace AuthJanitor.Providers
{
    public class ProviderConfiguration : IProviderConfiguration
    {
        public bool TestProviderConfiguration(LoadedProviderMetadata metadata, string serializedConfiguration)
        {
            try
            {
                var obj = JsonSerializer.Deserialize(serializedConfiguration, metadata.ProviderConfigurationType, ProviderManagerService.SerializerOptions);
                return obj != null;
            }
            catch { return false; }
        }
    }
}
