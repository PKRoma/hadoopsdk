namespace Microsoft.WindowsAzure.Management.HDInsight.Cmdlet
{
    using Microsoft.WindowsAzure.Management.Framework.InversionOfControl;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters;
    using Microsoft.WindowsAzure.Management.HDInsight.InversionOfControl;

    internal static class InitializeIocContainers
    {
        private static bool registered = false;
        private static object lockSync = new object();

        public static void Register()
        {
            if (!registered)
            {
                lock (lockSync)
                {
                    if (!registered)
                    {
                        var manager = ServiceLocator.Instance.Locate<IIocServiceLocationRuntimeManager>();
                        manager.RegisterType<IAzureHDInsightCommandFactory, AzureHDInsightCommandFactory>();
                        registered = true;
                    }
                }
            }
        }
    }
}
