using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Hadoop.MapReduce
{
    using Microsoft.Hadoop.MapReduce.Execution.Hadoop;
    using Microsoft.Hadoop.MapReduce.HdfsExtras;
    using Microsoft.Hadoop.MapReduce.HdfsExtras.Hdfs;

    public interface IHadoop
    {
        /// <summary>
        /// Gets a storage system that can be used to manipulate
        /// files in the distributed system.
        /// </summary>
        IHdfsFile StorageSystem { get; }

        /// <summary>
        /// Gets an <see cref="IStreamingJobExecutor"/> that can be 
        /// used to submit job classes to the server for execution.
        /// </summary>
        IStreamingJobExecutor MapReduceJob { get; }
    }
}
