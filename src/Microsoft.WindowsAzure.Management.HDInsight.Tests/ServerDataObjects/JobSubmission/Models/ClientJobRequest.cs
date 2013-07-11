using System.Collections.Generic;

namespace Microsoft.ClusterServices.RDFEProvider.ResourceExtensions.JobSubmission.Models
{
    /// <summary>
    /// Class that represents a job request from the end user (AUX/RDFE in this case).
    /// </summary>
    public class ClientJobRequest
    {
        public string JobName { get; set; }
        public JobType JobType { get; set; }
        public IEnumerable<JobRequestParameter> Parameters { get; set; }
        public IEnumerable<JobRequestParameter> Resources { get; set; }
        public IEnumerable<string> Arguments { get; set; }
        public string JarFile { get; set; }
        public string ApplicationName { get; set; }
        public string OutputStorageLocation { get; set; }
        public string Query { get; set; } 

        public ClientJobRequest()
        {
            this.Parameters = new List<JobRequestParameter>();
            this.Resources = new List<JobRequestParameter>();
            this.Arguments = new List<string>();
        }
    }

    /// <summary>
    /// Class that is used to represent a request parameter (JSON cannot serialize generic dictionaries).
    /// </summary>
    public class JobRequestParameter
    {
        public string Key { get; set; }
        public object Value { get; set; }
    }

    /// <summary>
    /// The type of job being requested. Used in validation and parsing.
    /// </summary>
    public enum JobType
    {
        Hive = 0,
        MapReduce = 1
    }
}