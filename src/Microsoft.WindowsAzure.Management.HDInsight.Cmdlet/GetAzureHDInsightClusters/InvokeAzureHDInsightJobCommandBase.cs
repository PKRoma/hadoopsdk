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
namespace Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.GetAzureHDInsightClusters
{
    using System;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Hadoop.Client;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.Commands.CommandImplementations;
    using Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.DataObjects;
    using Microsoft.WindowsAzure.Management.HDInsight;
    using Microsoft.WindowsAzure.Management.HDInsight.Framework.Core.Library;
    using Microsoft.WindowsAzure.Management.HDInsight.Logging;
    using Microsoft.WindowsAzure.Management.HDInsight.Framework.ServiceLocation;

    internal abstract class InvokeAzureHDInsightJobCommandBase : IInvokeAzureHDInsightJobCommand
    {
        private const decimal Success = 0;

        public JobDetails JobDetailsStatus { get; private set; }

        private void ClientOnJobStatus(object sender, WaitJobStatusEventArgs waitJobStatusEventArgs)
        {
            this.JobDetailsStatus = waitJobStatusEventArgs.JobDetails;
        }

        private const double WaitAnHourInSeconds = 3600;

        private IAzureHDInsightCommandBase client = null;
        private object lockObject = new object();
        private bool canceled;

        public void Cancel()
        {
            lock (this.lockObject)
            {
                this.canceled = true;
                if (this.client.IsNotNull())
                {
                    this.client.Cancel();
                }
            }
        }

        public ILogWriter Logger { get; set; }

        public void ValidateNotCanceled()
        {
            if (this.canceled)
            {
                throw new OperationCanceledException("The operation was canceled by the user.");
            }
        }

        public void SetClient(IAzureHDInsightCommandBase client)
        {
            lock (this.lockObject)
            {
                this.client = client;
            }
        }

        public string JobId { get; private set; }

        public InvokeAzureHDInsightJobCommandBase()
        {
            this.Output = new ObservableCollection<string>();
        }

        public AzureHDInsightHiveJobDefinition JobDefinition { get; set; }

        public AzureHDInsightClusterConnection Connection { get; set; }

        public ObservableCollection<string> Output { get; private set; }

        public async Task EndProcessing()
        {
            this.Connection.ArgumentNotNull("Connection");
            this.JobDefinition.ArgumentNotNull("HiveJob");

            this.Output.Clear();
            var currentConnection = this.Connection;

            this.WriteOutput("Submitting Hive query..");
            var startedJob = await this.StartJob(this.JobDefinition, currentConnection);
            this.JobId = startedJob.JobId;
            this.WriteOutput("Started Hive query with jobDetails Id : {0}", startedJob.JobId);

            var completedJob = await this.WaitForCompletion(startedJob, currentConnection);
            if (completedJob.ExitCode == InvokeHiveCommand.Success)
            {
                await this.WriteJobSuccess(completedJob, currentConnection);
            }
            else
            {
                await this.WriteJobFailure(completedJob, currentConnection);
            }
        }

        public CancellationToken CancellationToken
        {
            get { return this.client.CancellationToken; }
        }

        protected virtual async Task WriteJobFailure(AzureHDInsightJob completedJob, AzureHDInsightClusterConnection currentConnection)
        {
            completedJob.ArgumentNotNull("completedJob");
            currentConnection.ArgumentNotNull("currentConnection");
            this.WriteOutput("Hive query failed.");
            this.WriteOutput(Environment.NewLine);
            this.ValidateNotCanceled();
            var getJobOutputCommand = ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateGetJobOutput();
            this.SetClient(getJobOutputCommand);
            getJobOutputCommand.JobId = completedJob.JobId;
            getJobOutputCommand.Logger = this.Logger;
            getJobOutputCommand.OutputType = JobOutputType.StandardError;
            getJobOutputCommand.AssignCredentialsToCommand(currentConnection.Credential);
            getJobOutputCommand.Cluster = currentConnection.Cluster.Name;
            this.ValidateNotCanceled();
            await getJobOutputCommand.EndProcessing();
            var outputStream = getJobOutputCommand.Output.First();
            var content = new StreamReader(outputStream).ReadToEnd();
            this.WriteOutput(content);
        }

        protected virtual async Task WriteJobSuccess(AzureHDInsightJob completedJob, AzureHDInsightClusterConnection currentConnection)
        {
            completedJob.ArgumentNotNull("completedJob");
            currentConnection.ArgumentNotNull("currentConnection");
            this.WriteOutput("Hive query completed Successfully");
            this.WriteOutput(Environment.NewLine);
            this.ValidateNotCanceled();
            var getJobOutputCommand = ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateGetJobOutput();
            this.SetClient(getJobOutputCommand);
            getJobOutputCommand.JobId = completedJob.JobId;
            getJobOutputCommand.Logger = this.Logger;
            getJobOutputCommand.AssignCredentialsToCommand(currentConnection.Credential);
            getJobOutputCommand.Cluster = currentConnection.Cluster.Name;
            this.ValidateNotCanceled();
            await getJobOutputCommand.EndProcessing();
            var outputStream = getJobOutputCommand.Output.First();
            var content = new StreamReader(outputStream).ReadToEnd();
            this.WriteOutput(content);
        }

        protected virtual async Task<AzureHDInsightJob> StartJob(AzureHDInsightJobDefinition jobDefinition, AzureHDInsightClusterConnection currentConnection)
        {
            jobDefinition.ArgumentNotNull("jobDefinition");
            currentConnection.ArgumentNotNull("currentCluster");
            this.ValidateNotCanceled();
            var startJobCommand = ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateStartJob();
            this.SetClient(startJobCommand);
            startJobCommand.Cluster = currentConnection.Cluster.Name;
            startJobCommand.Logger = this.Logger;
            startJobCommand.AssignCredentialsToCommand(currentConnection.Credential);
            startJobCommand.JobDefinition = jobDefinition;
            this.ValidateNotCanceled();
            await startJobCommand.EndProcessing();
            return startJobCommand.Output.Last();
        }

        protected virtual async Task<AzureHDInsightJob> WaitForCompletion(AzureHDInsightJob startedJob, AzureHDInsightClusterConnection currentConnection)
        {
            startedJob.ArgumentNotNull("startedJob");
            currentConnection.ArgumentNotNull("currentConnection");
            this.ValidateNotCanceled();
            var waitJobCommand = ServiceLocator.Instance.Locate<IAzureHDInsightCommandFactory>().CreateWaitJobs();
            waitJobCommand.JobStatusEvent += this.ClientOnJobStatus;
            this.SetClient(waitJobCommand);
            waitJobCommand.Job = startedJob;
            waitJobCommand.Logger = this.Logger;
            waitJobCommand.WaitTimeoutInSeconds = WaitAnHourInSeconds;
            waitJobCommand.AssignCredentialsToCommand(currentConnection.Credential);
            this.ValidateNotCanceled();
            await waitJobCommand.ProcessRecord();
            return waitJobCommand.Output.Last();
        }

        protected virtual void WriteOutput(string content, params string[] args)
        {
            if (args.Any())
            {
                content = string.Format(CultureInfo.InvariantCulture, content, args);
            }

            this.Output.Add(content);
        }
    }
}
