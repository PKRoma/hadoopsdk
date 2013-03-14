using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Hadoop.MapReduce.Test
{
    using System.IO;
    using System.Linq;
    using Microsoft.Hadoop.MapReduce.Execution.Hadoop;
    using Microsoft.Hadoop.MapReduce.HdfsExtras;
    using Microsoft.Hadoop.MapReduce.HdfsExtras.Hdfs;
    using Microsoft.Hadoop.MapReduce.Test.ProcessDetailsParser;
    using Microsoft.Hadoop.WebClient.WebHCatClient;
    using Microsoft.Hadoop.WebHDFS;

    [TestClass]
    public class MapReduceBasicTests
    {
        public void RunProcDetailsMapReduce(IHdfsFile hdfsFile, IMapReduceStreamingExecutor executor)
        {
            var inputLocation = "/tests/basicTest/procdetails/input/input.txt";
            var outputLocation = "/tests/basicTest/procdetails/output";
            var procdetailsName = "procdetails.exe";

            hdfsFile.WriteAllText(inputLocation, "input");
            if (hdfsFile.Exists(outputLocation))
            {
                hdfsFile.Delete(outputLocation);
            }

            executor.File.Add(Path.Combine(Directory.GetCurrentDirectory(), procdetailsName));
            executor.Inputs.Add(inputLocation);
            executor.OutputLocation = outputLocation;
            executor.Mapper = procdetailsName;
            executor.Reducer = "NONE";
            var result = executor.Execute(true);
            Assert.AreEqual(0, result.Info.ExitCode, "MapReduce Executor returned non zero exit code.");

            string output = hdfsFile.ReadAllText(outputLocation + "/part-00000");

            var details = new ProcessDetails(output);

            Console.WriteLine("Working Directory");
            Console.WriteLine(details.WorkingDirectory);
            Console.WriteLine();

            Console.WriteLine("Command Line Arguments");
            foreach (var argument in details.Arguments)
            {
                Console.WriteLine(argument);
            }
            Console.WriteLine();

            Console.WriteLine("Environment Variables");
            foreach (var environmentVariable in details.EnvironmentVariables)
            {
                Console.WriteLine(environmentVariable.Key + "=" + environmentVariable.Value);
            }
            Console.WriteLine();

            Console.WriteLine("Directory Entries");
            foreach (var directoryEntry in details.DirectoryEntries)
            {
                Console.WriteLine(directoryEntry);
            }
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        public void LocalMapReduceCanRunProcDetails()
        {
            this.RunProcDetailsMapReduce(LocalHdfsFile.Create(), LocalMapReduceStreamingExecutor.Create());
        }

        [TestMethod]
        [TestCategory("Integration")]
        public void WebHcatMapReduceCanRunProcDetails()
        {

            IMapReduceStreamingExecutor executor = WebHcatMapReduceStreamingExecutor.Create(new Uri("http://localhost:50111"),
                                                                                            "hadoop",
                                                                                            null,
                                                                                            WebHdfsFile.Create("hadoop", new WebHDFSClient(new Uri(@"http://localhost:50070"), "hadoop")));

            IHdfsFile hdfsFile = WebHdfsFile.Create("hadoop", new WebHDFSClient(new Uri(@"http://localhost:50070"), "hadoop"));
            this.RunProcDetailsMapReduce(hdfsFile, executor);
        }
    }
}
