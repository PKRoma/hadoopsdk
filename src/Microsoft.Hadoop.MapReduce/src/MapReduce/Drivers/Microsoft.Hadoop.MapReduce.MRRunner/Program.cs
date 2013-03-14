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


namespace Microsoft.HdInsight.MRRunner
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Microsoft.Hadoop.MapReduce;
    using System.IO;
    using System.Security.Policy;

    /// <summary>
    /// An EXE that can load and run a StreamingJob from an external assembly.
    /// </summary>
    public static class Runner
    {
        private static string _className;
        private static string _assemblyName;

        private static List<string> _additionalArgs;

        /// <summary>
        /// Main.
        /// </summary>
        /// <param name="args">Command-line arguments.</param>
        public static void Main(string[] args)
        {
            try
            {
                int last = 0;
                int curr = 0;

                if (args.Length == 0)
                {
                    Usage();
                    Environment.Exit(1);
                }

                while (curr < args.Length)
                {
                    if (curr < args.Length && args[curr] == "-?" || args[curr].ToLower() == "--help")
                    {
                        Usage();
                        Environment.Exit(1);
                    }

                    if (curr + 1 < args.Length && args[curr].ToLower() == "-dll")
                    {
                        curr++;
                        string path = args[curr];
                        try
                        {
                            _assemblyName = Path.GetFullPath(path);
                        }
                        catch (Exception ex)
                        {
                            ReportError("Cannot determine complete path to DLL. " + ex.Message);
                            Environment.Exit(1);
                        }
                        curr++;
                    }

                    if (curr + 1 < args.Length && args[curr].ToLower() == "-class")
                    {
                        curr++;
                        _className = args[curr];
                        curr++;
                    }

                    if (curr < args.Length && args[curr] == "--")
                    {
                        curr++;
                        _additionalArgs = new List<string>();
                        for (; curr < args.Length;curr++)
                        {
                            _additionalArgs.Add(args[curr]);
                        }
                    }

                    if (curr == last)
                    {
                        ReportError("Unknown option " + args[curr] + ".");
                        Usage();
                        Environment.Exit(1);
                    }
                    last = curr;
                }

                if (_assemblyName == null)
                {
                    ReportError("Dll not set.  Use {-dll DLL}.");
                    Environment.Exit(1);
                }

                Run();

                Environment.Exit(0);
            }
            catch (Exception e)
            {
                ReportError("Exception: " + e.Message);
                Environment.Exit(1);
            }
        }

        //
        private static void Run()
        {
            Assembly asm = null;
            try
            {
                asm = Assembly.LoadFrom(_assemblyName);
            }
            catch (Exception e)
            {
                ReportError("DLL cannot be loaded: " + e.Message);
                Environment.Exit(1);
            }
            if (asm == null)
            {
                ReportError("DLL cannot be loaded.");
                Environment.Exit(1);
            }

            Type jobType = null;
            if (_className != null)
            {
                try
                {
                    jobType = asm.GetType(_className);
                }
                catch (Exception ex)
                {
                    ReportError("Class could not be loaded: " + ex.Message);
                    Environment.Exit(1);
                }

                if (jobType == null)
                {
                    ReportError("Class not found in assembly DLL. Use namespace-qualified type name or full Assembly-qualified type name.");
                    Environment.Exit(1);
                }
            }
            else
            {
                // search for appropriate type
                Type baseClass = typeof(HadoopJob);
                Type[] types = asm.GetTypes();
                Type[] matching = types.Where(type => baseClass.IsAssignableFrom(type)).ToArray();

                if (matching.Length == 0)
                {
                    ReportError("No classes in DLL derive from MapReduceJob.");
                    Environment.Exit(1);
                }

                if (matching.Length > 1)
                {
                    ReportError("More than one class in DLL derives from MapReduceJob. Specify the intended class explicitly.");
                    Environment.Exit(1);
                }

                jobType = matching[0];
            }

            // try creating an instance so that we can fast-fail in case of trouble.
            try
            {
                HadoopJob dummy = (HadoopJob)Activator.CreateInstance(jobType);
            }
            catch (Exception ex)
            {
                ReportError(string.Format("Could not create an instance of the map-reduce class. Class={0}. Msg={1}.", jobType.AssemblyQualifiedName, ex.Message));
                Environment.Exit(1);
            }
            
            try
            {
                // at this point we just want to call HadoopJobExecutor.Execute(job);
                // unfortunately it causes the dll dependency analysis to break as the Assembly.CodeBase is like not the folder that contains the users dll
                // The most robust solution is to create a new AppDomain and launch .Execute() from there.
                // Given that the users StreamingJob class shouldn't have to be tagged [serializable], we pass the type name and create a fresh object over the fence.

                string requesterPath = Path.GetDirectoryName(_assemblyName);
                AppDomainSetup setup = new AppDomainSetup();
                setup.ApplicationBase = requesterPath;
                Evidence ev = new Evidence(AppDomain.CurrentDomain.Evidence);
                AppDomain userDllDomain = userDllDomain = AppDomain.CreateDomain("UserAssemblyDomain", ev, setup);
                ExecutionCaller caller = (ExecutionCaller)userDllDomain.CreateInstanceFromAndUnwrap(Assembly.GetExecutingAssembly().Location, typeof(ExecutionCaller).FullName);
                
                string[] args = null;
                if(_additionalArgs != null){
                    args = _additionalArgs.ToArray();
                }
                caller.CallExecute(jobType, args);
            }
            catch (Exception ex)
            {
                ReportError("Error during job execution: " + ex.Message);
                Environment.Exit(1);
            }
        }

        private static void ReportError(string msg)
        {
            Console.WriteLine("ERROR: " + msg);
        }

        private static void Usage()
        {
            Console.WriteLine(
                "MRRunner.exe -dll DllPath [-class ClassName] [-- job_args]" + Environment.NewLine + 
                "  DllPath:   path to assembly DLL containing a MapReduceJob class" + Environment.NewLine + 
                "  ClassName: Either simple name or Assembly-qualified class name, " + Environment.NewLine + 
                "             eg MyMapReduceJob" + Environment.NewLine + 
                "                \"MyMapReduceJob, DllName, Version=.., Culture=.., PublicKeyToken=..\"" + Environment.NewLine +
                "             If className is not specified, Dll is searched for a MapReduceJob class" + Environment.NewLine +
                "  job-args:  all args after -- are passed to M/R job via ExecutionContext.Arguments" + Environment.NewLine +
                ""
            );
        }
    }
}
