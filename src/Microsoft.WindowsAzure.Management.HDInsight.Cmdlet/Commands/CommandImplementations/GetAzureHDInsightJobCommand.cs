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

    internal class GetAzureHDInsightJobCommand : AzureHDInsightJobCommand<AzureHDInsightJob>, IGetAzureHDInsightJobCommand
    {
        public async override Task EndProcessing()
        {
            var client = this.GetClient(this.Cluster);
            this.Output.Clear();
            if (string.IsNullOrEmpty(this.JobId))
            {
                var jobsList = await client.ListJobsAsync();
                this.Output.AddRange(jobsList.Jobs.Select(hadoopJob => new AzureHDInsightJob(hadoopJob, this.Cluster)));
            }
            else
            {
                var jobDetail = await client.GetJobAsync(this.JobId);
                if (jobDetail != null)
                {
                    this.Output.Add(new AzureHDInsightJob(jobDetail, this.Cluster));
                }
            }
        }
    }
}
