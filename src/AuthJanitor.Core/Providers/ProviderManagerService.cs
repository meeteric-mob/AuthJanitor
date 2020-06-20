// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
using AuthJanitor.Workflows;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AuthJanitor.Providers
{

    public class ProviderManagerService : IProviderStore
    {
        public static readonly JsonSerializerOptions SerializerOptions = new JsonSerializerOptions()
        {
            WriteIndented = false,
            IgnoreNullValues = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new JsonStringEnumConverter() }
        };

        private readonly IServiceProvider _serviceProvider;
        private readonly IWorkflow _workflow;
        private readonly IProviderConfiguration _providerConfiguration;

        public ProviderManagerService(
            IServiceProvider serviceProvider,
            IReadOnlyList<LoadedProviderMetadata> loadedProviders,
            IProviderConfiguration providerConfiguration,
            IWorkflow workflow)
        {
            _providerConfiguration = providerConfiguration;
            _workflow = workflow;
            _serviceProvider = serviceProvider;
            LoadedProviders = loadedProviders; 
        }

        public bool HasProvider(ProviderIdentifier providerId) => LoadedProviders.Any(p => p.Id == providerId);

        public IReadOnlyList<LoadedProviderMetadata> LoadedProviders { get; }

        public LoadedProviderMetadata GetProviderMetadata(ProviderIdentifier providerId)
        {
            if (!HasProvider(providerId))
                throw new Exception($"Provider '{providerId}' not available!");
            else
                return LoadedProviders.First(p => p.Id == providerId);
        }

        public IAuthJanitorProvider GetProviderInstance(ProviderIdentifier providerId)
        {
            var metadata = GetProviderMetadata(providerId);
            return ActivatorUtilities.CreateInstance(_serviceProvider, metadata.ProviderType) as IAuthJanitorProvider;
        }

        public IAuthJanitorProvider GetProviderInstance(ProviderIdentifier providerId, string serializedProviderConfiguration)
        {
            var instance = GetProviderInstance(providerId);
            instance.SerializedConfiguration = serializedProviderConfiguration;
            return instance;
        }

        public AuthJanitorProviderConfiguration GetProviderConfiguration(ProviderIdentifier providerId) => ActivatorUtilities.CreateInstance(_serviceProvider, GetProviderMetadata(providerId).ProviderConfigurationType) as AuthJanitorProviderConfiguration;
        public AuthJanitorProviderConfiguration GetProviderConfiguration(ProviderIdentifier providerId, string serializedConfiguration) => JsonSerializer.Deserialize(serializedConfiguration, GetProviderMetadata(providerId).ProviderConfigurationType, SerializerOptions) as AuthJanitorProviderConfiguration;

        public async Task ExecuteRekeyingWorkflow(WorkflowAttemptLogger logger, TimeSpan validPeriod, IEnumerable<IAuthJanitorProvider> providers)
        {
            await _workflow.ExecuteRekeyingWorkflow(logger, validPeriod, providers);
            //await _hmm.ExecuteRekeyingWorkflow(logger, validPeriod, providers);
        }

        public bool TestProviderConfiguration(ProviderIdentifier providerId, string serializedConfiguration)
        {
            var metadata = GetProviderMetadata(providerId);
            return _providerConfiguration.TestProviderConfiguration(metadata, serializedConfiguration);
            //return _hmm.TestProviderConfiguration(metadata, serializedConfiguration);
        }
       
    }

}

