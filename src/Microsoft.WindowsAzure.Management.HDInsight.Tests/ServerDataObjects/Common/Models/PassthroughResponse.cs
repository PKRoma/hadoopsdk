using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.ClusterServices.RDFEProvider.ResourceExtensions.JobSubmission.Models;

namespace Microsoft.ClusterServices.RDFEProvider.ResourceExtensions.Common.Models
{
    /// <summary>
    /// Class that represents a response to a passthrough request.
    /// </summary>
    [KnownType(typeof(List<string>))]
    [KnownType(typeof(JobDetails))]
    public class PassthroughResponse
    {
        public PassthroughErrorResponse Error { get; set; }
        public object Data { get; set; }
    }
}