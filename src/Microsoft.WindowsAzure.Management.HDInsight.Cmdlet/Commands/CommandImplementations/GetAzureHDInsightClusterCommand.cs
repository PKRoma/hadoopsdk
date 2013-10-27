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
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.Commands.CommandInterfaces;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.DataObjects;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters;
    using Microsoft.WindowsAzure.Management.HDInsight;
    using Microsoft.WindowsAzure.Management.HDInsight.Framework.Core.Library;

    internal class GetAzureHDInsightClusterCommand : AzureHDInsightClusterCommand<AzureHDInsightCluster>, IGetAzureHDInsightClusterCommand
    {
        public override async Task EndProcessing()
        {
            var client = this.GetClient();
            if (!string.IsNullOrWhiteSpace(this.Name))
            {
                var azureCluster = await client.GetClusterAsync(this.Name);
                if (azureCluster != null)
                {
                    this.Output.Add(new AzureHDInsightCluster(azureCluster));
                }
            }
            else
            {
                this.Output.AddRange(client.ListClusters().Select(c => new AzureHDInsightCluster(c)));
            }
        }
    }
}