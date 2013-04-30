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
        public string Username { get; set; }
        public string Password { get; set; }
    }

    [Serializable]
    public class AzureTestCredentials
    {
        public string CredentialsName { get; set; }
        public Guid SubscriptionId { get; set; }
        public string Certificate { get; set; }
        public string InvalidCertificate { get; set; }
        public string Cluster { get; set; }
        public string DnsName { get; set; }
        public string AzureUserName { get; set; }
        public string AzurePassword { get; set; }
        public string HadoopUserName { get; set; }
        public StorageAccountCredentials DefaultStorageAccount { get; set; }
        public AlternativeEnvironment AlternativeEnvironment { get; set; }

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
}
