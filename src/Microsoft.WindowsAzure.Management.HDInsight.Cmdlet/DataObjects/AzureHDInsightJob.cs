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
namespace Microsoft.WindowsAzure.Management.HDInsight.Cmdlet.DataObjects
{
    using System;
    using Microsoft.Hadoop.Client;
    using Microsoft.WindowsAzure.Management.HDInsight;
    using Microsoft.WindowsAzure.Management.HDInsight.Framework.Core.Library;

    /// <summary>
    /// Represents an Azure HD Insight jobDetails for the PowerShell Cmdlets.
    /// </summary>
    public class AzureHDInsightJob : AzureHDInsightJobBase
    {
        /// <summary>
        /// Initializes a new instance of the AzureHDInsightJob class.
        /// </summary>
        /// <param name="jobDetails">The HDInsight jobDetails.</param>
        /// <param name="cluster">The cluster that the jobDetails was created against.</param>
        public AzureHDInsightJob(JobDetails jobDetails, string cluster)
            : base(jobDetails)
        {
            jobDetails.ArgumentNotNull("jobDetails");
            this.ExitCode = jobDetails.ExitCode;
            this.Name = jobDetails.Name;
            this.Query = jobDetails.Query;
            this.State = jobDetails.StatusCode.ToString();

            this.Cluster = cluster;
            this.StatusDirectory = jobDetails.StatusDirectory;
            this.SubmissionTime = jobDetails.SubmissionTime;
            this.PercentComplete = jobDetails.PercentComplete;
        }

        /// <summary>
        /// Gets or sets the status directory for the jobDetails.
        /// </summary>
        public string StatusDirectory { get; set; }

        /// <summary>
        /// Gets the exit code for the jobDetails.
        /// </summary>
        public int? ExitCode { get; private set; }

        /// <summary>
        /// Gets the name of the jobDetails.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the query for the jobDetails (if it was a hive jobDetails).
        /// </summary>
        public string Query { get; private set; }

        /// <summary>
        /// Gets the status code for the jobDetails.
        /// </summary>
        public string State { get; private set; }

        /// <summary>
        /// Gets the time the jobDetails was submitted.
        /// </summary>
        public DateTime SubmissionTime { get; private set; }

        /// <summary>
        /// Gets or sets the cluster to which the jobDetails was submitted.
        /// </summary>
        public string Cluster { get; set; }

        /// <summary>
        /// Gets or sets the percentage completion of the jobDetails.
        /// </summary>
        public string PercentComplete { get; set; }
    }
}
