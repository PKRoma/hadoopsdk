namespace Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;

    internal abstract class AzureHDInsightClusterCommand<T> : AzureHDInsightClusterCommandBase, IAzureHDInsightCommand<T>
    {
        public ICollection<T> Output { get; private set; }

        public AzureHDInsightClusterCommand()
        {
            this.Output = new Collection<T>();
        }
    }
}
