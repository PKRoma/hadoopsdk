namespace Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    internal interface IAddAzureHDInsightMetastoreBase
    {
        /// <summary>
        /// Gets or sets the AzureHDInsightConfig.
        /// </summary>
        AzureHDInsightConfig Config { get; set; }

        /// <summary>
        /// Gets or sets the Azure SQL Server for the metastore.
        /// </summary>
        string SqlAzureServerName { get; set; }

        /// <summary>
        /// Gets or sets the Azure SQL Server database name.
        /// </summary>
        string DatabaseName { get; set; }

        /// <summary>
        /// Gets or sets the Azure SQL Server username.
        /// </summary>
        string UserName { get; set; }

        /// <summary>
        /// Gets or sets the Azure SQL Server password.
        /// </summary>
        string Password { get; set; }

        /// <summary>
        /// Gets or sets the type of metastore represented by this object.
        /// </summary>
        AzureHDInsightMetastoreType MetastoreType { get; set; }
    }
}
