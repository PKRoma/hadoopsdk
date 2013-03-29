using System.Collections.Generic;

namespace Microsoft.Hadoop.WebClient.OozieClient.Contracts
{
    public class OozieJobProperties
    {
        private readonly string userName;
        private readonly string nameNodeHost;
        private readonly string jobTrackerHost;
        private readonly string appPath;
        private readonly string inputPath;
        private readonly string outputPath;

        public OozieJobProperties(string userName, string nameNodeHost, string jobTrackerHost, string appPath, string inputPath, 
            string outputPath)
        {
            this.userName = userName;
            this.nameNodeHost = nameNodeHost;
            this.jobTrackerHost = jobTrackerHost;
            this.appPath = appPath;
            this.inputPath = inputPath;
            this.outputPath = outputPath;
        }

        public Dictionary<string, string> ToDictionary()
        {
            var properties = new Dictionary<string, string>();
            properties["nameNode"] = nameNodeHost;
            properties["jobTracker"] = jobTrackerHost;
            properties["queueName"] = "default";
            properties["oozie.wf.application.path"] = appPath;
            properties["inputDir"] = inputPath;
            properties["outputDir"] = outputPath;
            properties["user.name"] = userName;
            return properties;
        }

        public string UserName
        {
            get { return userName; }
        }

        public string NameNodeHost
        {
            get { return nameNodeHost; }
        }

        public string JobTrackerHost
        {
            get { return jobTrackerHost; }
        }

        public string AppPath
        {
            get { return appPath; }
        }

        public string InputPath
        {
            get { return inputPath; }
        }

        public string OutputPath
        {
            get { return outputPath; }
        }
    }
}