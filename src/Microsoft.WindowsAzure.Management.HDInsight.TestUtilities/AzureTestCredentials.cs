namespace Microsoft.WindowsAzure.Management.HDInsight.TestUtilities
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    public class StorageAccountCredentials
    {
        public string Name { get; set; }
        public string Key { get; set; }
        public string Container { get; set; }
    }

    public class AlternativeEnvironment
    {
        public string Endpoint { get; set; }
        public string Namespace { get; set; }
        public Guid SubscriptionId { get; set; }
    }

    public class MetastoreCredentials
    {
        public string Description { get; set; }
        public string SqlServer { get; set; }
        public string Database { get; set; }
    }

    public class KnownCluster
    {
        public string Cluster { get; set; }
        public string DnsName { get; set; }
    }

    public enum EnvironmentType
    {
        Production,
        Current,
        Next,
        DogFood
    }

    public class CreationDetails
    {
        public string Location { get; set; }

        public StorageAccountCredentials DefaultStorageAccount { get; set; }

        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays",
            Justification = "Needed for serialization to work correctly. [tgs]")]
        public StorageAccountCredentials[] AdditionalStorageAccounts { get; set; }
    
        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays",
            Justification = "Needed for serialization to work correctly. [tgs]")]
        public MetastoreCredentials[] HiveStores { get; set; }

        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays",
            Justification = "Needed for serialization to work correctly. [tgs]")]
        public MetastoreCredentials[] OozieStores { get; set; }
    }

    [Serializable]
    public class AzureTestCredentials
    {
        public string CredentialsName { get; set; }
        public Guid SubscriptionId { get; set; }
        public string Certificate { get; set; }
        public string InvalidCertificate { get; set; }
        public string AzureUserName { get; set; }
        public string AzurePassword { get; set; }
        public string HadoopUserName { get; set; }
        public string Endpoint { get; set; }
        public string CloudServiceName { get; set; }

        public KnownCluster WellKnownCluster { get; set; }
        public EnvironmentType Type { get; set; }

        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays",
            Justification = "Needed for serialization to work correctly. [tgs]")]
        public CreationDetails[] Environments { get; set; }
    }
}
