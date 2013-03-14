using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Hadoop.WebClient.WebHCatClient
{
    using Microsoft.Hadoop.MapReduce.Execution.Hadoop;

    public interface IMapReduceStreamingWebHcatExecutor : IMapReduceStreamingExecutor, IWebHcatMapReduceExecutor
    {
    }
}
