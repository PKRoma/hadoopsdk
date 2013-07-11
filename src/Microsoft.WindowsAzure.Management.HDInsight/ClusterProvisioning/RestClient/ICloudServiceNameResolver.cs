namespace Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.RestClient
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    internal interface ICloudServiceNameResolver
    {
        string GetCloudServiceName(Guid subscriptionId, string extensionPrefix, string region);
    }
}
