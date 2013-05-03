namespace Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.Old
{
    using System.Runtime.Serialization;

    [DataContract(Namespace = "http://schemas.microsoft.com/windowsazure")]
    internal class CloudService : IExtensibleDataObject
    {
        // Properties
        [DataMember(Order = 3, EmitDefaultValue = false)]
        public string Description { get; set; }

        [DataMember(Order = 4, EmitDefaultValue = false)]
        public string GeoRegion { get; set; }

        [DataMember(Order = 2, EmitDefaultValue = false)]
        public string Label { get; set; }

        [DataMember(Order = 1, EmitDefaultValue = false)]
        public string Name { get; set; }

        [DataMember(Order = 5, EmitDefaultValue = false)]
        public ResourceList Resources { get; set; }

        public ExtensionDataObject ExtensionData { get; set; }
    }
}