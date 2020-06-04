// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
using AuthJanitor.Automation.Shared;
using AuthJanitor.Automation.Shared.MetaServices;
using AuthJanitor.Automation.Shared.Models;
using AuthJanitor.Automation.Shared.ViewModels;
using AuthJanitor.Integrations;
using AuthJanitor.Integrations.DataStores;
using AuthJanitor.Providers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Management.TrafficManager.Fluent.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace AuthJanitor.Automation.Agent
{
    public class ExternalSignalFunction
    {
        private readonly ExternalSignal _externalFunction;

        public ExternalSignalFunction(
            IOptions<AuthJanitorCoreConfiguration> configuration,
            TaskExecutionMetaService taskExecutionMetaService,
            IDataStore<ManagedSecret> managedSecretStore,
            IDataStore<RekeyingTask> rekeyingTaskStore)
        {
            _externalFunction = new ExternalSignal(configuration, taskExecutionMetaService, managedSecretStore, rekeyingTaskStore);
        }

        [FunctionName("ExternalSignal")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "secrets/{managedSecretId:guid}/{nonce}")] HttpRequest req,
            Guid managedSecretId,
            string nonce,
            ILogger log)
        {
            return await _externalFunction.Run(req, managedSecretId, nonce, log);
        }
    }
}
