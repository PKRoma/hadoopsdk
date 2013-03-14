using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Hadoop.WebClient.WebHCatClient
{
    public interface IWebHcatMapReduceExecutor
    {
        /// <summary>
        /// A directory where Templeton will write the status of the Map Reduce job. If 
        /// provided, it is the caller's responsibility to remove this directory when done.
        /// </summary>
        string StatusLocation { get; set; }

        /// <summary>
        /// Define a URL to be called upon job completion. You may embed a specific job ID into this 
        /// URL using $jobId. This tag will be replaced in the callback URL with this job's job ID.
        /// </summary>
        string CallBack { get; set; }
    }
}
