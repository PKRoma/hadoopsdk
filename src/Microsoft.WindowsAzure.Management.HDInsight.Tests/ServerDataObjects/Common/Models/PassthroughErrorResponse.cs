using System.Net;

namespace Microsoft.ClusterServices.RDFEProvider.ResourceExtensions.Common.Models
{
    /// <summary>
    /// Class to represent an error that has been returned in response to a passthrough request.
    /// </summary>
    public class PassthroughErrorResponse
    {
        public HttpStatusCode StatusCode { get; set; }
        public string ErrorId { get; set; }
    }
}