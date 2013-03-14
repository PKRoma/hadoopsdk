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


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Security;

namespace Microsoft.Hadoop.MapReduce
{
    using Microsoft.Hadoop.MapReduce.Execution.Hadoop;

    internal class ProcessUtil : IProcessUtil
    {
        private bool foundFailureString;

        private ProcessUtil()
        {
            this.foundFailureString = false;
        }

        internal static IProcessUtil Create()
        {
            return new ProcessUtil();
        }

        Task<int> IProcessUtil.RunCommand(IProcessExecutor executor)
        {
            return executor.Execute();
        }
        
        async Task<int> IProcessUtil.RunAndThrowOnError(IProcessExecutor executor)
        {
            int result = await((IProcessUtil)this).RunCommand(executor);
            if (result != 0 || this.foundFailureString)
            {
                throw new StreamingException(this.GenerateErrorMessage(this.foundFailureString, 
                                                                       executor.Command, 
                                                                       executor.CreateArgumentString(), 
                                                                       result));
            }
            return result;
        }

        private string GenerateErrorMessage(bool foundErrorString, string command, string args, int exitCode)
        {
            return string.Format("Process failed ({0}). For hadoop job failure, see job-tracker portal for error information and logs. Cmd = {1} {2}. ExitCode={3}",
                                 foundErrorString ? "'Streaming Job Failed!' message" : "non-zero exit code",
                                 command,
                                 args,
                                 exitCode);
        }

        internal void ProcessOutputLine(StandardLineType lineType, string output)
        {
            if (lineType == StandardLineType.StdErr && !string.IsNullOrEmpty(output))
            {
                if (output.Contains("Streaming Job Failed!") || output.Contains("Streaming Command Failed!") || output.Contains("Exception in thread"))
                {
                    this.foundFailureString = true;
                }
            }
        }

        internal static int RunHadoopCommand_ThrowOnError(IProcessExecutor executor)
        {
            Task<int> result = ProcessUtil.Create().RunAndThrowOnError(executor);
            result.Wait();
            return result.Result;
        }

        internal static int RunHadoopCommand(IProcessExecutor executor)
        {
            Task<int> result = ProcessUtil.Create().RunCommand(executor);
            result.Wait();
            return result.Result;
        }

        internal static void RunHadoopCommand_ThrowOnError(string cmd, string args)
        {
            RunHadoopCommand_ThrowOnError(cmd, args, true);
        }
        
        internal static void RunHadoopCommand_ThrowOnError(string cmd, string args, bool teeOutputToConsole)
        {
            string stdout;
            string stderr;
            int exitCode;
            RunCommand(cmd, args, out exitCode, out stdout, out stderr, teeOutputToConsole);
            if (exitCode != 0)
            {
                throw new StreamingException(string.Format("Process failed (non-zero exit code). For hadoop job failure, see job-tracker portal for error information and logs. Cmd = {0} {1}. ExitCode={2}", cmd, args, exitCode));
            }
            if (stderr.Contains("Streaming Job Failed!") || stderr.Contains("Streaming Command Failed!"))
            {
                throw new StreamingException(string.Format("Process failed ('Streaming Job Failed!' message'). See job-tracker portal for error information and logs."));
            }
        }

        internal static void RunCommand(string cmd, string args, out int exitCode, out string stdout, out string stderr)
        {
            RunCommand(cmd, args, out exitCode, out stdout, out stderr, true);
        }

        internal static void RunCommand(string cmd, string args, out int exitCode, out string stdout, out string stderr, bool teeOutputToConsole)
        {
            Process p = new Process();
            p.StartInfo = new ProcessStartInfo {
                FileName = cmd,
                Arguments = args,
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true, // if done, the output must be consumed before p.WaitForExit occurs.
                RedirectStandardError = true,
            };
            
            
            
            p.Start();
            StringBuilder stdoutBuilder = new StringBuilder();
            StringBuilder stderrBuilder = new StringBuilder();
            //read the stdout/stderr and pass them through to this program's console.
            Task t1 = Task.Factory.StartNew(() =>
            {
                string line;
                while ((line = p.StandardOutput.ReadLine()) != null)
                {
                    if (teeOutputToConsole)
                    {
                        Console.WriteLine(line); // tee to this process stdout
                    }
                    stdoutBuilder.AppendLine(line);
                }
            });

            Task t2 = Task.Factory.StartNew(() =>
            {
                string line;
                while ((line = p.StandardError.ReadLine()) != null)
                {
                    if (teeOutputToConsole)
                    {
                        Console.Error.WriteLine(line); // tee to this process stderr
                    }
                    stderrBuilder.AppendLine(line);
                }
            });

            
            Task.WaitAll(t1, t2);
            
            stdout = stdoutBuilder.ToString();
            stderr = stderrBuilder.ToString();
            p.WaitForExit();
            exitCode = p.ExitCode;
        }

        internal static void RunMRCommand(string command, string args, string stdOutFile, string stdErrFile)
        {
            //@@TODO: the stdout is getting read entirely into memory.. better to stream it.
            string stdout;
            string stderr;
            int exitCode;
            RunCommand(command, args, out exitCode, out stdout, out stderr);
            if (exitCode != 0)
            {
                throw new StreamingException(string.Format("External process failed (non-zero exit code). Cmd = {0} {1}. ExitCode={2}", command, args, exitCode));
            }

            File.AppendAllText(stdOutFile, stdout);
            File.AppendAllText(stdErrFile, stderr);

        }
    }
}
