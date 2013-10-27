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
    using System.Threading.Tasks;
    using Microsoft.Hadoop.Client;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.Commands.CommandInterfaces;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.DataObjects;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters;

    internal class WaitAzureHDInsightJobCommand : AzureHDInsightJobExecutorCommand<AzureHDInsightJob>, IWaitAzureHDInsightJobCommand
    {
        public WaitAzureHDInsightJobCommand()
        {
            this.WaitTimeoutInSeconds = 30 * 60;
        }

        public async Task ProcessRecord()
        {
            var client = this.GetClient(this.Job.Cluster);
            client.JobStatusEvent += this.ClientOnJobStatus;
            var job = new JobDetails() { JobId = this.Job.JobId };
            var jobDetail = await client.WaitForJobCompletionAsync(job, TimeSpan.FromSeconds(this.WaitTimeoutInSeconds), this.tokenSource.Token);
            this.Output.Add(new AzureHDInsightJob(jobDetail, this.Job.Cluster));
        }

        public event EventHandler<WaitJobStatusEventArgs> JobStatusEvent;

        public JobDetails JobDetailsStatus { get; private set; }

        private void ClientOnJobStatus(object sender, WaitJobStatusEventArgs waitJobStatusEventArgs)
        {
            this.JobDetailsStatus = waitJobStatusEventArgs.JobDetails;
            var handler = this.JobStatusEvent;
            if (handler != null)
            {
                handler(sender, waitJobStatusEventArgs);
            }
        }

        public override Task EndProcessing()
        {
            return TaskEx.GetCompletedTask();
        }

        public AzureHDInsightJob Job { get; set; }

        public double WaitTimeoutInSeconds { get; set; }
    }
}
