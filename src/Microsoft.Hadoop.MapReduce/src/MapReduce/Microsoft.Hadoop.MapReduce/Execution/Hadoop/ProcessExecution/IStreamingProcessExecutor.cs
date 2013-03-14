using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Hadoop.MapReduce.Execution.Hadoop
{
    public interface IStreamingProcessExecutor : IProcessExecutor
    {
        void AddJar(string jar);
        void AddFile(string file);
        void AddFiles(IEnumerable<string> files);
        void AddCmdEnv(string cmdEnvKey, string cmdEnvValue);
        void AddDefine(string propertyKey, string propertyValue);
        void AddInput(string input);
        void AddOutput(string output);
        void AddMapper(string mapper);
        void AddReducer(string reducer);
        void AddCombiner(string combiner);
    }
}
