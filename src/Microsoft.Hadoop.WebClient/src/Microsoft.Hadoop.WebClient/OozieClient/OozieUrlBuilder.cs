using System;

namespace Microsoft.Hadoop.WebClient.OozieClient
{
    static class OozieUrlBuilder
    {
        internal static string GetJobActionUrl(string jobId, string actionStatement)
        {
            var jobUrl = GetJobUrl(jobId);
            return String.Format("{0}?action={1}", jobUrl, actionStatement);
        }

        internal static string GetJobShowUrl(string jobId, string showStatement)
        {
            var jobUrl = GetJobUrl(jobId);
            return String.Format("{0}?show={1}", jobUrl, showStatement);
        }


        internal static string GetJobUrl(string jobId)
        {
            return String.Format("{0}/{1}", OozieResources.Job, jobId);
        }
    }
}
