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
    using Microsoft.Hadoop.WebHDFS.Adapters;

    public class HadoopOnAzure : IHadoop
    {
        internal HadoopOnAzure(IHdfsFile hdfsFile, IStreamingJobExecutor mapReduceExecutor)
        {
            this.StorageSystem = hdfsFile;
            this.MapReduceJob = mapReduceExecutor;
        }

        public static IHadoop Create(Uri clusterName, 
                                     string userName,
                                     string hadoopUserName,
                                     string password, 
                                     string storageAccount, 
                                     string storageKey,
                                     string container,
                                     bool createContainerIfMissing)
        {
            BlobStorageAdapter adapter = new BlobStorageAdapter(storageAccount, storageKey, container, createContainerIfMissing);
            IHdfsFile storageSystem = WebHdfsFile.Create(hadoopUserName, new WebHDFSClient(hadoopUserName, adapter));
            return new HadoopOnAzure(storageSystem, WebHcatStreamingJobExecutor.Create(clusterName, userName, password, storageSystem));
        }

        /// <inheritdoc />
        public IHdfsFile StorageSystem { get; private set; }

        /// <inheritdoc />
        public IStreamingJobExecutor MapReduceJob { get; private set; }
    }
}
