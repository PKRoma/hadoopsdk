using System.Collections.Generic;
using System.Linq;

namespace Microsoft.ClusterServices.RDFEProvider.ResourceExtensions.JobSubmission.Models
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Strongly typed base class for all job requests.
    /// </summary>
    public abstract class JobRequest
    {
        public string Name { get; set; }
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly",
            Justification = "Brought in from services team for testing.  NOT OUR CODE. [tgs]")]
        public IDictionary<string,string> Parameters { get; set; }
        public IEnumerable<string> Resources { get; set; }
        public string OutputStorageAccount { get; set; }

        protected JobRequest()
        {
            Parameters = new Dictionary<string, string>();
            Resources = new List<string>();
        }

        protected JobRequest(ClientJobRequest request)
        {
            Resources = request.Resources.Select(p => p.Value.ToString()).ToList();
            Parameters = request.Parameters.ToDictionary(i => i.Key, e => e.Value.ToString());
            Name = request.JobName;
            Parameters.Add(new KeyValuePair<string, string>(JobSubmissionConstants.JobNameDefine,request.JobName));
            OutputStorageAccount = request.OutputStorageLocation;
        }
    }
}