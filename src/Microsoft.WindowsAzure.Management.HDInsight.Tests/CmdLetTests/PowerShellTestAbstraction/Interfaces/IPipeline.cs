namespace Microsoft.WindowsAzure.Management.HDInsight.Tests.CmdLetTests.PowerShellTestAbstraction.Interfaces
{
    internal interface IPipeline : IRunspaceBase
    {
        ICommand AddCommand(string commandName);
        IPipelineResult Invoke();
    }
}
