namespace Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.Old
{
    using System.IO;
    using System.Runtime.Serialization;
    using System.Xml;

    [DataContract(Namespace = "http://schemas.microsoft.com/windowsazure")]
    internal class Resource : IExtensibleDataObject
    {
        // Methods

        // Properties
        [DataMember(Order = 7, EmitDefaultValue = false)]
        public string ETag { get; set; }

        [DataMember(Order = 11, EmitDefaultValue = false)]
        public XmlNode[] IntrinsicSettings { get; set; }

        [DataMember(Order = 3, EmitDefaultValue = false)]
        public string Name { get; set; }

        [DataMember(Order = 13, EmitDefaultValue = false)]
        public ResourceOperationStatus OperationStatus { get; set; }

        [DataMember(Order = 12, EmitDefaultValue = false)]
        public OutputItemList OutputItems { get; set; }

        [DataMember(Order = 4, EmitDefaultValue = false)]
        public string Plan { get; set; }

        [DataMember(Order = 5, EmitDefaultValue = false)]
        public string PromotionCode { get; set; }

        [DataMember(Order = 1, EmitDefaultValue = false)]
        public string ResourceProviderNamespace { get; set; }

        [DataMember(Order = 6, EmitDefaultValue = false)]
        public string SchemaVersion { get; set; }

        [DataMember(Order = 8, EmitDefaultValue = false)]
        public string State { get; set; }

        [DataMember(Order = 9, EmitDefaultValue = false)]
        public string SubState { get; set; }

        [DataMember(Order = 2, EmitDefaultValue = false)]
        public string Type { get; set; }

        [DataMember(Order = 10, EmitDefaultValue = false)]
        public UsageMeterCollection UsageMeters { get; set; }

        public ExtensionDataObject ExtensionData { get; set; }
        
        internal string SerializeToXml()
        {
            var ser = new DataContractSerializer(this.GetType());
            using (var ms = new MemoryStream())
            {
                ser.WriteObject(ms, this);
                ms.Seek(0, SeekOrigin.Begin);
                return new StreamReader(ms).ReadToEnd();
            }
        }
    }
}