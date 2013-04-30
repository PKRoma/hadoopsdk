namespace Microsoft.WindowsAzure.Management.HDInsight.Tests.CmdLetTests.PowerShellTestAbstraction.Concreates
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Automation;
    using System.Management.Automation.Runspaces;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.WindowsAzure.Management.HDInsight.Tests.CmdLetTests.PowerShellTestAbstraction.Interfaces;

    internal class PipelineResultsAbstraction : RunspaceAbstraction, IPipelineResult
    {
        public PipelineResultsAbstraction(ICollection<PSObject> results, Runspace runspace) : base(runspace)
        {
            this.Results = results;
        }

        public ICollection<PSObject> Results { get; private set; }
    }
}
