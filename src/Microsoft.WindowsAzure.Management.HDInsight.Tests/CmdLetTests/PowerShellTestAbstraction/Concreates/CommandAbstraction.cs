namespace Microsoft.WindowsAzure.Management.HDInsight.Tests.CmdLetTests.PowerShellTestAbstraction.Concreates
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Automation.Runspaces;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.WindowsAzure.Management.HDInsight.Tests.CmdLetTests.PowerShellTestAbstraction.Interfaces;

    internal class CommandAbstraction : PipelineAbstraction, ICommand
    {
        protected Command Command { get; private set; }

        internal CommandAbstraction(Command command, Pipeline pipeline) : base(pipeline, pipeline.Runspace)
        {
            this.Command = command;
        }

        public ICommand WithParameter(string name, object value)
        {
            this.Command.Parameters.Add(name, value);
            return this;
        }
    }
}
