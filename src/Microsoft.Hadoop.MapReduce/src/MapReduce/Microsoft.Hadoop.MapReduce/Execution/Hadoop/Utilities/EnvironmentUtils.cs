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
using System.IO;
using System.Reflection;
using System.Xml.Linq;

namespace Microsoft.Hadoop.MapReduce
{
    internal static class EnvironmentUtils
    {
        internal const string EnvVarName_Hadoop_Home = "HADOOP_HOME";
        internal const string EnvVarName_Java_Home = "Java_HOME";
        internal const string EnvVarName_Class_Path = "HADOOP_HOME";

        internal const string HadoopExecutableName = "hadoop.cmd";
        internal const string HadoopStreamingJarName = "hadoop-streaming.jar";
        internal const string MapDriverExeName = "Microsoft.Hadoop.MapDriver.exe";
        internal const string ReduceDriverExeName = "Microsoft.Hadoop.ReduceDriver.exe";
        internal const string CombineDriverExeName = "Microsoft.Hadoop.CombineDriver.exe";
        internal const string ThreadingHelperDllName = "Microsoft.WindowsAzure.Management.Framework.Threading.dll";

        internal const string CmdLine_Stream_Num_Map_Output_Key_Fields = "-D stream.num.map.output.key.fields={0}"; // this is how to specify it on the hadoop cmd-line.
        internal const string EnvVar_Stream_Num_Map_Output_Key_Fields = "stream_num_map_output_key_fields"; // this is the matching env-var available to driver.exes

        internal const string EnvVarName_User_MapperDLLName = "MSFT_HADOOP_MAPPER_DLL"; 
        internal const string EnvVarName_User_MapperTypeName = "MSFT_HADOOP_MAPPER_TYPE";

        internal const string EnvVarName_User_ReducerDLLName = "MSFT_HADOOP_REDUCER_DLL";
        internal const string EnvVarName_User_ReducerTypeName = "MSFT_HADOOP_REDUCER_TYPE";

        internal const string EnvVarName_User_CombinerDLLName = "MSFT_HADOOP_COMBINER_DLL";
        internal const string EnvVarName_User_CombinerTypeName = "MSFT_HADOOP_COMBINER_TYPE";
        private static string s_mrlibFolder;

        internal static string TaskNode_PathToShippedResource(string fileName)
        {
            return fileName;
        }

        internal static string PathToHadoopExe
        {
            get { return MakeHadoopBinPath(HadoopExecutableName); }
        }

        internal static string HadoopHome
        {
            get
            {
                string hh = Environment.GetEnvironmentVariable(EnvVarName_Hadoop_Home);
                hh = hh.Replace("\"", "");
                return hh;
            }
        }
        
        internal static string PathToStreamingJar {
            get { return Path.Combine(Path.Combine(HadoopHome, "lib"), HadoopStreamingJarName); }
        }

        internal static string MRLibPath
        {
            get 
            {
                if (s_mrlibFolder != null)
                    return s_mrlibFolder;

                string userDLLPath = Path.GetDirectoryName(Path.GetFullPath((new System.Uri(Assembly.GetExecutingAssembly().CodeBase)).AbsolutePath));
                string mrLibEnvPath = Environment.GetEnvironmentVariable("MRLib") ?? "MRLib environment var not set";
                
                string[] paths = {
                                    Environment.CurrentDirectory,
                                    userDLLPath,
                                    Path.Combine(userDLLPath, "MRLib"),
                                    mrLibEnvPath
                                 };
                foreach (string path in paths)
                {
                    if (File.Exists(Path.Combine(path, MapDriverExeName)))
                    {
                        s_mrlibFolder = path;
                        return s_mrlibFolder;
                    }
                }
                throw new StreamingException("Could not locate the MRLib tools (MapDriver.exe, etc). \r\n Paths tested: " + Environment.NewLine + string.Join(Environment.NewLine, paths));
            }
        }
        
        internal static string PathToMapDriverExe
        {
            get { 
                return MakeMRLibPath(MapDriverExeName);
            }
        }

        internal static string PathToReduceDriverExe
        {
            get {
                return MakeMRLibPath(ReduceDriverExeName);
            }
        }

        internal static string PathToCombineDriverExe
        {
            get {
                return MakeMRLibPath(CombineDriverExeName);
            }
        }

        internal static string PathToThreadingHelperDll
        {
            get { return MakeMRLibPath(ThreadingHelperDllName); }
        }


        internal static string[] GetPathToClientAssemblies()
        {
            //walk stack looking for a different DLL.
            return TypeSystem.GetLoadedNonSystemAssemblyPaths().ToArray();
        }


        private static string MakeHadoopBinPath(string fileName){
            string path = Path.Combine(HadoopHome, "bin", fileName);
            return path;
        }

        private static string MakeMRLibPath(string fileName)
        {
            return Path.Combine(MRLibPath, fileName);
        }

        internal static string GetDefaultHadoopFileSystem()
        {
            if (HadoopHome == null)
            {
                return null;
            }
            string coreSitePath = Path.Combine(HadoopHome, "conf", "core-site.xml");
            if (!File.Exists(coreSitePath))
            {
                return null;
            }
            XDocument coreSiteXml = XDocument.Load(coreSitePath);
            XElement fsProperty = coreSiteXml.Root.Elements("property").SingleOrDefault(p => p.Element("name").Value == "fs.default.name");
            if (fsProperty == null)
            {
                return null;
            }
            return fsProperty.Element("value").Value;
        }

        internal static void CheckHadoopEnvironment()
        {
            bool error = false;
            StringBuilder errors = new StringBuilder();
            foreach (string varName in (new string[] { EnvVarName_Hadoop_Home, EnvVarName_Java_Home, EnvVarName_Class_Path }))
            {
                string var = Environment.GetEnvironmentVariable(varName);
                if (string.IsNullOrEmpty(var))
                {
                    error = true;
                    errors.AppendLine("Environment variable not set: " + varName);
                }
            }

            

            string dummy;
            
            // check whether the driver EXEs can be located.
            try
            {
                
                dummy = PathToMapDriverExe;
            }
            catch (StreamingException ex)
            {
                error = true;
                errors.AppendLine(ex.Message);
            }

            try
            {

                dummy = PathToCombineDriverExe;
            }
            catch (StreamingException ex)
            {
                error = true;
                errors.AppendLine(ex.Message);
            }

            try
            {
                dummy = PathToReduceDriverExe;
            }
            catch (StreamingException ex)
            {
                error = true;
                errors.AppendLine(ex.Message);
            }

            if (error)
            {
                string msg = errors.ToString();
                Logger.WriteLine(msg);
                throw new StreamingException("The environment is not suitable:" + Environment.NewLine + msg);
            }
        }


        internal static List<string> BuildFileIncludeList(List<string> includes, List<string> excludes)
        {
            List<string> files = new List<string>();
            files.AddRange(GetPathToClientAssemblies());
            foreach (string file in includes)
            {
                if (files.IndexOf(file) == -1)
                {
                    files.Add(file);
                }
            }

            foreach (string file in excludes)
            {
                files.RemoveAll(s=>s.Contains(file));
            }

            Logger.WriteLine("File dependencies to include with job:");
            foreach (string f in files)
            {
                if (includes.Contains(f))
                {
                    Logger.Write("[Manual]        ");
                }
                else
                {
                    Logger.Write("[Auto-detected] ");
                }
                Logger.WriteLine(string.Format("{0}", f));
            }


            return files;
        }
    }
}
