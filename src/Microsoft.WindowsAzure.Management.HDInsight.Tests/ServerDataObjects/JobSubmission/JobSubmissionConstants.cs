namespace Microsoft.ClusterServices.RDFEProvider.ResourceExtensions.JobSubmission
{
    public static class JobSubmissionConstants
    {
        //Misc job related constants
        public const string ResourceExtentionName = "jobs";
        public const string JobIdPropertyName = "id";
        public const string DefaultJobsContainer = "hdinsightjobhistory";
        public const string SasPolicyName = "HdInsightJobHistoryReadOnlyPolicy";
        public const string AsvFormatString = "asv://{0}@{1}/{2}";
        public const string JobNameDefine = "hdInsightJobName";

        //ErrorId
        public const string InvalidJobSumbmissionRequestErrorId = "INVALID_JOBREQUEST";
        public const string ClusterUnavailableErrorId = "CLUSTER_UNAVAILABLE";
        public const string JobSubmissionFailedErrorId = "JOB_SUBMISSION_FAILED";

        //Log Messages
        public const string JobSubmissionFailedLogMessage = "Job submission failed for cluster: '{0}' ErrorId: '{1}'";
        public const string ClusterRequestErrorLogMessage = "An error was returned from the cluster/container while submitting a job request. Cluster: '{0}' HttpStatusCode: '{1}' ReasonPhrase: '{2}'";
        public const string UnkownJobSubmissionErrorLogMessage = "Job submission failed for an unknown reason. Cluster: '{0}' Details: '{1}'";

        public const string InvalidJobRequestLogMessage = "An invalid job request was submitted. Details: {0}";
        public const string InvalidJobHistoryRequestLogMessage = "An invalid job history request was submitted. RequestUri: {0}";
        public const string PassThroughActionCreationFailedLogMessage = "An action could not be created for the given request. ResourceExtension: '{0}' HttpMethod: '{1}'";
        public const string ContentReadFailureLogMessage = "Failed to read the content of a request. RequestUri: '{0}' HttpMethod: '{1}' Details: '{2}'";
        public const string BypassServerCertValidationLogMessage = "No validating server certificates for cluster: '{0}'";

        public const string JobRequestActionNotFound = "A passthrough action could not be created for the given job request. ResourceName: '{0}' Request: '{1}'";
        public const string JobHistoryRequestActionNotFound = "A passthrough action could not be created for the given job history request. ResourceName: '{0}' RequestUri: '{1}'";
        public const string UnableToParseJobDetailsLogMessage = "Unable to parse job details from the server response. ErrorMessage: '{0}' Server Response: '{1}'";

        public const string FailedToCreateOutputContainer = "Could not create Job History Container. AsvAccountName: '{0}' ContainerName: '{1}' Details: '{2}'";

    }
}