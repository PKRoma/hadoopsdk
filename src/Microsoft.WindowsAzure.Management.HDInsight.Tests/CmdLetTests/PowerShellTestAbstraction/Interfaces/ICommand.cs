namespace Microsoft.WindowsAzure.Management.HDInsight.Tests.CmdLetTests.PowerShellTestAbstraction.Interfaces
{
    internal interface ICommand : IPipeline
    {
        ICommand WithParameter(string name, object value);
    }
}
