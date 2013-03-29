using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Hadoop.MapReduce.HadoopImplementations
{
    using Microsoft.Hadoop.MapReduce.Execution.Hadoop;
    using Microsoft.Hadoop.MapReduce.HdfsExtras;
    using Microsoft.Hadoop.MapReduce.HdfsExtras.Hdfs;
    using Microsoft.Hadoop.WebClient.WebHCatClient;
    using Microsoft.Hadoop.WebHDFS;

    public class WebHadoop : IHadoop
    {
        internal WebHadoop(IHdfsFile hdfsFile, IStreamingJobExecutor mapReduceExecutor)
        {
            this.StorageSystem = hdfsFile;
            this.MapReduceJob = mapReduceExecutor;
        }

        public static IHadoop Create(Uri clusterName, string userName, string password)
        {
            string clusterAddress = clusterName.Scheme + "://" + clusterName.Host + ":50111";
            string storageAddress = clusterName.Scheme + "://" + clusterName.Host + ":50070";
            IHdfsFile storageSystem = WebHdfsFile.Create(userName, new WebHDFSClient(new Uri(storageAddress), userName));
            return new WebHadoop(storageSystem, WebHcatStreamingJobExecutor.Create(new Uri(clusterAddress), userName, password, storageSystem));
        }

        /// <inheritdoc />
        public IHdfsFile StorageSystem { get; private set; }

        /// <inheritdoc />
        public IStreamingJobExecutor MapReduceJob { get; private set; }
    }
}
