// /********************************************************
// *                                                       *
// *   Copyright (C) Microsoft. All rights reserved.       *
// *                                                       *
// ********************************************************/

namespace Microsoft.ClusterServices.RDFEProvider.ResourceExtensions.JobSubmission.Models
{
    /// <summary>
    /// Enum for the possible job states. These values are tied/sourced from Tempelton. 
    /// DO NOT UPDATE/CHANGE unless the values returned from Tempelton have changed. 
    /// </summary>
    public enum JobStatusCode
    {
        /// <summary>
        /// Indicates the job is running.
        /// </summary>
        Initializing = 0,

        /// <summary>
        /// Indicates the job is running.
        /// </summary>
        Running = 1,

        /// <summary>
        /// Indicates the job has completed.
        /// </summary>
        Completed = 2,

        /// <summary>
        /// Indicates that the job has failed.
        /// </summary>
        Failed = 3,

        /// <summary>
        /// Indicates that the job is in an unknown state.
        /// </summary>
        Unknown = 4
    }
}