using System;
using System.Collections.Generic;

namespace Microsoft.ClusterServices.RDFEProvider.ResourceExtensions.JobSubmission.Models
{
    /// <summary>
    /// Strongly typed object for Map Reduce job requests.
    /// </summary>
    public class MapReduceJobRequest : JobRequest
    {
        public string JarFile { get; set; }
        public string ApplicationName { get; set; }
        public IEnumerable<string> Arguments { get; set; }

        public MapReduceJobRequest()
        {
            Arguments = new List<string>();
            JarFile = string.Empty;
            ApplicationName = string.Empty;
        }

        public MapReduceJobRequest(ClientJobRequest request) : base(request)
        {
            ApplicationName = request.ApplicationName;
            JarFile = request.JarFile;
            Arguments = request.Arguments;
        }

        public static bool TryParse(ClientJobRequest request, out JobRequest output)
        {
            try
            {
                output = new MapReduceJobRequest(request);
                return true;
            }
            catch (Exception)
            {
                output = null;
                return false;
            }
        }
    }
}