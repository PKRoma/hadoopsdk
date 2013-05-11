namespace Microsoft.Hadoop.MapReduce.Execution.Hadoop
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Hadoop.MapReduce.HdfsExtras;
    using Microsoft.Hadoop.MapReduce.HdfsExtras.Hdfs;
    using Microsoft.Hadoop.WebClient.WebHCatClient;

    public abstract class StreamingJobExecutorBase : IStreamingJobExecutor
    {
        public StreamingJobExecutorBase(IJobTypeExtractor typeExtractor, 
                                        IMapReduceStreamingExecutor executor,
                                        IHdfsFile hdfsFile)
        {
            this.TypeExtractor = typeExtractor;
            this.Executor = executor;
            this.HdfsFile = hdfsFile;
        }

        protected IJobTypeExtractor TypeExtractor { get; private set; }
        protected IMapReduceStreamingExecutor Executor { get; private set; }
        protected IHdfsFile HdfsFile { get; private set; }

        /// <inheritdoc />
        public MapReduceResult ExecuteJob(Type jobType)
        {
            return this.ExecuteJob(jobType, null);
        }

        /// <inheritdoc />
        public MapReduceResult ExecuteJob(Type jobType, string[] arguments)
        {
            object jobObject = Activator.CreateInstance(jobType, arguments);
            HadoopJob job = (HadoopJob)jobObject;
            ExecutorContext context = new ExecutorContext();
            context.Arguments = arguments;
            job.Initialize(context);
            HadoopJobConfiguration config = job.Configure(context);

            Type mapperType, combinerType, reducerType;
            this.TypeExtractor.ExtractTypes(jobType, out mapperType, out combinerType, out reducerType);

            var retval = this.ExecuteCore(mapperType, reducerType, combinerType, config);
            job.Cleanup(context);
            return retval;
            //return this.ExecuteCore(mapperType, combinerType, reducerType, config)
            //           .ContinueWith(r => { job.Cleanup(context); return r.Result; });
        }

        /// <inheritdoc />
        public MapReduceResult ExecuteJob<TJobType>() where TJobType : HadoopJob, new()
        {
            return this.ExecuteJob<TJobType>(null);
        }

        /// <inheritdoc />
        public MapReduceResult ExecuteJob<TJobType>(string[] arguments) where TJobType : HadoopJob, new()
        {
            return ExecuteJob(typeof(TJobType), arguments);
        }

        /// <summary>
        /// Checks the user-defined map/reduce/combine types to make sure they're valid - and throw if not.
        /// </summary>
        /// <param name="mapperType">The mapper type.</param>
        /// <param name="combinerType">The combiner type.</param>
        /// <param name="reducerType">The reducer type.</param>
        internal void CheckUserTypes(Type mapperType, Type reducerType, Type combinerType)
        {
            if (mapperType != null)
            {
                TypeSystem.CheckUserType(mapperType, typeof(MapperBase));
            }
            else
            {
                throw new StreamingException("Mapper cannot be null");
            }

            if (combinerType != null)
            {
                TypeSystem.CheckUserType(combinerType, typeof(ReducerCombinerBase));
            }

            if (reducerType != null)
            {
                TypeSystem.CheckUserType(reducerType, typeof(ReducerCombinerBase));
            }
        }
        /// <inheritdoc />
        public MapReduceResult Execute(Type mapperType, Type reducerType, Type combinerType, HadoopJobConfiguration config)
        {
            this.CheckUserTypes(mapperType, reducerType, combinerType);

            var retval = this.ExecuteCore(mapperType, reducerType, combinerType, config);
            return retval;
        }

        /// <inheritdoc />
        public MapReduceResult Execute<TMapper>(HadoopJobConfiguration config) where TMapper : MapperBase, new()
        {
            return this.Execute(typeof(TMapper), null, null, config);
        }

        /// <inheritdoc />
        public MapReduceResult Execute<TMapper, TReducer>(HadoopJobConfiguration config)
                                 where TMapper : MapperBase, new()
                                 where TReducer : ReducerCombinerBase, new()
        {
            return this.Execute(typeof(TMapper), typeof(TReducer), null, config);
        }

        /// <inheritdoc />
        public MapReduceResult Execute<TMapper, TReducer, TCombiner>(HadoopJobConfiguration config) 
                                 where TMapper : MapperBase, new() 
                                 where TReducer : ReducerCombinerBase, new() 
                                 where TCombiner : ReducerCombinerBase, new()
        {
            return this.Execute(typeof(TMapper), typeof(TReducer), typeof(TCombiner), config);
        }

        /// <summary>
        /// Performs an implementation specific execution of the underlying map reduce job.
        /// </summary>
        /// <param name="mapper">The type of the mapper to execute.</param>
        /// <param name="reducer">The type of the reducer to execute.</param>
        /// <param name="combiner">The type of the combiner to execute.</param>
        /// <param name="config">The configuration to use during execution.</param>
        protected virtual MapReduceResult ExecuteCore(Type mapper, Type reducer, Type combiner, HadoopJobConfiguration config)
        {
            EnvironmentUtils.CheckHadoopEnvironment();
            this.DeleteOutputFolder(config);
            this.BuildCommand(mapper, reducer, combiner, config);
            return this.Executor.Execute(true);
        }

        /// <summary>
        /// Deletes the output folder if it exists.
        /// </summary>
        internal void DeleteOutputFolder(HadoopJobConfiguration config)
        {
            if (config.DeleteOutputFolder)
            {
                if (this.HdfsFile.Exists(config.OutputFolder))
                {
                    Logger.WriteLine("Output folder exists.. deleting.");
                    this.HdfsFile.Delete(config.OutputFolder);
                }
            }
        }

        /// <summary>
        /// Builds the command execution specific to the implementation.
        /// </summary>
        /// <param name="mapper">The type of the mapper to execute.</param>
        /// <param name="reducer">The type of the reducer to execute.</param>
        /// <param name="combiner">The type of the combiner to execute.</param>
        /// <param name="config">The configuration to use during execution.</param>
        protected virtual void BuildCommand(Type mapper, Type reducer, Type combiner, HadoopJobConfiguration config)
        {
            this.Executor.Inputs.Add(config.InputPath);
            foreach (var input in config.AdditionalInputPath)
            {
                this.Executor.Inputs.Add(input);
            }

            if (config.KeepFailedTaskFiles)
            {
                this.Executor.Defines["keep.failed.task.files"] = "true";
            }

            this.Executor.OutputLocation = config.OutputFolder;
            this.Executor.Mapper = EnvironmentUtils.TaskNode_PathToShippedResource(EnvironmentUtils.MapDriverExeName);
            if (reducer != null)
            {
                this.Executor.Reducer = EnvironmentUtils.TaskNode_PathToShippedResource(EnvironmentUtils.ReduceDriverExeName);
                this.Executor.CmdEnv[EnvironmentUtils.EnvVarName_User_ReducerDLLName] = Path.GetFileName(reducer.Assembly.Location);
                this.Executor.CmdEnv[EnvironmentUtils.EnvVarName_User_ReducerTypeName] = reducer.FullName.Replace(" ", "");
            }
            else
            {
                this.Executor.Reducer = "NONE";
                this.Executor.Defines["mapred.reduce.tasks"] = "0";
            }

            if (combiner != null)
            {
                this.Executor.Combiner = EnvironmentUtils.TaskNode_PathToShippedResource(EnvironmentUtils.CombineDriverExeName);
                this.Executor.CmdEnv[EnvironmentUtils.EnvVarName_User_CombinerDLLName] = Path.GetFileName(combiner.Assembly.Location);
                this.Executor.CmdEnv[EnvironmentUtils.EnvVarName_User_CombinerTypeName] = combiner.FullName.Replace(" ", "");
            }

            List<string> filesToInclude = EnvironmentUtils.BuildFileIncludeList(config.FilesToInclude, config.FilesToExclude);

            if (!filesToInclude.Contains(EnvironmentUtils.PathToMapDriverExe, StringComparer.OrdinalIgnoreCase))
            {
                filesToInclude.Add(EnvironmentUtils.PathToMapDriverExe);
            }
            if (!filesToInclude.Contains(EnvironmentUtils.PathToReduceDriverExe, StringComparer.OrdinalIgnoreCase))
            {
                filesToInclude.Add(EnvironmentUtils.PathToReduceDriverExe);
            }
            if (!filesToInclude.Contains(EnvironmentUtils.PathToCombineDriverExe, StringComparer.OrdinalIgnoreCase))
            {
                filesToInclude.Add(EnvironmentUtils.PathToCombineDriverExe);
            }
            if (!filesToInclude.Contains(EnvironmentUtils.PathToThreadingHelperDll))
            {
                filesToInclude.Add(EnvironmentUtils.PathToThreadingHelperDll);
            }

            foreach (var fileToInclude in filesToInclude)
            {
                this.Executor.File.Add(fileToInclude);
            }

            this.Executor.CmdEnv[EnvironmentUtils.EnvVarName_User_MapperDLLName] = Path.GetFileName(mapper.Assembly.Location);
            this.Executor.CmdEnv[EnvironmentUtils.EnvVarName_User_MapperTypeName] = mapper.FullName.Replace(" ", "");

            List<string> shuffleSortStreamingArguments;
            Dictionary<string, string> shuffleSortDefines;
            this.ExtractShuffleSortParameters(config, out shuffleSortStreamingArguments, out shuffleSortDefines);
            foreach (var shuffleSortDefine in shuffleSortDefines)
            {
                this.Executor.Defines[shuffleSortDefine.Key] = shuffleSortDefine.Value;
            }
            foreach (var shuffleSortStreamingArgument in shuffleSortStreamingArguments)
            {
                if (!this.Executor.Args.Contains(shuffleSortStreamingArgument))
                {
                    this.Executor.Args.Add(shuffleSortStreamingArgument);
                }
            }

            this.Executor.Defines["mapred.map.max.attempts"] = config.MaximumAttemptsMapper.ToString(CultureInfo.InvariantCulture);
            this.Executor.Defines["mapred.reduce.max.attempts"] = config.MaximumAttemptsReducer.ToString(CultureInfo.InvariantCulture);

            if (config.CompressOutput)
            {
                this.Executor.Defines["mapred.output.compress"] = "true";
                this.Executor.Defines["mapred.output.compression.codec"] = "true";
            }

            this.Executor.Verbose = config.Verbose;
        }

        /// <summary>
        /// Extracts the appropriate parameters to put in for the streaming job to get the 
        /// appropriate sort key and shuffle key counts requested.
        /// </summary>
        /// <param name="config">The job configuration.</param>
        /// <param name="additionalStreamingArguments">The extracted streaming arguments.</param>
        /// <param name="additionalDefines">The extracted defines (-D) needed.</param>
        internal void ExtractShuffleSortParameters(HadoopJobConfiguration config,
                                                   out List<string> additionalStreamingArguments, 
                                                   out Dictionary<string, string> additionalDefines)
        {
            additionalStreamingArguments = new List<string>();
            additionalDefines = new Dictionary<string, string>();
            if (config.ShuffleKeyColumnCount != 1 || config.SortKeyColumnCount != 1)
            {
                if (config.SortKeyColumnCount < config.ShuffleKeyColumnCount)
                {
                    throw new StreamingException(string.Format("KeyPartsSort is less than KeyPartsShuffle.  This would produce nondeterministic groups to the reducer. KeyPartsSort:{0} KeyPartsShuffle{1}",
                        config.SortKeyColumnCount, config.ShuffleKeyColumnCount));
                }

                //partitioner settings
                additionalStreamingArguments.Add("-partitioner org.apache.hadoop.mapred.lib.KeyFieldBasedPartitioner");
                additionalDefines.Add("mapreduce.job.output.key.comparator.class", "org.apache.hadoop.mapred.lib.KeyFieldBasedComparator");
                additionalDefines.Add("mapreduce.partition.keypartitioner.options", String.Format("-k{0},{0}", config.ShuffleKeyColumnCount));

                //sorter settings
                additionalDefines.Add("stream.num.map.output.key.fields", config.SortKeyColumnCount.ToString());
                additionalDefines.Add("mapreduce.partition.keycomparator.options", String.Format("-k1,{0}n", config.SortKeyColumnCount)); // n=numeric comparison, if appropriate.
            }
        }

    }
}