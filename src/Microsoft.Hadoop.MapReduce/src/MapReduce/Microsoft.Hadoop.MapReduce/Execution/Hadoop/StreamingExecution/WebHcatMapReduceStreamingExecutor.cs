using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Hadoop.WebClient.WebHCatClient
{
    using System.Globalization;
    using System.IO;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Microsoft.Hadoop.MapReduce;
    using Microsoft.Hadoop.MapReduce.Execution;
    using Microsoft.Hadoop.MapReduce.Execution.Hadoop;
    using Microsoft.Hadoop.MapReduce.HdfsExtras;
    using Microsoft.Hadoop.MapReduce.HdfsExtras.Hdfs;
    using Microsoft.Hadoop.WebHCat.Protocol;
    using Microsoft.Hadoop.WebHDFS;
    using Microsoft.Hadoop.WebHDFS.Adapters;
    using Newtonsoft.Json.Linq;

    public class WebHcatMapReduceStreamingExecutor : IMapReduceStreamingWebHcatExecutor
    {
        private WebHCatHttpClient client;
        private string host;
        private IHdfsFile hdfsFile;

        /// <inheritdoc />
        public Guid JobGuid { get; private set; }

        /// <inheritdoc />
        public bool Verbose { get; set; }

        internal WebHcatMapReduceStreamingExecutor(Uri clusterName, string userName, string password, IHdfsFile hdfsFile)
        {
            this.JobGuid = Guid.NewGuid();
            this.hdfsFile = hdfsFile;
            this.host = clusterName.Host;
            this.client = new WebHCatHttpClient(clusterName, userName, password);
            this.File = new List<string>();
            this.Defines = new Dictionary<string, string>();
            this.CmdEnv = new Dictionary<string, string>();
            this.Args = new List<string>();
            this.Inputs = new List<string>();
        }

        /// <summary>
        /// Factory method to create a new <see cref="WebHcatMapReduceStreamingExecutor"/>.
        /// </summary>
        /// <param name="clusterName">The name of the cluster to connect to.</param>
        /// <param name="userName">The userName to use when connecting to the cluster.</param>
        /// <param name="password">The password to use when connecting to the cluster.</param>
        /// <param name="hdfsFile">The hdfsFile to use for temporary file operations.</param>
        /// <returns>
        /// A new <see cref="IMapReduceStreamingExecutor"/> specific to this implementation.
        /// </returns>
        public static IMapReduceStreamingWebHcatExecutor Create(Uri clusterName, string userName, string password, IHdfsFile hdfsFile)
        {
            return new WebHcatMapReduceStreamingExecutor(clusterName, userName, password, hdfsFile);
        }

        /// <inheritdoc />
        public MapReduceResult Execute(bool throwOnError)
        {
            // use for status dir & output directory.. 
            string dirName = "dotnetcli/" + this.JobGuid.ToString();
            string statusDirectory = this.hdfsFile.GetAbsolutePath(dirName + "/status");
            string appDirectory = dirName + "/app";

            this.File.AsParallel().ForAll(file => this.hdfsFile.CopyFromLocal(file, hdfsFile.GetAbsolutePath(appDirectory + "/" + Path.GetFileName(file))));
            IEnumerable<string> remoteFileList = this.File.Select(file => this.hdfsFile.GetFullyQualifiedPath(appDirectory + "/" + Path.GetFileName(file)) + "#" + Path.GetFileName(file));

            string statusLocation = this.StatusLocation;
            bool deleteStatusLocation = false;
            if (string.IsNullOrEmpty(statusLocation))
            {
                deleteStatusLocation = true;
                statusLocation = statusDirectory;
            }
            else
            {
                statusLocation = this.hdfsFile.GetAbsolutePath(statusLocation);
            }

            if (this.Verbose)
            {
                if (!this.Args.Contains(StreamingCommands.Verbose))
                {
                    this.Args.Add(StreamingCommands.Verbose);
                }
            }
            //var blobAdapter = new BlobStorageAdapter(storageAccount, storageA-ccountKey);
            //blobAdapter.Connect(asvContainer, false);
            //var fsClient = new WebHDFSClient(blobAdapter);

            //files.AsParallel().ForAll(file => { fsClient.CreateFile(file, appDirectory + "/" + Path.GetFileName(file)); });
            //IEnumerable<string> remoteFileList = files.Select(file => appDirectory + "/" + Path.GetFileName(file));

            var streamTask = this.client.CreateMapReduceStreamingJob(this.hdfsFile.GetFullyQualifiedPath(this.Inputs.ElementAt(0)),
                                                                     this.hdfsFile.GetFullyQualifiedPath(this.OutputLocation),
                                                                     this.Mapper,
                                                                     this.Reducer,
                                                                     null,
                                                                     this.Defines,
                                                                     remoteFileList,
                                                                     this.CmdEnv,
                                                                     this.Args,
                                                                     this.hdfsFile.GetFullyQualifiedPath(statusLocation),
                                                                     string.Empty);

            streamTask.Wait();
            string results = streamTask.Result.Content.ReadAsStringAsync().WaitForResult();
            JObject reader = JObject.Parse(results);

            string jobId = reader.Value<string>("id");

            var waitForCompleteTask = this.client.WaitForJobToCompleteAsync(jobId);
            waitForCompleteTask.Wait();

            // Now to get the status information.
            var result = this.client.GetQueue(jobId).WaitForResult();
            string queueResult = result.Content.ReadAsStringAsync().WaitForResult();
            JObject queueResultReader = JObject.Parse(queueResult);
            int exitCode = queueResultReader.Value<int>("exitValue");
            string stdOut = this.hdfsFile.ReadAllText(statusLocation + "/stdout");
            string stdErr = this.hdfsFile.ReadAllText(statusLocation + "/stderr");

            if (deleteStatusLocation)
            {
                this.hdfsFile.Delete(statusLocation);
            }

            Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "Job {0} completed.", jobId));

            //streamTask.ContinueWith(
            //   jobComplete =>
            //   jobComplete.Result.Content.ReadAsAsync<JObject>()
            //              .ContinueWith(jobJson => this.client.WaitForJobToCompleteAsync(jobJson.Result.Value<string>("id")))
            //              .ContinueWith(x => Console.WriteLine("All Done"))
            //              .Wait());
            return new MapReduceResult(jobId, new Info(stdOut, stdErr, exitCode));
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

        /// <inheritdoc />
        public string StatusLocation { get; set; }

        /// <inheritdoc />
        public string CallBack { get; set; }
    }
}
