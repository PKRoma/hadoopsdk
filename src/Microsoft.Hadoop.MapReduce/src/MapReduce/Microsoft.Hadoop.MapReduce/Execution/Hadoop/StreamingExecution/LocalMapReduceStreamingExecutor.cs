using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Hadoop.MapReduce.Execution.Hadoop
{
    using System.Globalization;
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.Hadoop.MapReduce.HdfsExtras;
    using Microsoft.Hadoop.MapReduce.HdfsExtras.Hdfs;
    using Microsoft.Hadoop.WebClient.WebHCatClient;

    public class LocalMapReduceStreamingExecutor : IMapReduceStreamingExecutor
    {
        internal delegate IStreamingProcessExecutor StreamingProcessExecutorConstructor();
        private IHdfsFile hdfsFile;
        private string jobId;
        internal StreamingProcessExecutorConstructor MakeExecutor;

        /// <inheritdoc />
        public Guid JobGuid { get; private set; }

        /// <inheritdoc />
        public bool Verbose { get; set; }

        private LocalMapReduceStreamingExecutor(StreamingProcessExecutorConstructor constructor, IHdfsFile hdfsFile)
        {
            this.JobGuid = Guid.NewGuid();
            this.hdfsFile = hdfsFile;
            this.MakeExecutor = constructor;
            this.File = new List<string>();
            this.Defines = new Dictionary<string, string>();
            this.CmdEnv = new Dictionary<string, string>();
            this.Args = new List<string>();
            this.Inputs = new List<string>();
        }

        /// <summary>
        /// Factory method to create a new <see cref="LocalMapReduceStreamingExecutor"/>.
        /// </summary>
        /// <returns>
        /// A new <see cref="IMapReduceStreamingExecutor"/> specific to this implementation.
        /// </returns>
        public static IMapReduceStreamingExecutor Create()
        {
            return new LocalMapReduceStreamingExecutor(() => new StreamingProcessExecutor(), LocalHdfsFile.Create());
        }

        /// <inheritdoc />
        public ICollection<string> Inputs { get; private set; }

        /// <inheritdoc />
        public string OutputLocation { get; set; }

        /// <inheritdoc />
        public string Mapper { get; set; }

        /// <inheritdoc />
        public string Reducer { get; set; }

        /// <inheritdoc />
        public string Combiner { get; set; }

        /// <inheritdoc />
        public ICollection<string> File { get; private set; }

        /// <inheritdoc />
        public IDictionary<string, string> Defines { get; private set; }

        /// <inheritdoc />
        public IDictionary<string, string> CmdEnv { get; private set; }

        /// <inheritdoc />
        public ICollection<string> Args { get; private set; }

        internal void ProcessOutputLine(StandardLineType lineType, string output)
        {
            if (string.IsNullOrEmpty(output))
            {
                return;
            }

            if (lineType == StandardLineType.StdErr)
            {
                if (output.Contains("Streaming Job Failed!") || output.Contains("Streaming Command Failed!"))
                {
                    this.foundFailureString = true;
                }
                else
                {
                    var idLoc = output.IndexOf(IdString, StringComparison.Ordinal);
                    if (idLoc >= 0)
                    {
                        this.jobId = output.Substring(idLoc + IdString.Length);
                    }
                }
            }
        }

        private bool foundFailureString;

        private string GenerateErrorMessage(bool foundErrorString, string command, string args, int exitCode)
        {
            return string.Format("Process failed ({0}). For hadoop job failure, see job-tracker portal for error information and logs. \r\nCmd = {1} {2}\r\nExitCode={3}",
                                 foundErrorString ? "'Streaming Job Failed!' message" : "non-zero exit code",
                                 command,
                                 args,
                                 exitCode);
        }

        /// <inheritdoc />
        public MapReduceResult Execute(bool throwOnError)
        {
            this.jobId = "(unknown)";
            string dirName = "dotnetcli/" + this.JobGuid.ToString();
            string appDirectory = dirName + "/app";

            this.File.AsParallel().ForAll(file => this.hdfsFile.CopyFromLocal(file, hdfsFile.GetFullyQualifiedPath(appDirectory + "/" + Path.GetFileName(file))));
            IEnumerable<string> remoteFileList = this.File.Select(file => hdfsFile.GetFullyQualifiedPath(appDirectory + "/" + Path.GetFileName(file)));

            var executor = MakeExecutor();
            executor.ProcessOutput = this.ProcessOutputLine;
            executor.Command = EnvironmentUtils.PathToHadoopExe;

            // Starting...
            // This variable must not be fully qualified because it needs to come from 
            // the default container.
            executor.AddJar(EnvironmentUtils.PathToStreamingJar);

            // Generic...
            if (string.IsNullOrEmpty(this.Reducer))
            {
                this.Defines["mapred.reduce.tasks"] = 0.ToString(CultureInfo.InvariantCulture);
            }

            foreach (var define in this.Defines)
            {
                executor.AddDefine(define.Key, define.Value);
            }

            executor.AddFiles(remoteFileList);

            // Regular...
            foreach (var input in this.Inputs)
            {
                executor.AddInput(hdfsFile.GetFullyQualifiedPath(input));
            }

            if (!string.IsNullOrEmpty(this.OutputLocation))
            {
                executor.AddOutput(hdfsFile.GetFullyQualifiedPath(this.OutputLocation));
            }

            if (!string.IsNullOrEmpty(this.Mapper))
            {
                executor.AddMapper(this.Mapper);
            }

            if (!string.IsNullOrEmpty(this.Reducer))
            {
                executor.AddReducer(this.Reducer);
            }
            
            if (!string.IsNullOrEmpty(this.Combiner))
            {
                executor.AddCombiner(this.Combiner);
            }

            if (this.Verbose && !this.Args.Contains(StreamingCommands.Verbose))
            {
                this.Args.Add(StreamingCommands.Verbose);
            }

            foreach (var arg in this.Args)
            {
                executor.Arguments.Add(arg);
            }

            foreach (var cmdEnv in this.CmdEnv)
            {
                executor.AddCmdEnv(cmdEnv.Key, cmdEnv.Value);
            }

            executor.WriteOutputAndErrorToConsole = true;

            var executionTask = executor.Execute();
            executionTask.Wait();

            // NEIN: This needs to be fixed, it is here because out/err lines have not completed.
            ((ProcessExecutor)executor).WaitForProcessCompleteion();

            int exitCode = executionTask.Result;

            if (exitCode != 0 || this.foundFailureString)
            {
                throw new StreamingException(this.GenerateErrorMessage(this.foundFailureString,
                                                                       executor.Command,
                                                                       executor.CreateArgumentString(),
                                                                       exitCode));
            }
            Info info = new Info(string.Join(Environment.NewLine, executor.StandardOut),
                                 string.Join(Environment.NewLine, executor.StandardError),
                                 exitCode);

            string fullCommandString = string.Format("{0} {1}", executor.Command, executor.CreateArgumentString());
            Logger.LogCommand(fullCommandString);

            return new MapReduceResult(this.jobId, info);
        }

        private const string IdString = "INFO streaming.StreamJob: Running job: ";
    }
}
