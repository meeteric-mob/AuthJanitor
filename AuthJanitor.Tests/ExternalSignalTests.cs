using AuthJanitor.Automation.Agent;
using AuthJanitor.Automation.Shared.MetaServices;
using AuthJanitor.Automation.Shared.Models;
using AuthJanitor.Integrations;
using AuthJanitor.Integrations.DataStores;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AuthJanitor.Tests
{
    public class ExternalSignalTests
    {
        [Fact]
        public async Task ValidateExternalSignalsDoesSomething()
        {
            var config = new AuthJanitorCoreConfiguration();
            TaskExecutionMetaService taskMetaService = null;
            IDataStore<ManagedSecret> secretStore = null;
            IDataStore<RekeyingTask> rekeyTaskStore = null;
            var service = new ExternalSignal(Options.Create(config), taskMetaService, secretStore, rekeyTaskStore);
            await service.Run(null, Guid.Empty, null, null);
        }
    }
}
