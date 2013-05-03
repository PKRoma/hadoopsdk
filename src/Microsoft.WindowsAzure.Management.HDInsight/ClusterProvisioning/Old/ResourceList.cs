namespace Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.Old
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    [CollectionDataContract(Name = "Resources", ItemName = "Resource",
        Namespace = "http://schemas.microsoft.com/windowsazure")]
    internal class ResourceList : List<Resource>
    {
        // Methods
        public ResourceList()
        {
        }

        public ResourceList(IEnumerable<Resource> resources) : base(resources)
        {
        }
    }
}