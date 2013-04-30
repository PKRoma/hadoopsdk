using System;

namespace Microsoft.Hadoop.WebClient.AmbariClient
{
    static class AmbariUrlBuilder
    {
        public static string GetGetHostComponentMetricUrl(string clusterName, string headnodeName)
        {
            return string.Format(@"{0}/hosts/{1}/host_components/jobtracker?fields=metrics/mapred.JobTracker", clusterName, headnodeName);
        }

        public static string GetAsvMetricsUrl(string storageAccount, DateTime start, DateTime end)
        {
            string timeStr = string.Format("[{0},{1}]", ToLinuxEpoch(start), ToLinuxEpoch(end));
            return string.Format(@"/ambari/asvmetrics/IsotopeWorkerNode/{0}?fields=asv_raw_bytes_uploaded{1}", storageAccount, timeStr);
        }

        public static long ToLinuxEpoch(DateTime val)
        {
            DateTime begin = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return (long)(val.ToUniversalTime() - begin).TotalSeconds;
        }
    }

    public static class DateTimeExtensions
    {
        
    }
}
