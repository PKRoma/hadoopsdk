namespace Microsoft.WindowsAzure.Management.HDInsight.Tests.CmdLetTests.PowerShellTestAbstraction.Interfaces
{
    using System.Collections.Generic;
    using System.Management.Automation;

    interface IPipelineResult : IRunspaceBase
    {
        ICollection<PSObject> Results { get; }
    }
}
