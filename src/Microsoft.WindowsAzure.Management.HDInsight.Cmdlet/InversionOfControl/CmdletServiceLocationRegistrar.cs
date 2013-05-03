namespace Microsoft.WindowsAzure.Management.HDInsight.Cmdlet
{
    using System;
    using Microsoft.WindowsAzure.Management.Framework;
    using Microsoft.WindowsAzure.Management.Framework.InversionOfControl;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters;
    using Microsoft.WindowsAzure.Management.HDInsight.InversionOfControl;

    /// <summary>
    /// Registers Cmdlet services with the IoC system.
    /// </summary>
    public class CmdletServiceLocationRegistrar : IServiceLocationRegistrar
    {
        /// <inheritdoc />
        public void Register(IIocServiceLocationRuntimeManager manager)
        {
            if (manager.IsNull())
            {
                throw new ArgumentNullException("manager");
            }

            manager.RegisterType<IAzureHDInsightCommandFactory, AzureHDInsightCommandFactory>();
        }
    }
}
