using System;

namespace Microsoft.ClusterServices.RDFEProvider.ResourceExtensions.JobSubmission.Models
{
    /// <summary>
    /// strongly typed request object for Hive Jobs.
    /// </summary>
    public class HiveJobRequest : JobRequest
    {
        public string Query { get; set; }


        public HiveJobRequest()
        {
            Query = string.Empty;
        }

        public HiveJobRequest(ClientJobRequest request)
            : base(request)
        {
            Query = request.Query;
        }

        public static bool TryParse(ClientJobRequest request, out JobRequest output)
        {
            try
            {
                output = new HiveJobRequest(request);
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