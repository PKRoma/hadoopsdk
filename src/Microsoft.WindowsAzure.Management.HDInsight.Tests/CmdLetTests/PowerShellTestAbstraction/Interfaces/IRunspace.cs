namespace Microsoft.WindowsAzure.Management.HDInsight.Tests.CmdLetTests.PowerShellTestAbstraction.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.WindowsAzure.Management.Framework;

    internal interface IRunspace : IRunspaceBase, IQueryDisposable
    {
    }
}
