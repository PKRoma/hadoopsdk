namespace Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.Data
{
    /// <summary>
    /// Azure Storage Account Configuration for addition ASV HDInsight drives.
    /// </summary>
    public class StorageAccountConfiguration
    {
        /// <summary>
        /// Gets the Name of the Storage Account.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the Key for the Storage Account.
        /// </summary>
        public string Key { get; private set; }

        /// <summary>
        /// Initializes a new instance of the StorageAccountConfiguration class.
        /// </summary>
        /// <param name="accountName">Account name of the Storage Account.</param>
        /// <param name="key">Key for the Storage Account.</param>
        public StorageAccountConfiguration(string accountName, string key)
        {
            this.Name = accountName;
            this.Key = key;
        }
    }
}
