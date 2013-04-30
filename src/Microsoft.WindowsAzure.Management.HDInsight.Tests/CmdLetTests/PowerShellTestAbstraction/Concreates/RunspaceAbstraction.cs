namespace Microsoft.WindowsAzure.Management.HDInsight.Tests.CmdLetTests.PowerShellTestAbstraction.Concreates
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Automation.Runspaces;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.WindowsAzure.Management.Framework;
    using Microsoft.WindowsAzure.Management.HDInsight.Tests.CmdLetTests.PowerShellTestAbstraction.Interfaces;

    class RunspaceAbstraction : DisposableObject, IRunspace
    {
        protected Runspace Runspace { get; private set; }

        public RunspaceAbstraction(Runspace runspace)
        {
            this.Runspace = runspace;
        }

        public static IRunspace Create()
        {
            var runspace = Help.SaveCreate(() => RunspaceFactory.CreateRunspace());
            runspace.Open();
            return new RunspaceAbstraction(runspace);
        }

        public IPipeline NewPipeline()
        {
            return Help.SaveCreate(() => new PipelineAbstraction(this.Runspace.CreatePipeline(), this.Runspace));
        }
    }
}
