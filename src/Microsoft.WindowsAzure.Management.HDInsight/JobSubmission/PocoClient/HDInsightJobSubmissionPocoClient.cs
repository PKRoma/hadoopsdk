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

namespace Microsoft.WindowsAzure.Management.HDInsight.JobSubmission.PocoClient
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using System.Xml;
    using Microsoft.WindowsAzure.Management.Framework;
    using Microsoft.WindowsAzure.Management.Framework.InversionOfControl;
    using Microsoft.WindowsAzure.Management.HDInsight.ClusterProvisioning.Data;
    using Microsoft.WindowsAzure.Management.HDInsight.ConnectionContext;
    using Microsoft.WindowsAzure.Management.HDInsight.JobSubmission.Data;
    using Microsoft.WindowsAzure.Management.HDInsight.JobSubmission.RestClient;

    /// <summary>
    /// The PocoClient for submitting jobs to an HDInsight server.
    /// </summary>
    public class HDInsightJobSubmissionPocoClient : IHDInsightJobSubmissionPocoClient
    {
        private readonly IConnectionCredentials credentials;

        internal HDInsightJobSubmissionPocoClient(IConnectionCredentials credentials)
        {
            this.credentials = credentials;
        }

        /// <inheritdoc />
        public async Task<HDInsightJobCreationResults> CreateHiveJob(string dnsName, string location, HDInsightHiveJobCreationDetails details)
        {
            JobPayloadConverter converter = new JobPayloadConverter();
            using (var client = ServiceLocator.Instance.Locate<IHDInsightJobSubmissionRestClientFactory>().Create(this.credentials))
            {
                var payload = converter.SerializeJobCreationDetails(details);
                var result = await client.CreateJob(dnsName, location, payload);
                return converter.DeserializeJobCreationResults(result);
            }
        }

        /// <inheritdoc />
        public async Task<HDInsightJobCreationResults> CreateMapReduceJob(string dnsName, string location, HDInsightMapReduceJobCreationDetails details)
        {
            JobPayloadConverter converter = new JobPayloadConverter();
            using (var client = ServiceLocator.Instance.Locate<IHDInsightJobSubmissionRestClientFactory>().Create(this.credentials))
            {
                var payload = converter.SerializeJobCreationDetails(details);
                var result = await client.CreateJob(dnsName, location, payload);
                return converter.DeserializeJobCreationResults(result);
            }
        }

        /// <inheritdoc />
        public async Task<HDInsightJobList> ListJobs(string dnsName, string location)
        {
            List<string> jobIds = new List<string>();
            JobPayloadConverter converter = new JobPayloadConverter();
            using (var client = ServiceLocator.Instance.Locate<IHDInsightJobSubmissionRestClientFactory>().Create(this.credentials))
            {
                var result = await client.ListJobs(dnsName, location);
                return converter.DeserializeJobList(result);
            }
        }

        /// <inheritdoc />
        public async Task<HDInsightJob> GetJobDetail(string dnsName, string location, string jobId)
        {
            JobPayloadConverter converter = new JobPayloadConverter();
            using (var client = ServiceLocator.Instance.Locate<IHDInsightJobSubmissionRestClientFactory>().Create(this.credentials))
            {
                var result = await client.GetJobDetail(dnsName, location, jobId);
                var retval = converter.DeserializeJobDetails(result, jobId);
                return retval;
            }
        }
    }
}
