using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Hadoop.MapReduce.Test.ExecutorTests
{
    using System.Threading.Tasks;
    using Microsoft.Hadoop.MapReduce.Execution.Hadoop;

    public delegate int CommandSimulator(MockProcessExecutor executor);

    public class MockProcessExecutor : IProcessExecutor
    {
        public CommandSimulator Simulator;

        public MockProcessExecutor()
        {
            this.standardError = new List<string>();
            this.standardOut = new List<string>();
            this.Arguments = new List<string>();
            this.EnvironemntVariables = new Dictionary<string, string>();
        }

        public void WriteOutputLine(StandardLineType type, string outputLine)
        {
            List<string> outputList = type == StandardLineType.StdErr ? this.standardError : this.standardOut;
            if (this.WriteOutputAndErrorToConsole)
            {
                Console.WriteLine(outputLine);
            }
            if (this.CacheOutputs)
            {
                outputList.Add(outputLine);
            }
            var proc = this.ProcessOutput;
            if (!ReferenceEquals(proc, null))
            {
                proc(type, outputLine);
            }
        }

        private List<string> standardOut;
        public IEnumerable<string> StandardOut { get; private set; }

        private List<string> standardError;
        public IEnumerable<string> StandardError { get; private set; }

        public ICollection<string> Arguments { get; private set; }
        public IDictionary<string, string> EnvironemntVariables { get; private set; }

        public bool WriteOutputAndErrorToConsole { get; set; }
        public bool CacheOutputs { get; set; }
        public string CreateArgumentString()
        {
            throw new NotImplementedException();
        }

        public ProcessOutputLine ProcessOutput { get; set; }

        public string Command { get; set; }

        public int ExitCode { get; set; }

        private Task<int> task;

        public void WaitForExecution()
        {
            this.task.Wait();
        }

        public Task<int> Execute()
        {
            var sim = this.Simulator;
            if (!ReferenceEquals(sim, null))
            {
                this.task = TaskEx.Run(() => sim(this));
            }
            this.task = TaskEx.Run(() => 0);
            return this.task;
        }
    }
}
