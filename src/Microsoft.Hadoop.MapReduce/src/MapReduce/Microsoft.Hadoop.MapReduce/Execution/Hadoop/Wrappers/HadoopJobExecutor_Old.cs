// Copyright (c) Microsoft Corporation
// All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not
// use this file except in compliance with the License.  You may obtain a copy
// of the License at http://www.apache.org/licenses/LICENSE-2.0   
//
// THIS CODE IS PROVIDED *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
// KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED 
// WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, 
// MERCHANTABLITY OR NON-INFRINGEMENT.  
//
// See the Apache Version 2.0 License for specific language governing 
// permissions and limitations under the License. 


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Microsoft.Hadoop.MapReduce
{
    using Microsoft.Hadoop.MapReduce.HdfsExtras;
    using Microsoft.Hadoop.MapReduce.HdfsExtras.Hdfs;

    /// <summary>
    /// Enables execution of a StreamingJob
    /// </summary>
    public static class HadoopJobExecutor_Old
    {
        /// <summary>
        /// Executes the specified job.
        /// </summary>
        /// <typeparam name="JobType">The job type.</typeparam>
        public static void ExecuteJob<JobType>() where JobType : HadoopJob, new()
        {
            ExecuteJob<JobType>(null);
        }

        public static void ExecuteJob(Type jobType, string[] arguments)
        {
            HadoopJob job = (HadoopJob)Activator.CreateInstance(jobType, arguments);
            ExecutorContext context = new ExecutorContext();
            context.Arguments = arguments;
            job.Initialize(context);
            HadoopJobConfiguration config = job.Configure(context);

            Type mapperType, combinerType, reducerType;
            ExtractTypes(jobType, out mapperType, out combinerType, out reducerType);

            HadoopJobExecutor_Old.ExecuteCore(mapperType, combinerType, reducerType, config);
            job.Cleanup(context);
        }

        /// <summary>
        /// Executes the specified job.
        /// </summary>
        /// <remarks>
        /// Arguments passed to the job are available via <see cref="Microsoft.Hadoop.MapReduce.ExecutorContext.Arguments"/>.
        /// </remarks>
        /// <typeparam name="JobType">The job type.</typeparam>
        /// <param name="arguments">Arguments to pass to the job.</param>
        public static void ExecuteJob<JobType>(string[] arguments) where JobType : HadoopJob, new()
        {
            ExecuteJob(typeof(JobType), arguments);
        }

        /// <summary>
        /// Executes mapper with specified config.
        /// </summary>
        /// <typeparam name="TMapper">The type of the mapper.</typeparam>
        /// <param name="config">The config.</param>
        public static void Execute<TMapper>(HadoopJobConfiguration config)
            where TMapper : MapperBase, new()
        {
            HadoopJobExecutor_Old.ExecuteCore(typeof(TMapper), null, null, config);
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
            HadoopJobExecutor_Old.ExecuteCore(typeof(TMapper), null, typeof(TReducer), config);
        }

        /// <summary>
        /// Executes mapper/combiner/reducer with specified config.
        /// </summary>
        /// <typeparam name="TMapper">The type of the mapper.</typeparam>
        /// <typeparam name="TCombiner">The type of the combiner.</typeparam>
        /// <typeparam name="TReducer">The type of the reducer.</typeparam>
        /// <param name="config">The config.</param>
        public static void Execute<TMapper,TCombiner,TReducer>(HadoopJobConfiguration config) 
            where TMapper : MapperBase, new()
            where TCombiner: ReducerCombinerBase, new()
            where TReducer : ReducerCombinerBase, new()
        {
            HadoopJobExecutor_Old.ExecuteCore(typeof(TMapper), typeof(TCombiner), typeof(TReducer), config);
        }


        /// <summary>
        /// Produces the Hadoop command line that will run the job but does not execute it.
        /// </summary>
        /// <returns>Command line.</returns>
        public static string MakeCommandLine<JobType>() where JobType : HadoopJob, new()
        {
            string cmdPath;
            string args;
            JobType job = new JobType();
            ExecutorContext context = new ExecutorContext();
            HadoopJobConfiguration config = job.Configure(context);
            Type mapperType, combinerType, reducerType;
            ExtractTypes<JobType>(out mapperType, out combinerType, out reducerType);
            MakeCmdLineCore(mapperType, combinerType, reducerType, config, out cmdPath, out args);
            return string.Format("{0} {1}", cmdPath, args);
        }

        /// <summary>
        /// Produces the Hadoop command line that will run the job but does not execute it.
        /// </summary>
        /// <returns>Command line.</returns>
        public static string MakeCommandLine<TMapper>(HadoopJobConfiguration config)
            where TMapper : MapperBase, new()
        {
            string cmdPath;
            string args;
            MakeCmdLineCore(typeof(TMapper), null, null, config, out cmdPath, out args);
            return string.Format("{0} {1}", cmdPath, args);
        }

        /// <summary>
        /// Produces the Hadoop command line that will run the job but does not execute it.
        /// </summary>
        /// <returns>Command line.</returns>
        public static string MakeCommandLine<TMapper, TReducer>(HadoopJobConfiguration config)
            where TMapper : MapperBase, new()
            where TReducer : ReducerCombinerBase, new()
        {
            string cmdPath;
            string args;
            MakeCmdLineCore(typeof(TMapper), null, typeof(TReducer), config, out cmdPath, out args);
            return string.Format("{0} {1}", cmdPath, args);
        }

        /// <summary>
        /// Produces the Hadoop command line that will run the job but does not execute it.
        /// </summary>
        /// <returns>Command line.</returns>
        public static string MakeCommandLine<TMapper,TCombiner,TReducer>(HadoopJobConfiguration config)
            where TMapper : MapperBase, new()
            where TCombiner : ReducerCombinerBase, new()
            where TReducer : ReducerCombinerBase, new()
        {
            string cmdPath;
            string args;
            MakeCmdLineCore(typeof(TMapper), typeof(TCombiner), typeof(TReducer), config, out cmdPath, out args);
            return string.Format("{0} {1}", cmdPath, args);
        }


        internal static void ExtractTypes<JobType>(out Type mapperType, out Type combinerType, out Type reducerType)
        {
            mapperType = null;
            combinerType = null;
            reducerType = null;
            
            Type jobTypeBase = typeof(JobType);
            while (!jobTypeBase.Name.StartsWith("HadoopJob`"))
            {
                jobTypeBase = jobTypeBase.BaseType;

                if (jobTypeBase == null)
                {
                    throw new StreamingException("JobType should derive from HadoopJob<>,HadoopJob<,> or HadoopJob<,,>");
                }
            }

            mapperType = jobTypeBase.GetGenericArguments()[0];

            if (jobTypeBase.GetGenericTypeDefinition() == typeof(HadoopJob<,>))
            {
                reducerType = jobTypeBase.GetGenericArguments()[1];
            }

            if (jobTypeBase.GetGenericTypeDefinition() == typeof(HadoopJob<,,>))
            {
                combinerType = jobTypeBase.GetGenericArguments()[1];
                reducerType = jobTypeBase.GetGenericArguments()[2];
            }
        }

        
        /// <summary>
        /// Executes this job.
        /// </summary>
        /// <remarks>
        /// Blocks until the job is complete.
        /// </remarks>
        private static void ExecuteCore(Type mapperType, Type combinerType, Type reducerType, HadoopJobConfiguration config)
        {
            string cmdPath;
            string args;

            EnvironmentUtils.CheckHadoopEnvironment();
            ExecutorUtils.DeleteOutputFolder(LocalHdfsFile.Create, config);
            MakeCmdLineCore(mapperType, combinerType, reducerType, config, out cmdPath, out args);
            string fullCommandString = string.Format("{0} {1}", cmdPath, args);
            Logger.LogCommand(fullCommandString);
            ProcessUtil.RunHadoopCommand_ThrowOnError(cmdPath, args);
        }
        

        internal static void MakeCmdLineCore(Type mapperType, Type combinerType, Type reducerType, HadoopJobConfiguration config, out string cmdPath, out string args)
        {
            ExecutorUtils.CheckUserTypes(mapperType, combinerType, reducerType);

            //check class visibility etc.

            //put the name of the MapReduce class into an env-var eg .cmdenv (MapReduceClass)
            //set vars.

            //d:\data>hadoop jar c:\Apps\dist\lib\hadoop-streaming.jar -input test.txt 
            //-mapper ..\..\jars\Map1.exe -reducer ..\..\jars\Reduce1.exe 
            //-output output37 -file d:\data\Reduce1.exe -file d:\data\Map1.exe


            StringBuilder starterArgsBuilder = new StringBuilder();
            StringBuilder regularArgsBuilder = new StringBuilder();
            StringBuilder genericArgsBuilder = new StringBuilder();

            starterArgsBuilder.AppendFormat("jar {0}", EnvironmentUtils.PathToStreamingJar);

            // -------------------
            // generic options:
            // -------------------

            // generic options must come before command options. (http://hadoop-common.472056.n3.nabble.com/hadoop-streaming-tutorial-with-archives-option-td218845.html)

            //commandLineArgs += " \"-D mapred.map.tasks=1\""; //@TODO: configurable.  NOTE: the quotes around "-D param=value" are critical.
            //commandLineArgs += " \"-D mapred.reduce.tasks=1\"";  //@TODO: configurable.
            //commandLineArgs += " \"-D mapred.job.name=Microsoft-Hadoop-Streaming-Job\""; //@TODO: configurable.
            //commandLineArgs += " \"-Dkeep.failed.task.files=true\""; // help with debugging

            if (config.InputPath == null && (config.AdditionalInputPath == null || config.AdditionalInputPath.Count == 0))
            {
                throw new StreamingException("InputPathHDFS is null and AdditionalInputPathHDFS is empty.");
            }
            if (config.InputPath != null)
            {
                regularArgsBuilder.AppendFormat(" -input {0}", config.InputPath);
            }

            if (config.AdditionalInputPath != null)
            {
                foreach (string inputPath in config.AdditionalInputPath)
                {
                    regularArgsBuilder.AppendFormat(" -input {0}", inputPath);
                }
            }
            regularArgsBuilder.AppendFormat(" -output {0}", config.OutputFolder);

            regularArgsBuilder.AppendFormat(" -mapper {0}", EnvironmentUtils.TaskNode_PathToShippedResource(EnvironmentUtils.MapDriverExeName));

            if (reducerType != null)
            {
                regularArgsBuilder.AppendFormat(" -reducer {0}", EnvironmentUtils.TaskNode_PathToShippedResource(EnvironmentUtils.ReduceDriverExeName));
            }
            else
            {
                genericArgsBuilder.AppendFormat(" -D \"mapred.reduce.tasks=0\"");
            }

            if (combinerType != null)
            {
                regularArgsBuilder.AppendFormat(" -combiner {0}", EnvironmentUtils.TaskNode_PathToShippedResource(EnvironmentUtils.CombineDriverExeName));
            }

            regularArgsBuilder.AppendFormat(" -file {0}", EnvironmentUtils.PathToMapDriverExe);
            regularArgsBuilder.AppendFormat(" -file {0}", EnvironmentUtils.PathToReduceDriverExe);
            regularArgsBuilder.AppendFormat(" -file {0}", EnvironmentUtils.PathToCombineDriverExe);

            List<string> filesToInclude = EnvironmentUtils.BuildFileIncludeList(config.FilesToInclude, config.FilesToExclude);
            


            foreach (string path in filesToInclude)
            {
                regularArgsBuilder.AppendFormat(" -file \"" + path + "\"");
            }

            //Runtime driver will access user type similar to this example:
            //  Type.GetType("Test_MapReduce.BasicStreamingTests+MyMapRed, Test_MapReduce")
            //  Note this relies on the DLLs and Driver EXE being in the same folder (default Fusion rules)

            //Environment variables to communicate with the driver EXEs
            //note: the extra quotes are necessary
            regularArgsBuilder.AppendFormat(" -cmdenv \"{0}={1}\"",
                 EnvironmentUtils.EnvVarName_User_MapperDLLName,
                 Path.GetFileName(mapperType.Assembly.Location)
                );

            regularArgsBuilder.AppendFormat(" -cmdenv \"{0}={1}\"",
                 EnvironmentUtils.EnvVarName_User_MapperTypeName,
                 mapperType.FullName.Replace(" ", "")
                );

            if (combinerType != null)
            {
                regularArgsBuilder.AppendFormat(" -cmdenv \"{0}={1}\"",
                     EnvironmentUtils.EnvVarName_User_CombinerDLLName,
                     Path.GetFileName(combinerType.Assembly.Location)
                    );


                regularArgsBuilder.AppendFormat(" -cmdenv \"{0}={1}\"",
                     EnvironmentUtils.EnvVarName_User_CombinerTypeName,
                     combinerType.FullName.Replace(" ", "")
                    );
            }

            if (reducerType != null)
            {
                regularArgsBuilder.AppendFormat(" -cmdenv \"{0}={1}\"",
                     EnvironmentUtils.EnvVarName_User_ReducerDLLName,
                     Path.GetFileName(reducerType.Assembly.Location)
                    );

                regularArgsBuilder.AppendFormat(" -cmdenv \"{0}={1}\"",
                     EnvironmentUtils.EnvVarName_User_ReducerTypeName,
                     reducerType.FullName.Replace(" ", "")
                    );
            }

            if (config.Verbose)
            {
                regularArgsBuilder.AppendFormat(" -verbose");
            }

            foreach (string genericArg in config.AdditionalGenericArguments)
            {
                genericArgsBuilder.AppendFormat(" {0}", genericArg);
            }

            foreach (string regularArg in config.AdditionalStreamingArguments)
            {
                regularArgsBuilder.AppendFormat(" {0}", regularArg);
            }

            List<string> shuffleSortStreamingArguments;
            Dictionary<string, string> shuffleSortDefines;
            ExecutorUtils.ExtractShuffleSortParameters(config, out shuffleSortStreamingArguments, out shuffleSortDefines);
            if (shuffleSortStreamingArguments.Count > 0)
            {
                regularArgsBuilder.Append(" " + String.Join(" ", shuffleSortStreamingArguments));
            }
            if (shuffleSortDefines.Count > 0)
            {
                genericArgsBuilder.Append(" " + String.Join(" ", shuffleSortDefines.Select(kvp => String.Format("-D \"{0}={1}\"", kvp.Key, kvp.Value))));
            }

            genericArgsBuilder.AppendFormat(" -D \"mapred.map.max.attempts={0}\"", config.MaximumAttemptsMapper);
            genericArgsBuilder.AppendFormat(" -D \"mapred.reduce.max.attempts={0}\"", config.MaximumAttemptsReducer);

            if (config.CompressOutput)
            {
                genericArgsBuilder.AppendFormat(" -D \"mapred.output.compress=true\"");
                genericArgsBuilder.AppendFormat(" -D \"mapred.output.compression.codec=org.apache.hadoop.io.compress.GzipCodec\"");
            }

            // Tidy up and run job
            // ---------------------

            // @@TODO: Ideas to cope with path limitations:
            //   1. pack dependencies as archive and use -archive <archiveFile>
            //   2. pack hadoop settings in config.xml and use -conf <confFile>
            //   3. generic option -files might help.. but its only a small factor improvement.

            // 2047 seems the safest limitation.. (x64 shell should cope with 8191.. but I've not seen it work yet.)
            // http://support.microsoft.com/kb/830473/en-us

            string argsString = string.Format("{0} {1} {2}", starterArgsBuilder.ToString(), genericArgsBuilder.ToString(), regularArgsBuilder.ToString());
            string fullCommandString = string.Format("{0} {1}", EnvironmentUtils.PathToHadoopExe, argsString);

            if (fullCommandString.Length > 2047)
            {
                throw new StreamingException("Command-line is too long");
            }

            // prepare out-vars
            cmdPath = EnvironmentUtils.PathToHadoopExe;
            args = argsString;
        }
    }
}
