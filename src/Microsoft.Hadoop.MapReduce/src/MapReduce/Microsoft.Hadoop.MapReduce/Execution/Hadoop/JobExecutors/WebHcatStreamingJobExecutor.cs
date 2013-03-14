using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Hadoop.WebClient.WebHCatClient
{
    using System.Threading.Tasks;
    using Microsoft.Hadoop.MapReduce;
    using Microsoft.Hadoop.MapReduce.Execution.Hadoop;
    using Microsoft.Hadoop.MapReduce.HdfsExtras;
    using Microsoft.Hadoop.MapReduce.HdfsExtras.Hdfs;

    public class WebHcatStreamingJobExecutor : StreamingJobExecutorBase
    {
        private IMapReduceStreamingWebHcatExecutor executor;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebHcatStreamingJobExecutor"/> class.
        /// </summary>
        /// <param name="typeExtractor">
        /// A <see cref="IJobTypeExtractor"/> to be used to gather job type information for execution.
        /// </param>
        /// <param name="executor">
        /// A <see cref="IMapReduceStreamingExecutor"/> to be used to execute the job.
        /// </param>
        /// <param name="hdfsFile">
        /// A <see cref="IHdfsFile"/> to be used to work with (job component) files input/output files during execution.
        /// </param>
        internal WebHcatStreamingJobExecutor(IJobTypeExtractor typeExtractor, 
                                             IMapReduceStreamingWebHcatExecutor executor,
                                             IHdfsFile hdfsFile) : 
                                        base(typeExtractor, executor, hdfsFile)
        {
            this.executor = executor;
        }

        protected override void BuildCommand(Type mapper, Type reducer, Type combiner, HadoopJobConfiguration config)
        {
            base.BuildCommand(mapper, reducer, combiner, config);
            string dirName = "dotnetcli/" + this.executor.JobGuid.ToString();
            string statusDirectory = dirName + "/status";
            this.executor.StatusLocation = statusDirectory;
        }

        public static IStreamingJobExecutor Create(Uri clusterName, string userName, string password, IHdfsFile hdfsFile)
        {
            return new WebHcatStreamingJobExecutor(new JobTypeExtractor(), 
                                                   WebHcatMapReduceStreamingExecutor.Create(clusterName, userName, password, hdfsFile),
                                                   hdfsFile);
        }
    }
}
