namespace Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    internal class AddAzureHDInsightMetastoreCommand : AzureHDInsightCommand<AzureHDInsightConfig>, IAddAzureHDInsightMetastoreCommand
    {
        private AzureHDInsightMetastore metastore = new AzureHDInsightMetastore();

        public AddAzureHDInsightMetastoreCommand()
        {
            this.Config = new AzureHDInsightConfig();
        }

        public override void EndProcessing()
        {
            if (this.MetastoreType == AzureHDInsightMetastoreType.HiveMetastore)
            {
                this.Config.HiveMetastore = this.metastore;
            }
            else
            {
                this.Config.OozieMetastore = this.metastore;
            }
            this.Output.Add(this.Config);
        }

        public AzureHDInsightConfig Config { get; set; }

        public string SqlAzureServerName
        {
            get { return this.metastore.SqlAzureServerName; }
            set { this.metastore.SqlAzureServerName = value; }
        }
        
        public string DatabaseName
        {
            get { return this.metastore.DatabaseName; }
            set { this.metastore.DatabaseName = value; }
        }

        public string UserName
        {
            get { return this.metastore.UserName; }
            set { this.metastore.UserName = value; }
        }
        
        public string Password
        {
            get { return this.metastore.Password; }
            set { this.metastore.Password = value; }
        }

        public AzureHDInsightMetastoreType MetastoreType
        {
            get { return this.metastore.MetastoreType; }
            set { this.metastore.MetastoreType = value; }
        }
    }
}
