namespace Microsoft.Hadoop.WebClient.OozieClient.Contracts
{
    /// <summary>
    /// Oozie job status
    /// </summary>
    public static class OozieJobStatus
    {
        public const string Prep = "PREP";
        public const string Ready = "READY";
        public const string Running = "RUNNING";
        public const string Suspended = "SUSPENDED";
        public const string Succeeded = "SUCCEEDED";
        public const string Killed = "KILLED";
        public const string Failed = "FAILED";
        public const string Ok = "OK";
    }

}
