namespace Microsoft.WindowsAzure.Management.HDInsight.Data
{
    /// <summary>
    /// Azure Storage Account Configuration for addition ASV HDInsight drives.
    /// </summary>
    public class AsvAccountConfiguration
    {
        /// <summary>
        /// Gets the AccountName of the Storage Account.
        /// </summary>
        public string AccountName { get; private set; }

        /// <summary>
        /// Gets the Key for the Storage Account.
        /// </summary>
        public string Key { get; private set; }

        /// <summary>
        /// Initializes a new instance of the AsvAccountConfiguration class.
        /// </summary>
        /// <param name="accountName">Account name of the Storage Account.</param>
        /// <param name="key">Key for the Storage Account.</param>
        public AsvAccountConfiguration(string accountName, string key)
        {
            this.AccountName = accountName;
            this.Key = key;
        }
    }
}
