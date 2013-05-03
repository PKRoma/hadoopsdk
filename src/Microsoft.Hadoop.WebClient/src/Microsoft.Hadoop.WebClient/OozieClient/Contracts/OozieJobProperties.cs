using System.Collections.Generic;
using System.Globalization;

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
        private readonly bool? useSystemLibpath;

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

        public OozieJobProperties(string userName, string nameNodeHost, string jobTrackerHost, string appPath, string inputPath,
            string outputPath, bool useSystemLibpath) :
                this(userName, nameNodeHost, jobTrackerHost, appPath, inputPath, outputPath)
        {
            this.useSystemLibpath = useSystemLibpath;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification="Boolean values are not subject to globalization problems [ML]")]
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
            if (useSystemLibpath.HasValue)
            {
                properties["oozie.use.system.libpath"] = useSystemLibpath.Value.ToString().ToLower(CultureInfo.InvariantCulture);
            }
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

        public bool? UseSystemLibpath
        {
            get { return useSystemLibpath;  }
        }
    }
}