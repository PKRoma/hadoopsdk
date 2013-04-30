namespace Microsoft.Hadoop.MapReduce.Test.ProcessDetailsParser
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using ProcDetailsTestApplication;

    public class ProcessDetails
    {
        public IEnumerable<string> Arguments { get; private set; }
        public IDictionary<string, string> EnvironmentVariables { get; private set; }
        public string WorkingDirectory { get; private set; }
        public IEnumerable<string> DirectoryEntries { get; private set; }

        public ProcessDetails(string content)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(content);
            builder.Replace(ProcDetails.TabReplacementSequence.ToString(), "\t");
            builder.Replace(ProcDetails.CharacterReturnReplacementSequence.ToString(), "\r");
            builder.Replace(ProcDetails.LinefeedReplacementSequence.ToString(), "\n");

            this.EnvironmentVariables = new Dictionary<string, string>();
            string[] sections = content.Split(new string[] { ProcDetails.SectionDelimiter.ToString() }, StringSplitOptions.None);

            // Environment Variables = [0]
            string[] vars = sections[0].Split(new string[] { ProcDetails.EntryDelimiter.ToString() }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var var in vars)
            {
                string[] pair = var.Split(new string[] { ProcDetails.EntryPairDelimiter.ToString() }, StringSplitOptions.None);
                this.EnvironmentVariables.Add(pair[0], pair[1]);
            }

            // Command Line Arguments = [1]
            var arguments = new List<string>();
            arguments.AddRange(sections[1].Split(new string[] { ProcDetails.EntryDelimiter.ToString() }, StringSplitOptions.RemoveEmptyEntries));
            this.Arguments = arguments;

            // Current Working Directory = [2]
            this.WorkingDirectory = sections[2];

            // Current Working Directory Entries = [3]
            var directoryEntries = new List<string>();
            directoryEntries.AddRange(sections[3].Split(new string[] { ProcDetails.EntryDelimiter.ToString() }, StringSplitOptions.RemoveEmptyEntries));
            this.DirectoryEntries = directoryEntries;
        }

    }
}
