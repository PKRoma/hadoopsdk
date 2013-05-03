namespace Microsoft.WindowsAzure.Management.HDInsight.InversionOfControl
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.WindowsAzure.Management.Framework;
    using Microsoft.WindowsAzure.Management.Framework.InversionOfControl;
    using Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.Asv;
    using Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.Client;
    using Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.PocoClient;
    using Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.RestClient;
    using Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.WebRequest;
    using Microsoft.WindowsAzure.Management.HDInsight.ConnectionContext;

    /// <summary>
    /// Registers services with the IOC for use by this assembly.
    /// </summary>
    public class SdkServiceLocationRegistrar : IServiceLocationRegistrar
    {
        /// <inheritdoc />
        public void Register(IIocServiceLocationRuntimeManager manager)
        {
            if (manager.IsNull())
            {
                throw new ArgumentNullException("manager");
            }

            manager.RegisterType<IHttpClientAbstractionFactory, HttpClientAbstractionFactory>();
            manager.RegisterType<IHDInsightManagementRestClientFactory, HDInsightManagementRestClientFactory>();
            manager.RegisterType<IHDInsightManagementPocoClientFactory, HDInsightManagementPocoClientFactory>();
            manager.RegisterType<IHDInsightClientFactory, HDInsightClientFactory>();
            manager.RegisterType<IAsvClientFactory, AsvClientFactory>();
            manager.RegisterType<IHDInsightClientFactory, HDInsightClientFactory>();
            manager.RegisterType<IHDInsightSyncClientFactory, HDInsightSyncClientFactory>();
            manager.RegisterType<IConnectionCredentialsFactory, ProductionConnectionCredentialsFactory>();
        }
    }
}
