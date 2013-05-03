namespace Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.Old
{
    using System.Runtime.Serialization;

    [DataContract(Name = "OperationStatus", Namespace = "http://schemas.microsoft.com/windowsazure")]
    internal class ResourceOperationStatus : IExtensibleDataObject
    {
        // Methods

        // Properties
        [DataMember(Order = 3, EmitDefaultValue = false)]
        public ResourceErrorInfo Error { get; set; }

        [DataMember(Order = 2, EmitDefaultValue = false)]
        public string Result { get; set; }

        [DataMember(Order = 1, EmitDefaultValue = false)]
        public string Type { get; set; }

        public ExtensionDataObject ExtensionData { get; set; }
    }
}