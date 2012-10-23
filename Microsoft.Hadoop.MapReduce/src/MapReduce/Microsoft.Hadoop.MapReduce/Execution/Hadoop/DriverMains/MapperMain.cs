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
using System.Diagnostics;
using System.Reflection;

namespace Microsoft.Hadoop.MapReduce
{
    /// <summary>
    /// The main class for Mapper driver EXE
    /// </summary>
    public class MapperMain
    {
        const string User_Streaming_DLL_Dir = @"..\..\jars\"; //keep in sync

        private static string UserDLLPath
        {
            get
            {
                string userDLL = Environment.GetEnvironmentVariable(EnvironmentUtils.EnvVarName_User_MapperDLLName);
                if (string.IsNullOrEmpty(userDLL))
                {
                    throw new StreamingException(string.Format("EnvVar {0} is not set", EnvironmentUtils.EnvVarName_User_MapperDLLName));
                }

                if (File.Exists(userDLL)) // supports localExecutor.
                {
                    return userDLL; 
                }
                else // supports hadoop streaming executor.
                {
                    return Path.GetFullPath(EnvironmentUtils.TaskNode_PathToShippedResource(userDLL));
                }
            }
        }

        private static string MapperTypeName
        {
            get
            {
                string mapType = Environment.GetEnvironmentVariable(EnvironmentUtils.EnvVarName_User_MapperTypeName);
                if (string.IsNullOrEmpty(mapType))
                {
                    throw new StreamingException(string.Format("EnvVar {0} is not set", EnvironmentUtils.EnvVarName_User_MapperTypeName));
                }
                return mapType;
            }
        }

        /// <summary>
        /// Main function for Mapper driver EXEs.
        /// </summary>
        public static void Main()
        {
            // WARNING: If launch is used, you _must_ attach else the program will bail out immediately.  Use MsgBox.Show or similar for ignorable break.
            //         --> this is due to a bug in .NET4 framework. https://connect.microsoft.com/VisualStudio/feedback/details/611486/debugger-launch-is-now-crashing-my-net-application-after-upgrading-to-net-4-0
            //Debugger.Launch();

            string tmp = UserDLLPath;
            
            //@@TODO: something weird is going on causing this to erroneously not pass with local executor
            //if (!File.Exists(tmp))
            //{
            //    throw new StreamingException("User DLL not found.  Expected location = " + UserDLLPath);
            //}

            Assembly userAssembly;
            Type mapperType;

            try
            {
                userAssembly = Assembly.LoadFrom(UserDLLPath);
                mapperType = userAssembly.GetType(MapperTypeName);
            }
            catch (Exception ex)
            {
                throw new StreamingException(string.Format("The user type could not be loaded. DLL={0}, Type={1}", UserDLLPath, MapperTypeName), ex);
            }
            
            MapperBase mapper = (MapperBase)Activator.CreateInstance(mapperType);    
            if (mapperType == null)
            {
                throw new StreamingException(string.Format("The user type could not be loaded. DLL={0}, Type={1}", UserDLLPath, MapperTypeName));
            }

            HadoopMapperContext context = new HadoopMapperContext();            
            Process(mapper, new StdinEnumerable(), context);
        }

        internal static void Process(MapperBase mapper, IEnumerable<string> inputEnumerable, MapperContext context)
        {
            mapper.Initialize(context);
            foreach(string line in inputEnumerable)
            {
                mapper.Map(line, context);
            }
            mapper.Cleanup(context);
        }
    }
}
