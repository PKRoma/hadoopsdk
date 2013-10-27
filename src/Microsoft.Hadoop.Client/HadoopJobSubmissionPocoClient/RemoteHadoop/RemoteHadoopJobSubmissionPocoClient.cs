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
namespace Microsoft.Hadoop.Client.HadoopJobSubmissionPocoClient.RemoteHadoop
{
    using System.Threading.Tasks;
    using Microsoft.Hadoop.Client.Data;
    using Microsoft.Hadoop.Client.HadoopJobSubmissionRestCleint.Remote;
    using Microsoft.Hadoop.Client.WebHCatRest;
    using Microsoft.WindowsAzure.Management.HDInsight.Framework.ServiceLocation;
    using Microsoft.WindowsAzure.Management.HDInsight;

    /// <summary>
    /// Used to submit a jobDetails (plain old class objects) to a remote Hadoop cluster.
    /// </summary>
    internal class RemoteHadoopJobSubmissionPocoClient : IHadoopJobSubmissionPocoClient
    {
        private readonly BasicAuthCredential credentials;
        private readonly IHadoopJobSubmissionRestClient client;

        /// <summary>
        /// Initializes a new instance of the RemoteHadoopJobSubmissionPocoClient class.
        /// </summary>
        /// <param name="credentials">
        /// The credential to use to connect to the cluster.
        /// </param>
        /// <param name="context">
        /// A context that contains a CancellationToken that can be used to cancel events.
        /// </param>
        public RemoteHadoopJobSubmissionPocoClient(BasicAuthCredential credentials, IAbstractionContext context)
        {
            this.credentials = credentials;
            this.client = ServiceLocator.Instance.Locate<IHadoopRemoteJobSubmissionRestClientFactory>().Create(credentials, context);
        }

        /// <inheritdoc />
        public async Task<JobList> ListJobs()
        {
            //NEIN: Any code modification here should add unit tests for this class
            var converter = new PayloadConverter();
            var result = await this.client.ListJobs();
            return converter.DeserializeListJobResult(result.Content);
        }

        /// <inheritdoc />
        public async Task<JobDetails> GetJob(string jobId)
        {
            //NEIN: Any code modification here should add unit tests for this class
            var converter = new PayloadConverter();
            var result = await this.client.GetJob(jobId);
            return converter.DeserializeJobDetails(result.Content);
        }

        /// <inheritdoc />
        public async Task<JobCreationResults> SubmitMapReduceJob(MapReduceJobCreateParameters details)
        {
            //NEIN: Any code modification here should add unit tests for this class
            var converter = new PayloadConverter();
            var payload = converter.SerializeMapReduceRequest(this.credentials.UserName, details);
            var result = await this.client.SubmitMapReduceJob(payload);
            return new JobCreationResults() { JobId = converter.DeserializeJobSubmissionResponse(result.Content) };
        }

        /// <inheritdoc />
        public async Task<JobCreationResults> SubmitHiveJob(HiveJobCreateParameters details)
        {
            //NEIN: Any code modification here should add unit tests for this class
            var converter = new PayloadConverter();
            var payload = converter.SerializeHiveRequest(this.credentials.UserName, details);
            var result = await this.client.SubmitHiveJob(payload);
            return new JobCreationResults() { JobId = converter.DeserializeJobSubmissionResponse(result.Content) };
        }

        /// <inheritdoc />
        public async Task<JobCreationResults> SubmitPigJob(PigJobCreateParameters pigJobCreateParameters)
        {
            //NEIN: Any code modification here should add unit tests for this class
            var converter = new PayloadConverter();
            var payload = converter.SerializePigRequest(this.credentials.UserName, pigJobCreateParameters);
            var result = await this.client.SubmitPigJob(payload);
            return new JobCreationResults() { JobId = converter.DeserializeJobSubmissionResponse(result.Content) };
        }

        public async Task<JobCreationResults> SubmitSqoopJob(SqoopJobCreateParameters sqoopJobCreateParameters)
        {
            //NEIN: Any code modification here should add unit tests for this class
            var converter = new PayloadConverter();
            var payload = converter.SerializeSqoopRequest(this.credentials.UserName, sqoopJobCreateParameters);
            var result = await this.client.SubmitSqoopJob(payload);
            return new JobCreationResults() { JobId = converter.DeserializeJobSubmissionResponse(result.Content) };
        }

        /// <inheritdoc />
        public async Task<JobCreationResults> SubmitStreamingJob(StreamingMapReduceJobCreateParameters pigJobCreateParameters)
        {
            //NEIN: Any code modification here should add unit tests for this class
            var converter = new PayloadConverter();
            var payload = converter.SerializeStreamingMapReduceRequest(this.credentials.UserName, pigJobCreateParameters);
            var result = await this.client.SubmitStreamingMapReduceJob(payload);
            return new JobCreationResults() { JobId = converter.DeserializeJobSubmissionResponse(result.Content) };
        }

        public async Task<JobDetails> StopJob(string jobId)
        {
            var result = await this.client.StopJob(jobId);
            var converter = new PayloadConverter();

            return converter.DeserializeJobDetails(result.Content);
        }
    }
}
