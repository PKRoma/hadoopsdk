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

namespace Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.PSCmdlets
{
    using System;
    using System.IO;
    using System.Management.Automation;
    using System.Reflection;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.Commands.BaseCommandInterfaces;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.Commands.CommandInterfaces;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.DataObjects;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters.Extensions;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.ServiceLocation;
    using Microsoft.WindowsAzure.Management.HDInsight.Logging;

    /// <summary>
    ///     Cmdlet that lists all the Jobs running on a HDInsight cluster.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, AzureHdInsightPowerShellConstants.AzureHDInsightJobOutput)]
    public class GetAzureHDInsightJobOutputCmdlet : AzureHDInsightCmdlet, IGetAzureHDInsightJobOutputBase
    {
        private readonly IGetAzureHDInsightJobOutputCommand command;

        /// <summary>
        ///     Initializes a new instance of the GetAzureHDInsightJobOutputCmdlet class.
        /// </summary>
        public GetAzureHDInsightJobOutputCmdlet()
        {
            this.command = ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateGetJobOutput();
        }

        /// <inheritdoc />
        [Parameter(Mandatory = false, HelpMessage = "The management certificate used to manage the Azure subscription.")]
        [Alias(AzureHdInsightPowerShellConstants.AliasCert)]
        public X509Certificate2 Certificate
        {
            get { return this.command.Certificate; }
            set { this.command.Certificate = value; }
        }

        /// <inheritdoc />
        [Parameter(Mandatory = false, HelpMessage = "The CloudServiceName to use when managing the HDInsight cluster.")]
        public string CloudServiceName
        {
            get { return this.command.CloudServiceName; }
            set { this.command.CloudServiceName = value; }
        }

        /// <inheritdoc />
        [Parameter(Position = 0, Mandatory = true, HelpMessage = "The endpoint to connect to the Azure HDInsight cluster.", ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        [Alias(AzureHdInsightPowerShellConstants.AliasClusterName)]
        public string Cluster
        {
            get { return this.command.Cluster; }
            set { this.command.Cluster = value; }
        }

        /// <inheritdoc />
        [Parameter(Mandatory = false, HelpMessage = "Specify this switch to download the task logs.")]
        public SwitchParameter DownloadTaskLogs { get; set; }

        /// <inheritdoc />
        [Parameter(Mandatory = false, HelpMessage = "The Endpoint to use when connecting to Azure.")]
        public Uri EndPoint
        {
            get { return this.command.EndPoint; }
            set { this.command.EndPoint = value; }
        }

        /// <inheritdoc />
        [Parameter(Mandatory = true, HelpMessage = "The JobID of the jobDetails to get details for.", ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        [Alias(AzureHdInsightPowerShellConstants.JobId)]
        public string JobId
        {
            get { return this.command.JobId; }
            set { this.command.JobId = value; }
        }

        /// <inheritdoc />
        public JobOutputType OutputType { get; set; }

        /// <inheritdoc />
        [Parameter(Mandatory = false, HelpMessage = "Specify this switch to get the Standard error logs.")]
        public SwitchParameter StandardError { get; set; }

        /// <inheritdoc />
        [Parameter(Mandatory = false, HelpMessage = "Specify this switch to get the Standard output logs.")]
        public SwitchParameter StandardOutput { get; set; }

        /// <inheritdoc />
        [Parameter(Position = 1, Mandatory = true, HelpMessage = "The subscription id for the Azure subscription.")]
        [Alias(AzureHdInsightPowerShellConstants.AliasSub)]
        public string Subscription
        {
            get { return this.command.Subscription; }
            set { this.command.Subscription = value; }
        }

        /// <inheritdoc />
        [Parameter(Mandatory = false, HelpMessage = "The target directory to download the task logs to.")]
        [Alias(AzureHdInsightPowerShellConstants.AliasTaskLogsDirectory)]
        public string TaskLogsDirectory
        {
            get { return this.command.TaskLogsDirectory; }
            set { this.command.TaskLogsDirectory = value; }
        }

        /// <inheritdoc />
        [Parameter(Mandatory = false, HelpMessage = "Specify this switch to get the Task log summary.")]
        public SwitchParameter TaskSummary { get; set; }

        /// <inheritdoc />
        protected override void EndProcessing()
        {
            this.command.Logger = this.Logger;
            this.AssertTaskLogsDirectorySpecified(this.TaskLogsDirectory);

            if (this.StandardError.IsPresent)
            {
                this.command.OutputType = JobOutputType.StandardError;
            }
            else if (this.TaskSummary.IsPresent)
            {
                this.command.OutputType = JobOutputType.TaskSummary;
            }
            else if (this.DownloadTaskLogs.IsPresent)
            {
                this.command.OutputType = JobOutputType.TaskLogs;
            }
            else
            {
                this.command.OutputType = JobOutputType.StandardOutput;
            }

            try
            {
                this.command.Logger = this.Logger;
                Task task = this.command.EndProcessing();
                CancellationToken token = this.command.CancellationToken;
                while (!task.IsCompleted)
                {
                    this.WriteDebugLog();
                    task.Wait(1000, token);
                }
                if (task.IsFaulted)
                {
                    throw new AggregateException(task.Exception);
                }
                foreach (Stream output in this.command.Output)
                {
                    string contents = new StreamReader(output).ReadToEnd();
                    this.WriteObject(contents);
                }
                this.WriteDebugLog();
            }
            catch (Exception ex)
            {
                Type type = ex.GetType();
                this.Logger.Log(Severity.Error, Verbosity.Normal, this.FormatException(ex));
                this.WriteDebugLog();
                if (type == typeof(AggregateException) || type == typeof(TargetInvocationException) || type == typeof(TaskCanceledException))
                {
                    ex.Rethrow();
                }
                else
                {
                    throw;
                }
            }
            this.WriteDebugLog();
        }

        /// <inheritdoc />
        protected override void StopProcessing()
        {
            this.command.Cancel();
        }

        private void AssertTaskLogsDirectorySpecified(string taskLogsDirectory)
        {
            if (this.DownloadTaskLogs.IsPresent && taskLogsDirectory.IsNullOrEmpty())
            {
                throw new PSArgumentException("Please specify the directory to download logs to.", "taskLogsDirectory");
            }
        }
    }
}
