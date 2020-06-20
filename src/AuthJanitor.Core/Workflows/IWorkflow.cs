using AuthJanitor.Providers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AuthJanitor.Workflows
{
    public interface IWorkflow
    {
        //TODO: ExecuteWorkflow better naming 
        Task ExecuteRekeyingWorkflow(WorkflowAttemptLogger logger, TimeSpan validPeriod, IEnumerable<IAuthJanitorProvider> providers);

    }
}
