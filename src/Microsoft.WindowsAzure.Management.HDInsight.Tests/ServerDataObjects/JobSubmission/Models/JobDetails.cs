using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Microsoft.ClusterServices.RDFEProvider.ResourceExtensions.JobSubmission.Models
{
    public class JobDetails
    {
        /// <summary>
        /// Status Code that represents the jobs state in templelton.
        /// </summary>
        public JobStatusCode StatusCode { get; set; }

        /// <summary>
        /// The "friendly" name for this job.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The time when the job was submitted, in UTC ticks.
        /// </summary>
        public string SubmissionTime;

        /// <summary>
        /// The physical http path to the results file for this job. 
        /// </summary>
        public Uri PhysicalOutputPath { get; set; }

        /// <summary>
        /// The logical (ASV) path to the output file. 
        /// </summary>
        public string LogicalOutputPath { get; set; }

        /// <summary>
        /// The physical (http) path to the error output.
        /// </summary>
        public string ErrorOutputPath { get; set; }

        /// <summary>
        /// The exit code of the job. 
        /// </summary>
        public int ExitCode { get; set; }

        /// <summary>
        /// The Hive query that was used for the job (if applicable).
        /// </summary>
        public string Query { get; set; }
    }

}