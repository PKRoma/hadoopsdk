using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Hadoop.MapReduce.Execution
{
    internal static class StreamingCommands
    {
        public const string Jar = "jar";
        public const string Input = "-input";
        public const string Output = "-output";
        public const string Mapper = "-mapper";
        public const string Reducer = "-reducer";
        public const string Combiner = "-combiner";
        public const string D = "-D";
        public const string File = "-file";
        public const string Files = "-files";
        public const string CmdEnv = "-cmdenv";
        public const string Verbose = "-verbose";
    }
}
