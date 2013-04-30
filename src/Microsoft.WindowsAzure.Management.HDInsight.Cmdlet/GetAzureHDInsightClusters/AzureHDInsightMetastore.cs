namespace Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Represents an AzureHDInsightMetastore.
    /// </summary>
    public class AzureHDInsightMetastore
    {
        /// <summary>
        /// Gets or sets the Azure SQL Server for the metastore.
        /// </summary>
        public string SqlAzureServerName { get; set; }

        /// <summary>
        /// Gets or sets the Azure SQL Server database name.
        /// </summary>
        public string DatabaseName { get; set; }

        /// <summary>
        /// Gets or sets the Azure SQL Server username.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the Azure SQL Server password.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the type of metastore represented by this object.
        /// </summary>
        public AzureHDInsightMetastoreType MetastoreType { get; set; }
    }
}
