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

namespace Microsoft.Hadoop.MapReduce.Execution.Hadoop
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    ///     Provides a wrapper around process execution.
    /// </summary>
    public class ProcessExecutor : IProcessExecutor
    {
        private Process process;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessExecutor"/> class.
        /// </summary>
        public ProcessExecutor()
        {
            this.Arguments = new List<string>();
            this.EnvironemntVariables = new Dictionary<string, string>();
            this.CacheOutputs = true;
            foreach (DictionaryEntry entry in Process.GetCurrentProcess().StartInfo.EnvironmentVariables)
            {
                this.EnvironemntVariables.Add(entry.Key.ToString(), entry.Value.ToString());
            }
        }

        /// Indicates that this process execution has been previously executed and can not be reused.
        private bool used;

        /// <inheritdoc />
        private List<string> standardOut;

        /// <inheritdoc />
        private List<string> standardError;

        public IEnumerable<string> StandardOut
        {
            get { return this.standardOut; }
        }

        /// <inheritdoc />
        public IEnumerable<string> StandardError
        {
            get { return this.standardError; }
        }

        /// <inheritdoc />
        public IDictionary<string, string> EnvironemntVariables { get; private set; }

        /// <inheritdoc />
        public ICollection<string> Arguments { get; private set; }

        /// <inheritdoc />
        public bool WriteOutputAndErrorToConsole { get; set; }

        /// <inheritdoc />
        public bool CacheOutputs { get; set; }

        /// <inheritdoc />
        public ProcessOutputLine ProcessOutput { get; set; }

        /// <inheritdoc />
        public string Command { get; set; }

        private void Initialize()
        {
            this.standardOut = new List<string>();
            this.standardError = new List<string>();
        }

        public void WaitForProcessCompleteion()
        {
            this.process.WaitForExit();
        }

        // A helper function used to ensure a string is properly quoted for use.
        protected string PropertyQuoteString(string input)
        {
            // If the argument is supplied quote encoded or contains no spaces, keep it untouched
            if ((input[0] == '"' && input[input.Length - 1] == '"') || !input.Contains(' '))
            {
                return input;
            }

            // Replace any backslashes with a double backslash.
            input = input.Replace(@"\", @"\\");
            // Replace any quotes with a backslash quote pair.
            input = input.Replace("\"", "\\\"");
            return "\"" + input + "\"";
        }

        /// <inheritdoc />
        public string CreateArgumentString()
        {
            // TODO: Minor Improvement, This processing can be performed in one pass of the argument strings.
            // NOTE: Improving use one pass is not urgent given expected input sizes and call frequency.

            string[] args = this.Arguments.ToArray();
            var builder = new StringBuilder();
            for (int i = 0; i < args.Length; i++)
            {
                // Append space between arguments.
                if (i > 0)
                {
                    builder.Append(' ');
                }

                builder.Append(this.PropertyQuoteString(args[i]));
            }
            return builder.ToString();
        }

        internal ProcessStartInfo CreateProcessStartInfo()
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = this.Command,
                Arguments = this.CreateArgumentString(),
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
            };
            foreach (KeyValuePair<string, string> environemntVariable in this.EnvironemntVariables)
            {
                startInfo.EnvironmentVariables[environemntVariable.Key] = environemntVariable.Value;
            }
            return startInfo;
        }

        /// The exit code returned from process execution.
        private int exitCode;

        /// <inheritdoc />
        public virtual int ExitCode
        {
            get { return this.exitCode; }
        }

        /// <inheritdoc />
        public virtual Task<int> Execute()
        {
            if (this.used)
            {
                throw new InvalidOperationException("ProcessExecutor.Execute can not be called more than once.");
            }
            this.used = true;
            this.Initialize();

            this.process = new Process { StartInfo = this.CreateProcessStartInfo(), EnableRaisingEvents = true };

            var taskCompleteionSource = new TaskCompletionSource<int>();

            this.process.Exited += (sender, args) =>
            {
                this.exitCode = this.process.ExitCode;
                taskCompleteionSource.TrySetResult(this.process.ExitCode);
            };

            this.process.ErrorDataReceived += this.ReceiveStandardError;
            this.process.OutputDataReceived += this.ReceiveStandardOut;

            if (this.process.Start() == false)
            {
                throw new InvalidOperationException("Unable to start the process");
            }

            this.process.BeginErrorReadLine();
            this.process.BeginOutputReadLine();

            return taskCompleteionSource.Task;
        }

        // Handles a line of standard output coming in from the command line.
        private void ReceiveStandardOut(object sendingProcess, DataReceivedEventArgs args)
        {
            if (this.WriteOutputAndErrorToConsole)
            {
                Console.Out.WriteLine(args.Data);
            }
            if (this.CacheOutputs)
            {
                this.standardOut.Add(args.Data);
            }
            ProcessOutputLine procOutput = this.ProcessOutput;
            if (!ReferenceEquals(procOutput, null))
            {
                procOutput(StandardLineType.StdOut, args.Data);
            }
        }

        // Handles a line of standard error coming in from the command line.
        private void ReceiveStandardError(object sendingProcess, DataReceivedEventArgs args)
        {
            if (this.WriteOutputAndErrorToConsole)
            {
                Console.Error.WriteLine(args.Data);
            }
            if (this.CacheOutputs)
            {
                this.standardError.Add(args.Data);
            }
            ProcessOutputLine procOutput = this.ProcessOutput;
            if (!ReferenceEquals(procOutput, null))
            {
                procOutput(StandardLineType.StdErr, args.Data);
            }
        }
    }
}
