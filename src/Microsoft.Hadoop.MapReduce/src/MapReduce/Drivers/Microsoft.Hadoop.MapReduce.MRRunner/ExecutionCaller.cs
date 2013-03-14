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
    using Microsoft.Hadoop.MapReduce;

    /// <summary>
    /// Helper class that can run in the context of a new AppDomain and invoke HadoopJobExecutor.
    /// </summary>
    internal class ExecutionCaller : MarshalByRefObject
    {
        public ExecutionCaller()
        {
        }

        public void CallExecute(Type userType, string[] args)
        {
            Hadoop.Connect().MapReduceJob.ExecuteJob(userType, args);
            //MethodInfo mi = typeof(HadoopJobExecutor_Old).GetMethod("ExecuteJob", BindingFlags.Public | BindingFlags.Static, null, new Type[] {typeof(string[])}, null ).MakeGenericMethod(userType);
            //mi.Invoke(null, new object[] {args});
        }
    }
}
