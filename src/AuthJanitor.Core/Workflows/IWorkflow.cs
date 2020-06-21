using AuthJanitor.Providers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AuthJanitor.Workflows
{
    public interface IWorkflow
    {
        Task ExecuteWorkflow(WorkflowAttemptLogger logger, TimeSpan validPeriod, IEnumerable<IAuthJanitorProvider> providers);

    }
}
