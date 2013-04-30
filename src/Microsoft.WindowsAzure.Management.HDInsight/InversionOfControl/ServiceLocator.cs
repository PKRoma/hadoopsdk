namespace Microsoft.WindowsAzure.Management.HDInsight.InversionOfControl
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.WindowsAzure.Management.Framework.InversionOfControl;
    using Microsoft.WindowsAzure.Management.HDInsight.Asv;
    using Microsoft.WindowsAzure.Management.HDInsight.ConnectionContext;
    using Microsoft.WindowsAzure.Management.HDInsight.PocoClient;
    using Microsoft.WindowsAzure.Management.HDInsight.RestClient;
    using Microsoft.WindowsAzure.Management.HDInsight.WebRequest;
    using Microsoft.WindowsAzure.Management.HDInsight.Client;

    /// <summary>
    /// Locates Services via IOC container.
    /// </summary>
    public static class ServiceLocator 
    {
        private static object syncObject = new object();
        
        private static IIocServiceLocator locator;

        /// <summary>
        /// Gets the IOC service locator.
        /// </summary>
        public static IIocServiceLocator Instance
        {
            get
            {
                if (ReferenceEquals(locator, null))
                {
                    lock (syncObject)
                    {
                        if (ReferenceEquals(locator, null))
                        {
                            locator = IocServiceLocator.Instance;
                            var runtimeManager = locator.Locate<IIocServiceLocationRuntimeManager>();
                            runtimeManager.RegisterType<IHttpClientAbstractionFactory, HttpClientAbstractionFactory>();
                            runtimeManager.RegisterType<IHDInsightManagementRestClientFactory, HDInsightManagementRestClientFactory>();
                            runtimeManager.RegisterType<IHDInsightManagementPocoClientFactory, HDInsightManagementPocoClientFactory>();
                            runtimeManager.RegisterType<IHDInsightClientFactory, HDInsightClientFactory>();
                            runtimeManager.RegisterType<IAsvClientFactory, AsvClientFactory>();
                            runtimeManager.RegisterType<IHDInsightClientFactory, HDInsightClientFactory>();
                            runtimeManager.RegisterType<IHDInsightSyncClientFactory, HDInsightSyncClientFactory>();
                            runtimeManager.RegisterType<IConnectionCredentialsFactory, ProductionConnectionCredentialsFactory>();
                        }
                    }
                }
                return locator;
            }
        }
    }
}
