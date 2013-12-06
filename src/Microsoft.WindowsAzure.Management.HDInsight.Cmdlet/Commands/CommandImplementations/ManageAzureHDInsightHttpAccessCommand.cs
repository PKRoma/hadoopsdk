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
    using System.Management.Automation;
    using System.Threading.Tasks;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.Commands.CommandInterfaces;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.DataObjects;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters.Extensions;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.ServiceLocation;

    internal class ManageAzureHDInsightHttpAccessCommand : AzureHDInsightClusterCommand<AzureHDInsightCluster>, IManageAzureHDInsightHttpAccessCommand
    {
        /// <inheritdoc />
        public PSCredential Credential { get; set; }

        /// <inheritdoc />
        public bool Enable { get; set; }

        /// <inheritdoc />
        public string Location { get; set; }

        /// <inheritdoc />
        public override async Task EndProcessing()
        {
            this.Name.ArgumentNotNullOrEmpty("Name");
            this.Location.ArgumentNotNullOrEmpty("Location");

            IHDInsightClient client = this.GetClient();
            if (this.Enable)
            {
                this.Credential.ArgumentNotNull("Credential");
                await client.EnableHttpAsync(this.Name, this.Location, this.Credential.UserName, this.Credential.GetCleartextPassword());
            }
            else
            {
                await client.DisableHttpAsync(this.Name, this.Location);
            }

            var getCommand = ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateGet();
            getCommand.Subscription = this.Subscription;
            getCommand.Certificate = this.Certificate;
            getCommand.Name = this.Name;
            await getCommand.EndProcessing();
            this.Output.AddRange(getCommand.Output);
        }
    }
}
