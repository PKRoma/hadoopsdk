namespace Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters
{
    using System;
    using System.Security.Cryptography.X509Certificates;

    internal interface IAzureHDInsightCommandFactory
    {
        IGetAzureHDInsightClusterCommand CreateGet();

        INewAzureHDInsightClusterCommand CreateCreate();

        IRemoveAzureHDInsightClusterCommand CreateDelete();

        INewAzureHDInsightConfigCommand CreateNewConfig();

        ISetAzureHDInsightDefaultStorageCommand CreateSetDefaultStorage();

        IAddAzureHDInsightStorageCommand CreateAddStorage();

        IAddAzureHDInsightMetastoreCommand CreateAddMetastore();
    }
}