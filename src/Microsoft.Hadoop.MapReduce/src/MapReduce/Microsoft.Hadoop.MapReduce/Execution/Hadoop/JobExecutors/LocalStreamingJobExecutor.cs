using System.Text;

namespace Microsoft.Hadoop.MapReduce.Execution.Hadoop
{
    using Microsoft.Hadoop.MapReduce.HdfsExtras;
    using Microsoft.Hadoop.MapReduce.HdfsExtras.Hdfs;

    internal class LocalStreamingJobExecutor : StreamingJobExecutorBase, IStreamingJobExecutor
    {
        internal LocalStreamingJobExecutor(IJobTypeExtractor typeExtractor, 
                                           IMapReduceStreamingExecutor executor,
                                           IHdfsFile hdfsFile) :
                                      base(typeExtractor, executor, hdfsFile)
        {
        }

        public static IStreamingJobExecutor Create()
        {
            return new LocalStreamingJobExecutor(new JobTypeExtractor(), 
                                                 LocalMapReduceStreamingExecutor.Create(),
                                                 LocalHdfsFile.Create());
        }
    }
}
