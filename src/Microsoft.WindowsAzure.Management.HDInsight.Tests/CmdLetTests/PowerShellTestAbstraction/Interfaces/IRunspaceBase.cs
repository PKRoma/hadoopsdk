namespace Microsoft.WindowsAzure.Management.HDInsight.Tests.CmdLetTests.PowerShellTestAbstraction.Interfaces
{
    internal interface IRunspaceBase
    {
        IPipeline NewPipeline();
    }
}
