using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Hadoop.MapReduce.Execution.Hadoop
{
    using System.Threading.Tasks;
    using Microsoft.Hadoop.WebClient.WebHCatClient;

    public interface IStreamingJobExecutor
    {
        /// <summary>
        /// Executes the specified job with arguments.
        /// </summary>
        /// <param name="jobType">The job type to execute.</param>
        MapReduceResult ExecuteJob(Type jobType);

        /// <summary>
        /// Executes the specified job with arguments.
        /// </summary>
        /// <param name="jobType">The job type to execute.</param>
        /// <param name="arguments">Arguments to pass to the job.</param>
        MapReduceResult ExecuteJob(Type jobType, string[] arguments);
        
        /// <summary>
        /// Executes the specified job with arguments.
        /// </summary>
        /// <typeparam name="TJobType">The job type to execute.</typeparam>
        MapReduceResult ExecuteJob<TJobType>()
                          where TJobType : HadoopJob, new();
        
        /// <summary>
        /// Executes the specified job.
        /// </summary>
        /// <remarks>
        /// Arguments passed to the job are available via <see cref="Microsoft.Hadoop.MapReduce.ExecutorContext.Arguments"/>.
        /// </remarks>
        /// <typeparam name="TJobType">The job type.</typeparam>
        /// <param name="arguments">Arguments to pass to the job.</param>
        MapReduceResult ExecuteJob<TJobType>(string[] arguments) 
                          where TJobType : HadoopJob, new();

        /// <summary>
        /// Executes mapper/combiner/reducer with specified configuration.
        /// </summary>
        /// <param name="mapper">The type of the mapper to execute.</param>
        /// <param name="reducer">The type of the reducer to execute.</param>
        /// <param name="combiner">The type of the combiner to execute.</param>
        /// <param name="config">The configuration to use during execution.</param>
        MapReduceResult Execute(Type mapper, Type reducer, Type combiner, HadoopJobConfiguration config);

        /// <summary>
        /// Executes mapper/combiner/reducer with specified configuration.
        /// </summary>
        /// <typeparam name="TMapper">The type of the mapper to execute.</typeparam>
        MapReduceResult Execute<TMapper>(HadoopJobConfiguration config) where TMapper : MapperBase, new();

        /// <summary>
        /// Executes mapper/combiner/reducer with specified configuration.
        /// </summary>
        /// <typeparam name="TMapper">The type of the mapper to execute.</typeparam>
        /// <typeparam name="TReducer">The type of the reducer to execute.</typeparam>
        /// <param name="config">The configuration to use during execution.</param>
        MapReduceResult Execute<TMapper, TReducer>(HadoopJobConfiguration config) 
                          where TMapper : MapperBase, new() 
                          where TReducer : ReducerCombinerBase, new();
        
        /// <summary>
        /// Executes mapper/combiner/reducer with specified configuration.
        /// </summary>
        /// <typeparam name="TMapper">The type of the mapper to execute.</typeparam>
        /// <typeparam name="TReducer">The type of the reducer to execute.</typeparam>
        /// <typeparam name="TCombiner">The type of the combiner to execute.</typeparam>
        /// <param name="config">The configuration to use during execution.</param>
        MapReduceResult Execute<TMapper, TReducer, TCombiner>(HadoopJobConfiguration config)
                          where TMapper : MapperBase, new()
                          where TReducer : ReducerCombinerBase, new()
                          where TCombiner : ReducerCombinerBase, new();
    }
}
