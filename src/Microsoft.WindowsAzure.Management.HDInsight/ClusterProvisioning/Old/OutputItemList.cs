namespace Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.Old
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    [CollectionDataContract(Name = "OutputItems", ItemName = "OutputItem",
        Namespace = "http://schemas.microsoft.com/windowsazure")]
    internal class OutputItemList : List<OutputItem>
    {
        // Methods
        public OutputItemList()
        {
        }

        public OutputItemList(IEnumerable<OutputItem> outputs)
            : base(outputs)
        {
        }
    }
}