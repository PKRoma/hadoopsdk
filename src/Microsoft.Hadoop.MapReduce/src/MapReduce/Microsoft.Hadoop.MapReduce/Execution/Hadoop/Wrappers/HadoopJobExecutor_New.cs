using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Hadoop.MapReduce
{
    using Microsoft.Hadoop.MapReduce.Execution.Hadoop;
    using Microsoft.Hadoop.MapReduce.HdfsExtras;
    using Microsoft.Hadoop.MapReduce.HdfsExtras.Hdfs;
    using Microsoft.Hadoop.WebClient.WebHCatClient;

    public static class HadoopJobExecutor
    {
        internal delegate IStreamingJobExecutor StreamingExecutorFactory();
        internal delegate IHdfsFile HdfsFileFactory();

        internal static StreamingExecutorFactory CreateExecutor = LocalStreamingJobExecutor.Create;

        public static MapReduceResult Execute(Type mapper, Type reducer, Type combiner, HadoopJobConfiguration config)
        {
            var result = CreateExecutor().Execute(mapper, reducer, combiner, config);
            return result;
        }

        public static MapReduceResult ExecuteJob<TJobType>(string[] arguments) where TJobType : HadoopJob, new()
        {
            var result = CreateExecutor().ExecuteJob<TJobType>(arguments);
            return result;
            //result.Wait();
            //return result.Result;
        }

        public static MapReduceResult ExecuteJob(Type jobType, string[] argumetns)
        {
            var result = CreateExecutor().ExecuteJob(jobType, argumetns);
            return result;
        }

        public static MapReduceResult ExecuteJob<JobType>() where JobType : HadoopJob, new()
        {
            var result = ExecuteJob<JobType>(null);
            return result;
        }

        /// <summary>
        /// Executes mapper with specified config.
        /// </summary>
        /// <typeparam name="TMapper">The type of the mapper.</typeparam>
        /// <param name="config">The config.</param>
        public static MapReduceResult Execute<TMapper>(HadoopJobConfiguration config)
                                        where TMapper : MapperBase, new()
        {
            var result = CreateExecutor().Execute<TMapper>(config);
            return result;
        }

        /// <summary>
        /// Executes mapper/reducer with specified config.
        /// </summary>
        /// <typeparam name="TMapper">The type of the mapper.</typeparam>
        /// <typeparam name="TReducer">The type of the reducer.</typeparam>
        /// <param name="config">The config.</param>
        public static void Execute<TMapper, TReducer>(HadoopJobConfiguration config)
                     where TMapper : MapperBase, new()
                     where TReducer : ReducerCombinerBase, new()
        {
            var result = CreateExecutor().Execute<TMapper, TReducer>(config);
        }

        /// <summary>
        /// Executes mapper/combiner/reducer with specified config.
        /// </summary>
        /// <typeparam name="TMapper">The type of the mapper.</typeparam>
        /// <typeparam name="TCombiner">The type of the combiner.</typeparam>
        /// <typeparam name="TReducer">The type of the reducer.</typeparam>
        /// <param name="config">The config.</param>
        public static void Execute<TMapper, TCombiner, TReducer>(HadoopJobConfiguration config)
                     where TMapper : MapperBase, new()
                     where TCombiner : ReducerCombinerBase, new()
                     where TReducer : ReducerCombinerBase, new()
        {
            HadoopJobExecutor.Execute(typeof(TMapper), typeof(TCombiner), typeof(TReducer), config);
        }

    }
}
