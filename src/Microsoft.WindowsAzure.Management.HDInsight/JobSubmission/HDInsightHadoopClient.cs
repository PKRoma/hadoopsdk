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
namespace Microsoft.WindowsAzure.Management.HDInsight.JobSubmission
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Hadoop.Client;
    using Microsoft.Hadoop.Client.Storage;
    using Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning;
    using Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.ClusterManager;
    using Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.VersionFinder;
    using Microsoft.WindowsAzure.Management.HDInsight.Framework.Core.Library;
    using Microsoft.WindowsAzure.Management.HDInsight.Framework.ServiceLocation;
    using Microsoft.WindowsAzure.Management.HDInsight.JobSubmission.PocoClient;

    internal class HDInsightHadoopClient : ClientBase, IJobSubmissionClient
    {
        private const string StandardErrorFileName = "stderr";
        private const string StandardOutputFileName = "stdout";
        private const string TaskLogsFileName = "logs/list.txt";
        private const string TaskLogsDirectoryName = "logs";

        private readonly BasicAuthCredential remoteCredentials;
        private readonly ClusterDetails cluster;

        internal HDInsightHadoopClient(JobSubmissionCertificateCredential credential)
        {
            var overrideHandlers = ServiceLocator.Instance.Locate<IHDInsightClusterOverrideManager>().GetHandlers(credential, this.Context);
            ServiceLocator.Instance.Locate<IHDInsightClientFactory>().Create(credential);
            IHDInsightCertificateCredential actualCredentials = ServiceLocator.Instance.Locate<IHDInsightSubscriptionCertificateCredentialsFactory>().Create(credential);
            var hdinsight = ServiceLocator.Instance.Locate<IHDInsightClientFactory>().Create(actualCredentials);
            this.cluster = hdinsight.GetCluster(credential.Cluster);

            var versionFinderClient = overrideHandlers.VersionFinder;
            this.AssertSupportedVersion(this.cluster.VersionNumber, versionFinderClient);

            this.remoteCredentials = new BasicAuthCredential()
            {
                Server = GatewayUriResolver.GetGatewayUri(this.cluster.ConnectionUrl, this.cluster.VersionNumber),
                UserName = this.cluster.HttpUserName,
                Password = this.cluster.HttpPassword
            };
        }

        /// <inheritdoc/>
        public JobList ListJobs()
        {
            return this.ListJobsAsync().WaitForResult();
        }

        /// <inheritdoc/>
        public JobDetails GetJob(string jobId)
        {
            return this.GetJobAsync(jobId).WaitForResult();
        }

        /// <inheritdoc/>
        public JobCreationResults CreateMapReduceJob(MapReduceJobCreateParameters mapReduceJobCreateParameters)
        {
            return this.CreateMapReduceJobAsync(mapReduceJobCreateParameters).WaitForResult();
        }

        /// <inheritdoc/>
        public JobCreationResults CreateStreamingJob(StreamingMapReduceJobCreateParameters streamingMapReduceJobCreateParameters)
        {
            return this.CreateStreamingJobAsync(streamingMapReduceJobCreateParameters).WaitForResult();
        }

        /// <inheritdoc/>
        public JobCreationResults CreateHiveJob(HiveJobCreateParameters hiveJobCreateParameters)
        {
            return this.CreateHiveJobAsync(hiveJobCreateParameters).WaitForResult();
        }

        /// <inheritdoc/>
        public JobCreationResults CreatePigJob(PigJobCreateParameters pigJobCreateParameters)
        {
            return this.CreatePigJobAsync(pigJobCreateParameters).WaitForResult();
        }

        /// <inheritdoc/>
        public JobCreationResults CreateSqoopJob(SqoopJobCreateParameters sqoopJobCreateParameters)
        {
            return this.CreateSqoopJobAsync(sqoopJobCreateParameters).WaitForResult();
        }

        /// <inheritdoc/>
        public JobDetails StopJob(string jobId)
        {
            return this.StopJobAsync(jobId).WaitForResult();
        }

        /// <inheritdoc/>
        public Stream GetJobOutput(string jobId)
        {
            return this.GetJobOutputAsync(jobId).WaitForResult();
        }

        /// <inheritdoc/>
        public Stream GetJobErrorLogs(string jobId)
        {
            return this.GetJobErrorLogsAsync(jobId).WaitForResult();
        }

        /// <inheritdoc/>
        public Stream GetJobTaskLogSummary(string jobId)
        {
            return this.GetJobTaskLogSummaryAsync(jobId).WaitForResult();
        }

        /// <inheritdoc/>
        public void DownloadJobTaskLogs(string jobId, string targetDirectory)
        {
            this.DownloadJobTaskLogsAsync(jobId, targetDirectory).WaitForResult();
        }

        public event EventHandler<WaitJobStatusEventArgs> JobStatusEvent;

        /// <inheritdoc/>
        public void HandleClusterWaitNotifyEvent(JobDetails jobDetails)
        {
            var handler = this.JobStatusEvent;
            if (handler.IsNotNull())
            {
                handler(this, new WaitJobStatusEventArgs(jobDetails));
            }
        }

        /// <inheritdoc/>
        public async Task<JobList> ListJobsAsync()
        {
            var pocoClient = ServiceLocator.Instance.Locate<IHDInsightJobSubmissionPocoClientFactory>().Create(this.remoteCredentials, this.Context);
            return await pocoClient.ListJobs();
        }

        /// <inheritdoc/>
        public async Task<JobDetails> GetJobAsync(string jobId)
        {
            var pocoClient = ServiceLocator.Instance.Locate<IHDInsightJobSubmissionPocoClientFactory>().Create(this.remoteCredentials, this.Context);
            return await pocoClient.GetJob(jobId);
        }

        /// <inheritdoc/>
        public async Task<JobCreationResults> CreateMapReduceJobAsync(MapReduceJobCreateParameters mapReduceJobCreateParameters)
        {
            var pocoClient = ServiceLocator.Instance.Locate<IHDInsightJobSubmissionPocoClientFactory>().Create(this.remoteCredentials, this.Context);
            return await pocoClient.SubmitMapReduceJob(mapReduceJobCreateParameters);
        }

        /// <inheritdoc/>
        public async Task<JobCreationResults> CreateStreamingJobAsync(StreamingMapReduceJobCreateParameters streamingMapReduceJobCreateParameters)
        {
            var pocoClient = ServiceLocator.Instance.Locate<IHDInsightJobSubmissionPocoClientFactory>().Create(this.remoteCredentials, this.Context);
            return await pocoClient.SubmitStreamingJob(streamingMapReduceJobCreateParameters);
        }

        /// <inheritdoc/>
        public async Task<JobCreationResults> CreateHiveJobAsync(HiveJobCreateParameters hiveJobCreateParameters)
        {
            var pocoClient = ServiceLocator.Instance.Locate<IHDInsightJobSubmissionPocoClientFactory>().Create(this.remoteCredentials, this.Context);
            return await pocoClient.SubmitHiveJob(hiveJobCreateParameters);
        }

        /// <inheritdoc/>
        public async Task<JobCreationResults> CreatePigJobAsync(PigJobCreateParameters pigJobCreateParameters)
        {
            var pocoClient = ServiceLocator.Instance.Locate<IHDInsightJobSubmissionPocoClientFactory>().Create(this.remoteCredentials, this.Context);
            return await pocoClient.SubmitPigJob(pigJobCreateParameters);
        }

        /// <inheritdoc/>
        public async Task<JobCreationResults> CreateSqoopJobAsync(SqoopJobCreateParameters sqoopJobCreateParameters)
        {
            var pocoClient = ServiceLocator.Instance.Locate<IHDInsightJobSubmissionPocoClientFactory>().Create(this.remoteCredentials, this.Context);
            return await pocoClient.SubmitSqoopJob(sqoopJobCreateParameters);
        }

        /// <inheritdoc/>
        public async Task<JobDetails> StopJobAsync(string jobId)
        {
            var pocoClient = ServiceLocator.Instance.Locate<IHDInsightJobSubmissionPocoClientFactory>().Create(this.remoteCredentials, this.Context);
            return await pocoClient.StopJob(jobId);
        }

        /// <inheritdoc/>
        public async Task<Stream> GetJobOutputAsync(string jobId)
        {
            return await this.GetJobResultFile(jobId, StandardOutputFileName);
        }

        /// <inheritdoc/>
        public async Task<Stream> GetJobErrorLogsAsync(string jobId)
        {
            return await this.GetJobResultFile(jobId, StandardErrorFileName);
        }

        /// <inheritdoc/>
        public async Task<Stream> GetJobTaskLogSummaryAsync(string jobId)
        {
            return await this.GetJobResultFile(jobId, TaskLogsFileName);
        }

        /// <inheritdoc/>
        public async Task DownloadJobTaskLogsAsync(string jobId, string targetDirectory)
        {
            jobId.ArgumentNotNullOrEmpty("jobId");
            targetDirectory.ArgumentNotNullOrEmpty("targetDirectory");

            var job = await this.GetJobAsync(jobId);
            var storageClient = this.GetStorageClient();
            var storageAccount = this.cluster.DefaultStorageAccount;
            var taskLogsDirectoryPath = GetStatusDirectoryPath(job.StatusDirectory, storageAccount, this.remoteCredentials.UserName, TaskLogsDirectoryName);
            var taskLogDirectoryContents = await storageClient.List(taskLogsDirectoryPath, true);

            // List also returns the directory we're looking into, 
            // so we will ignore the directory as we can't read it.
            foreach (var taskLogFilePath in taskLogDirectoryContents.Where(path => !string.Equals(path.AbsoluteUri, taskLogsDirectoryPath.AbsoluteUri, StringComparison.OrdinalIgnoreCase)))
            {
                // create local file in the targetdirectory.
                var localFilePath = Path.Combine(targetDirectory, taskLogFilePath.Segments.Last());
                var fileContentStream = await storageClient.Read(taskLogFilePath);
                var fileContents = new StreamReader(fileContentStream).ReadToEnd();
                File.WriteAllText(localFilePath, fileContents);
            }
        }

        private void AssertSupportedVersion(Version hdinsightClusterVersion, IVersionFinderClient versionFinderClient)
        {
            switch (versionFinderClient.GetVersionStatus(hdinsightClusterVersion))
            {
                case VersionStatus.Obsolete:
                    throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture, HDInsightConstants.ClusterVersionTooLowForJobSubmissionOperations, hdinsightClusterVersion.ToString(), HDInsightSDKSupportedVersions.MinVersion, HDInsightSDKSupportedVersions.MaxVersion));

                case VersionStatus.ToolsUpgradeRequired:
                    throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture, HDInsightConstants.ClusterVersionTooHighForJobSubmissionOperations, hdinsightClusterVersion.ToString(), HDInsightSDKSupportedVersions.MinVersion, HDInsightSDKSupportedVersions.MaxVersion));
            }
        }

        private async Task<Stream> GetJobResultFile(string jobId, string fileName)
        {
            var job = await this.GetJobAsync(jobId);
            if (job == null || string.IsNullOrEmpty(job.StatusDirectory))
            {
                return new MemoryStream();
            }

            var storageAccount = this.cluster.DefaultStorageAccount;
            var statusDirectoryPath = GetStatusDirectoryPath(job.StatusDirectory, storageAccount, this.remoteCredentials.UserName, fileName);

            var storageClient = this.GetStorageClient();
            return await storageClient.Read(statusDirectoryPath);
        }

        private IStorageAbstraction GetStorageClient()
        {
            var storageCredentials = new WindowsAzureStorageAccountCredentials()
            {
                Name = GetAsvRootDirectory(this.cluster.DefaultStorageAccount.Name),
                Key = this.cluster.DefaultStorageAccount.Key
            };

            var storageClient = ServiceLocator.Instance.Locate<IWabStorageAbstractionFactory>().Create(storageCredentials);
            return storageClient;
        }

        internal static string GetAsvRootDirectory(string hostName)
        {
            return Constants.WabsProtocolSchemeName + hostName;
        }

        internal static Uri GetStatusDirectoryPath(string statusDirectory, WabStorageAccountConfiguration wabStorageAccount, string userAccount, string fileName)
        {
            Uri statusDirectoryPath;
            if (statusDirectory.StartsWith(Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase) || statusDirectory.StartsWith(Constants.WabsProtocol, StringComparison.OrdinalIgnoreCase) ||
                statusDirectory.StartsWith(Constants.WabsProtocol, StringComparison.OrdinalIgnoreCase))
            {
                statusDirectoryPath = new Uri(statusDirectory);
            }
            else if (statusDirectory.StartsWith("/", StringComparison.Ordinal))
            {
                statusDirectoryPath =
                 new Uri(
                     string.Format(
                         CultureInfo.InvariantCulture,
                         "{0}{1}@{2}/{3}/{4}",
                         Constants.WabsProtocolSchemeName,
                         wabStorageAccount.Container,
                         wabStorageAccount.Name,
                         statusDirectory.TrimStart('/'),
                         fileName));
            }
            else
            {
                statusDirectoryPath =
                   new Uri(
                       string.Format(
                           CultureInfo.InvariantCulture,
                           "{0}{1}@{2}/user/{3}/{4}/{5}",
                           Constants.WabsProtocolSchemeName,
                           wabStorageAccount.Container,
                           wabStorageAccount.Name,
                           userAccount,
                           statusDirectory.TrimStart('/'),
                           fileName));
            }

            return statusDirectoryPath;
        }
    }
}
