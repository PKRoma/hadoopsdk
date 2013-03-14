using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Hadoop.MapReduce.Execution.Hadoop
{
    public class StreamingProcessExecutor : ProcessExecutor, IStreamingProcessExecutor
    {
        public void AddJar(string jar)
        {
            this.Arguments.Add(StreamingCommands.Jar);
            this.Arguments.Add(jar);
        }

        public void AddFile(string file)
        {
            this.Arguments.Add(StreamingCommands.File);
            this.Arguments.Add(file);
        }

        public void AddFiles(IEnumerable<string> files)
        {
            if (!ReferenceEquals(files, null) && files.Count() > 0)
            {
                this.Arguments.Add(StreamingCommands.Files);
                this.Arguments.Add("\"" + string.Join(",", files) + "\"");
            }
        }

        internal void AddEqualPair(string key, string value)
        {
            this.Arguments.Add(string.Format("\"{0}={1}\"", key, value));
        }

        public void AddCmdEnv(string cmdEnvKey, string cmdEnvValue)
        {
            this.Arguments.Add(StreamingCommands.CmdEnv);
            this.AddEqualPair(cmdEnvKey, cmdEnvValue);
        }

        public void AddDefine(string propertyKey, string propertyValue)
        {
            this.Arguments.Add(StreamingCommands.D);
            this.AddEqualPair(propertyKey, propertyValue);
        }

        public void AddInput(string input)
        {
            this.Arguments.Add(StreamingCommands.Input);
            this.Arguments.Add(input);
        }

        public void AddOutput(string output)
        {
            this.Arguments.Add(StreamingCommands.Output);
            this.Arguments.Add(output);
        }

        public void AddMapper(string mapper)
        {
            this.Arguments.Add(StreamingCommands.Mapper);
            this.Arguments.Add(mapper);
        }

        public void AddReducer(string reducer)
        {
            this.Arguments.Add(StreamingCommands.Reducer);
            this.Arguments.Add(reducer);
        }

        public void AddCombiner(string combiner)
        {
            this.Arguments.Add(StreamingCommands.Combiner);
            this.Arguments.Add(combiner);
        }
    }
}
