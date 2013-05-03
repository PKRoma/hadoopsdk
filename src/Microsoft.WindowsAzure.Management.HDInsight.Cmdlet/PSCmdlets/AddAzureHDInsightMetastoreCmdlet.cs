namespace Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.PSCmdlets
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Automation;
    using System.Text;
    using Microsoft.WindowsAzure.Management.Framework;
    using Microsoft.WindowsAzure.Management.Framework.InversionOfControl;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters;
    using Microsoft.WindowsAzure.Management.HDInsight.InversionOfControl;

    /// <summary>
    /// Adds an AzureHDInsight metastore to the AzureHDInsight configuration.
    /// </summary>
    [Cmdlet(VerbsCommon.Add, AzureHdInsightPowerShellHardCodes.AzureHDInsightMetastore)]
    public class AddAzureHDInsightMetastoreCmdlet : AzureHDInsightCmdlet, IAddAzureHDInsightMetastoreBase
    {
        private IAddAzureHDInsightMetastoreCommand command;

        /// <summary>
        /// Initializes a new instance of the AddAzureHDInsightMetastoreCmdlet class.
        /// </summary>
        public AddAzureHDInsightMetastoreCmdlet()
        {
            this.command = ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateAddMetastore();
        }

        /// <summary>
        /// Gets or sets the Azure HDInsight Configuration for the Azure HDInsight cluster being constructed.
        /// </summary>
        [Parameter(Position = 0, Mandatory = true,
                   HelpMessage = "The HDInsight cluster configuration to use when creating the new cluster (created by New-AzureHDInsightConfig).",
                   ValueFromPipeline = true,
                   ParameterSetName = AzureHdInsightPowerShellHardCodes.ParameterSetAddMetastore)]
        public AzureHDInsightConfig Config
        {
            get { return this.command.Config; }
            set
            {
                if (value.IsNull())
                {
                    throw new ArgumentNullException("value",
                                                    "The value for the configuration can not be null.");
                }
                this.command.Config.ClusterSizeInNodes = value.ClusterSizeInNodes;
                this.command.Config.DefaultStorageAccount = value.DefaultStorageAccount;
                this.command.Config.AdditionalStorageAccounts.AddRange(value.AdditionalStorageAccounts);
                if (value.HiveMetastore.IsNotNull())
                {
                    this.command.Config.HiveMetastore = value.HiveMetastore;
                }
                if (value.OozieMetastore.IsNotNull())
                {
                    this.command.Config.OozieMetastore = value.OozieMetastore;
                }
            }
        }

        /// <summary>
        /// Gets or sets the Azure SQL Server to use for this metastore.
        /// </summary>
        [Parameter(Position = 1, Mandatory = true,
                   HelpMessage = "The Azure SQL Server instance to use for this metastore.",
                   ValueFromPipeline = false,
                   ParameterSetName = AzureHdInsightPowerShellHardCodes.ParameterSetAddMetastore)]
        public string SqlAzureServerName
        {
            get { return this.command.SqlAzureServerName; }
            set { this.command.SqlAzureServerName = value; }
        }

        /// <summary>
        /// Gets or sets the Azure SQL Server database to use for this metastore.
        /// </summary>
        [Parameter(Position = 2, Mandatory = true,
                   HelpMessage = "The database on the Azure SQL Server instance to use for this metastore.",
                   ValueFromPipeline = false,
                   ParameterSetName = AzureHdInsightPowerShellHardCodes.ParameterSetAddMetastore)]
        public string DatabaseName
        {
            get { return this.command.DatabaseName; }
            set { this.command.DatabaseName = value; }
        }

        /// <summary>
        /// Gets or sets the User name to use for the Azure SQL Server database.
        /// </summary>
        [Parameter(Position = 3, Mandatory = true,
                   HelpMessage = "The username to use for the Azure SQL Server database.",
                   ValueFromPipeline = false,
                   ParameterSetName = AzureHdInsightPowerShellHardCodes.ParameterSetAddMetastore)]
        public string UserName
        {
            get { return this.command.UserName; }
            set { this.command.UserName = value; }
        }

        /// <summary>
        /// Gets or sets the password to use for the Azure SQL Server database.
        /// </summary>
        [Parameter(Position = 4, Mandatory = true,
                   HelpMessage = "The password to use for the Azure SQL Server database.",
                   ValueFromPipeline = false,
                   ParameterSetName = AzureHdInsightPowerShellHardCodes.ParameterSetAddMetastore)]
        public string Password
        {
            get { return this.command.Password; }
            set { this.command.Password = value; }
        }

        /// <summary>
        /// Gets or sets the type of AzureHDInsight metastore represented by this object.
        /// </summary>
        [Parameter(Position = 5, Mandatory = true,
                   HelpMessage = "The type of AzureHDInsight metastore represented by this metastore.",
                   ValueFromPipeline = false,
                   ParameterSetName = AzureHdInsightPowerShellHardCodes.ParameterSetAddMetastore)]
        public AzureHDInsightMetastoreType MetastoreType
        {
            get { return this.command.MetastoreType; }
            set { this.command.MetastoreType = value; }
        }

        /// <inheritdoc />
        protected override void ProcessRecord()
        {
            base.ProcessRecord();
        }

        /// <inheritdoc />
        protected override void EndProcessing()
        {
            this.command.EndProcessing();
            foreach (var output in this.command.Output)
            {
                this.WriteObject(output);
            }
        }
    }
}
