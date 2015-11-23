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
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace Microsoft.Hadoop.MapReduce
{
    /// <summary>
    /// The main class for Reducer and Combiner driver EXEs
    /// </summary>
    public class ReducerCombinerMain
    {
        
        private static string ReducerDLLPath
        {
            get
            {
                string reducerDll = Environment.GetEnvironmentVariable(EnvironmentUtils.EnvVarName_User_ReducerDLLName);
                if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable(EnvironmentUtils.EnvVarName_User_ReducerDLLName)))
                {
                    throw new StreamingException(string.Format("EnvVar {0} is not set", EnvironmentUtils.EnvVarName_User_ReducerDLLName));
                }

                if (File.Exists(reducerDll)) // supports localExecutor.
                {
                    return reducerDll;
                }
                else
                {
                    return Path.GetFullPath(EnvironmentUtils.TaskNode_PathToShippedResource(reducerDll));
                }
            }
        }

        private static string ReducerTypeName
        {
            get
            {
                if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable(EnvironmentUtils.EnvVarName_User_ReducerTypeName)))
                {
                    throw new StreamingException(string.Format("EnvVar {0} is not set", EnvironmentUtils.EnvVarName_User_ReducerTypeName));
                }
                return Environment.GetEnvironmentVariable(EnvironmentUtils.EnvVarName_User_ReducerTypeName);
            }
        }

        private static string CombinerDLLPath
        {
            get
            {
                string combinerDll = Environment.GetEnvironmentVariable(EnvironmentUtils.EnvVarName_User_CombinerDLLName);
                if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable(EnvironmentUtils.EnvVarName_User_CombinerDLLName)))
                {
                    throw new StreamingException(string.Format("EnvVar {0} is not set", EnvironmentUtils.EnvVarName_User_CombinerDLLName));
                }
                if (File.Exists(combinerDll))
                {
                    return combinerDll;
                }
                else
                {
                    return Path.GetFullPath(EnvironmentUtils.TaskNode_PathToShippedResource(combinerDll));
                }
            }
        }

        private static string CombinerTypeName
        {
            get
            {
                if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable(EnvironmentUtils.EnvVarName_User_CombinerTypeName)))
                {
                    throw new StreamingException(string.Format("EnvVar {0} is not set", EnvironmentUtils.EnvVarName_User_CombinerTypeName));
                }
                return Environment.GetEnvironmentVariable(EnvironmentUtils.EnvVarName_User_CombinerTypeName);
            }
        }

       
        /// <summary>
        /// Main function for Reducer and Combiner driver EXEs.
        /// </summary>
        /// <param name="isCombiner">if set to <c>true</c> [is combiner].</param>
        public static void CombineReduceMain(bool isCombiner) //@@todo - remove unused bool.
        {
            // WARNING: If launch is used, you _must_ attach else the program will bail out immediately.  Use MsgBox.Show or similar for ignorable break.
            //         --> this is due to a bug in .NET4 framework. https://connect.microsoft.com/VisualStudio/feedback/details/611486/debugger-launch-is-now-crashing-my-net-application-after-upgrading-to-net-4-0
            //Debugger.Launch(); 

            // Check if to set the encoding to UTF8
            string encoding = Environment.GetEnvironmentVariable(EnvironmentUtils.EnvVarName_User_Encoding);
            if (encoding != null && encoding.ToLowerInvariant().Equals("utf8", StringComparison.OrdinalIgnoreCase))
            {
                Console.InputEncoding = Encoding.UTF8;
                Console.OutputEncoding = Encoding.UTF8;
            }

            // read relevant environment variables
            string env_outputKeyFields = Environment.GetEnvironmentVariable(EnvironmentUtils.EnvVar_Stream_Num_Map_Output_Key_Fields);
            int numKeyFields = 1;
            if (!string.IsNullOrEmpty(env_outputKeyFields))
            {
                numKeyFields = int.Parse(env_outputKeyFields);
            }


            if (isCombiner) //@@TODO: remove dual path
            {
                if (!File.Exists(CombinerDLLPath))
                {
                    throw new StreamingException("User DLL not found.  Expected location = " + CombinerDLLPath);
                }

                Assembly combinerAssembly;
                Type combinerType;
                object combinerObj;
                try
                {
                    combinerAssembly = Assembly.LoadFrom(CombinerDLLPath);
                    combinerType = combinerAssembly.GetType(CombinerTypeName);
                    combinerObj = Activator.CreateInstance(combinerType);
                }
                catch (Exception ex)
                {
                    throw new StreamingException(string.Format("Could not load combiner type. See inner exception. DLL={0}, Type={1}", CombinerDLLPath, CombinerTypeName), ex);
                }
                
                if (!(combinerObj is ReducerCombinerBase)){
                    throw new StreamingException(string.Format("The combiner type must inherit from ReducerCombinerBase. DLL={0}, Type={1}", CombinerDLLPath, CombinerTypeName));
                }

                HadoopReducerCombinerContext context = new HadoopReducerCombinerContext(true);
                Process((ReducerCombinerBase)combinerObj, new StdinEnumerable(), numKeyFields, context);
            }
            else
            {
                if (!File.Exists(ReducerDLLPath))
                {
                    throw new StreamingException("User DLL not found.  Expected location = " + ReducerDLLPath);
                }

                Assembly reducerAssembly;
                Type reducerType;

                try
                {
                    reducerAssembly = Assembly.LoadFrom(ReducerDLLPath);
                    reducerType = reducerAssembly.GetType(ReducerTypeName);
                }
                catch (Exception ex)
                {
                    throw new StreamingException(string.Format("The user type could not be loaded. DLL={0}, Type={1}", ReducerDLLPath, ReducerTypeName), ex);
                }

                ReducerCombinerBase reducer = (ReducerCombinerBase)Activator.CreateInstance(reducerType);
                if (reducer == null)
                {
                    throw new StreamingException(string.Format("The reduce type must inherit from ReducerBase. DLL={0}, Type={1}", CombinerDLLPath, CombinerTypeName));
                }

                HadoopReducerCombinerContext context = new HadoopReducerCombinerContext(false);
                Process(reducer, new StdinEnumerable(), numKeyFields, context);
            }
        }

        internal static void Process(ReducerCombinerBase reducer, IEnumerable<string> inputEnumerable, int numKeyFields, ReducerCombinerContext context)
        {
            reducer.Initialize(context);

            
            Grouper grouper = new Grouper(numKeyFields, inputEnumerable);

            IGrouping<string, string> group;
            while ((group = grouper.NextGroup()) != null)
            {
                reducer.Reduce(group.Key, group, context);
            }

            reducer.Cleanup(context);
        }
    }
}
