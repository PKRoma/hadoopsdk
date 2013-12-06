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
    using System.Threading.Tasks;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.Commands.CommandInterfaces;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.DataObjects;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters;

    internal class NewAzureHDInsightClusterConfigCommand : AzureHDInsightCommand<AzureHDInsightConfig>, INewAzureHDInsightClusterConfigCommand
    {
        private readonly AzureHDInsightConfig config = new AzureHDInsightConfig();

        /// <summary>
        ///     Gets or sets the size of the cluster in worker nodes.
        /// </summary>
        public int ClusterSizeInNodes
        {
            get { return this.config.ClusterSizeInNodes; }
            set { this.config.ClusterSizeInNodes = value; }
        }

        public override Task EndProcessing()
        {
            this.Output.Add(this.config);
            return TaskEx.GetCompletedTask();
        }
    }
}
