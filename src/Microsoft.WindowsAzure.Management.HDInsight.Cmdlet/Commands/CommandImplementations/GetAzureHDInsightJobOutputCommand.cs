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
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Hadoop.Client;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.Commands.CommandInterfaces;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.DataObjects;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters;
    using Microsoft.WindowsAzure.Management.HDInsight;
    using Microsoft.WindowsAzure.Management.HDInsight.Framework.Core.Library;

    internal class GetAzureHDInsightJobOutputCommand : AzureHDInsightJobCommand<Stream>, IGetAzureHDInsightJobOutputCommand
    {
        private const string TaskLogDownloadCompleteTemplate = "Task logs for jobDetails {0} were Successfully downloaded to {1}";

        public GetAzureHDInsightJobOutputCommand()
        {
            this.OutputType = JobOutputType.StandardOutput;
        }

        /// <inheritdoc />
        public JobOutputType OutputType { get; set; }

        /// <inheritdoc />
        public string TaskLogsDirectory { get; set; }

        /// <inheritdoc />
        public override Task EndProcessing()
        {
            return this.GetJobOutput(this.JobId);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "The only stream we're not disposing is created when the task logs are downloaded.")]
        private async Task GetJobOutput(string jobId)
        {
            this.JobId.ArgumentNotNullOrEmpty("jobId");
            Stream outputStream = null;

           var hadoopClient = this.GetClient(this.Cluster);
            switch (this.OutputType)
            {
                case JobOutputType.StandardError:
                    outputStream = await hadoopClient.GetJobErrorLogsAsync(jobId);
                    break;
                case JobOutputType.StandardOutput:
                    outputStream = await hadoopClient.GetJobOutputAsync(jobId);
                    break;
                case JobOutputType.TaskSummary:
                    outputStream = await hadoopClient.GetJobTaskLogSummaryAsync(jobId);
                    break;
                case JobOutputType.TaskLogs:
                    this.TaskLogsDirectory.ArgumentNotNullOrEmpty("TaskLogsDirectory");
                    await hadoopClient.DownloadJobTaskLogsAsync(jobId, this.TaskLogsDirectory);
                    var messageStream = new MemoryStream();
                    var downloadCompleteMessage = string.Format(
                        CultureInfo.InvariantCulture, TaskLogDownloadCompleteTemplate, this.JobId, this.TaskLogsDirectory);
                    var messageBytes = Encoding.UTF8.GetBytes(downloadCompleteMessage);
                    messageStream.Write(messageBytes, 0, messageBytes.Length);
                    messageStream.Seek(0, SeekOrigin.Begin);
                    outputStream = messageStream;
                    break;
            }

            this.Output.Add(outputStream);
        }
    }
}
