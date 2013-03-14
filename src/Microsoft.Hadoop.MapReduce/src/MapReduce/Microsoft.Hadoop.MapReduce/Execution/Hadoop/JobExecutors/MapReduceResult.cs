using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Hadoop.WebClient.WebHCatClient
{
    public class Info
    {
        public Info(string standardOut, string standardError, int exitCode)
        {
            this.StandardError = standardError;
            this.StandardOut = standardOut;
            this.ExitCode = exitCode;
        }
        public string StandardOut { get; private set; }
        public string StandardError { get; private set; }
        public int ExitCode { get; private set; }
    }

    public class MapReduceResult
    {
        public MapReduceResult(string id, Info info)
        {
            this.Id = id;
            this.Info = info;
        }
        public string Id { get; private set; }
        public Info Info { get; private set; }
    }
}
