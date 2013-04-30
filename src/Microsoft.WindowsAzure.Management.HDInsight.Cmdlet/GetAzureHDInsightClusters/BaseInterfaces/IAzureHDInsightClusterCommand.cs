namespace Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters.BaseInterfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    internal interface IAzureHDInsightClusterCommand<T> : IAzureHDInsightClusterCommandBase, IAzureHDInsightCommand<T>
    {
    }
}
