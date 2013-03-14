namespace Microsoft.Hadoop.MapReduce.HadoopImplementations
{
    using System;
    using Microsoft.Hadoop.MapReduce.Execution.Hadoop;
    using Microsoft.Hadoop.MapReduce.HdfsExtras;
    using Microsoft.Hadoop.MapReduce.HdfsExtras.Hdfs;
    using Microsoft.Hadoop.WebClient.WebHCatClient;
    using Microsoft.Hadoop.WebHDFS;

    public class LocalHadoop : IHadoop
    {
        internal LocalHadoop(IHdfsFile hdfsFile, IStreamingJobExecutor jobExecutor)
        {
            this.StorageSystem = hdfsFile;
            this.MapReduceJob = jobExecutor;
        }

        public static IHadoop Create()
        {
            return new LocalHadoop(LocalHdfsFile.Create(), LocalStreamingJobExecutor.Create());
        }

        /// <inheritdoc />
        public IHdfsFile StorageSystem { get; private set; }

        /// <inheritdoc />
        public IStreamingJobExecutor MapReduceJob { get; private set; }
    }
}
