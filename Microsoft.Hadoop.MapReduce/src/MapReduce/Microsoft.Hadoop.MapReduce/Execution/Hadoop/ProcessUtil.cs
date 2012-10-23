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
    internal static class ProcessUtil
    {
        
        
        //internal static void RunHadoopCommand(string hadoopArgs)
        //{
        //    RunCommand_ThrowOnError("hadoop", hadoopArgs);
        //}

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
