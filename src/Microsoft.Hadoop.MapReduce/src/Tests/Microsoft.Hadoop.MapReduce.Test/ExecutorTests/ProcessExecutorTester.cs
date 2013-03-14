// Copyright (c) Microsoft Corporation
// All rights reserved.
// 
// Licensed under the Apache License, Version 2.0 (the "License"); you may not
// use this file except in compliance with the License.  You may obtain a copy
// of the License at http://www.apache.org/licenses/LICENSE-2.0
// 
// THIS CODE IS PROVIDED *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
// KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED
// WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE,
// MERCHANTABLITY OR NON-INFRINGEMENT.
// 
// See the Apache Version 2.0 License for specific language governing
// permissions and limitations under the License.

namespace Microsoft.Hadoop.MapReduce.Test.ExecutorTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Hadoop.MapReduce.Execution.Hadoop;
    using Microsoft.Hadoop.MapReduce.Test.ProcessDetailsParser;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    internal class ProcessExecutorTester : ProcessExecutor
    {
        private const string exeFile = "procdetails.exe";
        private Task<int> task;

        private ProcessExecutor underlying = new ProcessExecutor();

        public ProcessDetails ProcessDetails { get; private set; }

        public ProcessExecutorTester()
        {
            this.underlying.Command = exeFile;
            this.underlying.ProcessOutput = this.ProcessOutputLine;
            this.underlying.CacheOutputs = false;
            this.exitCode = int.MinValue;
        }

        private StringBuilder output = new StringBuilder();

        private void ProcessOutputLine(StandardLineType lineType, string output)
        {
            if (lineType == StandardLineType.StdOut)
            {
                this.output.Append(output);
            }
        }

        internal void AssertArgsAndVariables()
        {
            Assert.IsTrue(this.Arguments.SequenceEqual(this.underlying.Arguments));
            Assert.IsTrue(this.EnvironemntVariables.SequenceEqual(this.underlying.EnvironemntVariables));
        }

        public override Task<int> Execute()
        {
            this.underlying.EnvironemntVariables.Clear();
            foreach (KeyValuePair<string, string> environemntVariable in this.EnvironemntVariables)
            {
                this.underlying.EnvironemntVariables.Add(environemntVariable);
            }

            foreach (string argument in this.Arguments)
            {
                this.underlying.Arguments.Add(argument);
            }

            this.task = this.underlying.Execute();
            return this.task;
        }

        private int exitCode;

        public override int ExitCode
        {
            get { return this.exitCode; }
        }


        public void ProcessResult()
        {
            int result = this.task.WaitForResult();
            this.underlying.WaitForProcessCompleteion();
            Assert.AreEqual(0, result);
            this.exitCode = this.underlying.ExitCode;
            this.ProcessDetails = new ProcessDetails(this.output.ToString());
        }
    }
}
