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

    internal class PipelineAbstraction : RunspaceAbstraction, IPipeline
    {
        protected Pipeline Pipeline { get; private set; }

        internal PipelineAbstraction(Pipeline pipeline, Runspace runspace) : base(runspace)
        {
            this.Pipeline = pipeline;
        }

        public ICommand AddCommand(string commandName)
        {
            Command command = new Command(commandName);
            this.Pipeline.Commands.Add(command);
            return Help.SaveCreate(() => new CommandAbstraction(command, this.Pipeline));
        }

        public IPipelineResult Invoke()
        {
            var results = this.Pipeline.Invoke();
            return Help.SaveCreate(() => new PipelineResultsAbstraction(results, this.Runspace));
        }
    }
}
