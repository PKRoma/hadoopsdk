// Copyright (c) Microsoft Corporation
// All rights reserved.
// 
// Licensed under the Apache License, Version 2.0 (the "License"); you may not
// use this file except in compliance with the License.  You may obtain a copy
// of the License at http://www.apache.org/licenses/LICENSE-2.0
// 
// THIS CODE IS PROVIDED *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
// KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED
// WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE,
// MERCHANTABLITY OR NON-INFRINGEMENT.
// 
// See the Apache Version 2.0 License for specific language governing
// permissions and limitations under the License.

namespace Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.Commands.CommandImplementations
{
    using System;
    using System.Globalization;
    using System.Threading.Tasks;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.Commands.CommandInterfaces;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.DataObjects;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters.Extensions;

    internal class UseAzureHDInsightClusterCommand : AzureHDInsightClusterCommand<AzureHDInsightClusterConnection>, IUseAzureHDInsightClusterCommand
    {
        private const string GrantHttpAccessCmdletName = "Grant Azure HDInsight Http Services Access";

        public override async Task EndProcessing()
        {
            this.Name.ArgumentNotNullOrEmpty("Name");
            IHDInsightClient client = this.GetClient();
            var cluster = await client.GetClusterAsync(this.Name);
            var connection = new AzureHDInsightClusterConnection();
            connection.Credential = this.GetSubscriptionCertificateCredentials();

            if (cluster == null)
            {
                throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, "Failed to connect to cluster :{0}", this.Name));
            }

            connection.Cluster = new AzureHDInsightCluster(cluster);

            if (cluster.State != ClusterState.Running)
            {
                throw new NotSupportedException(
                    string.Format(CultureInfo.InvariantCulture, "Cluster {0} is in an invalid state : {1}", this.Name, cluster.State.ToString()));
            }

            if (string.IsNullOrEmpty(cluster.HttpUserName))
            {
                throw new NotSupportedException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Cluster {0} is not configured for Http Services access.\r\nPlease use the {1} cmdlet to enable Http Services access.",
                        this.Name,
                        GrantHttpAccessCmdletName));
            }

            this.Output.Add(connection);
        }
    }
}
