namespace Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters
{
    using System;
    using System.Management.Automation;
    using System.Security.Cryptography.X509Certificates;
    using Microsoft.WindowsAzure.Management.HDInsight.InversionOfControl;

    internal class AzureHDInsightCommandFactory : IAzureHDInsightCommandFactory
    {
        public IGetAzureHDInsightClusterCommand CreateGet()
        {
            return new GetAzureHDInsightClusterCommand();
        }

        public INewAzureHDInsightClusterCommand CreateCreate()
        {
            return new NewAzureHDInsightClusterCommand();
        }

        public IRemoveAzureHDInsightClusterCommand CreateDelete()
        {
            return new RemoveAzureHDInsightClusterCommand();
        }

        public INewAzureHDInsightConfigCommand CreateNewConfig()
        {
            return new NewAzureHDInsightConfigCommand();
        }

        public ISetAzureHDInsightDefaultStorageCommand CreateSetDefaultStorage()
        {
            return new SetAzureHDInsightDefaultStorageCommand();
        }

        public IAddAzureHDInsightStorageCommand CreateAddStorage()
        {
            return new AddAzureHDInsightStorageCommand();
        }

        public IAddAzureHDInsightMetastoreCommand CreateAddMetastore()
        {
            return new AddAzureHDInsightMetastoreCommand();
        }
    }
}