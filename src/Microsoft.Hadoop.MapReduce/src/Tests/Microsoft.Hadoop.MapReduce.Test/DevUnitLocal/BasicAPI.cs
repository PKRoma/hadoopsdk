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
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Hadoop.MapReduce.Test.DevUnitLocal
{
    [TestClass]
    public class BasicAPI
    {
        [TestMethod]
        [ExpectedException(typeof(StreamingException))]
        [TestCategory("CheckIn")]
        public void UntypedJobTest()
        {
            Hadoop.Connect().MapReduceJob.ExecuteJob<UntypedJob>();
        }

        public class UntypedJob : HadoopJob
        {
            public override HadoopJobConfiguration Configure(ExecutorContext context)
            {
                return new HadoopJobConfiguration();
            }
        }

        [TestMethod]
        public void MakeCommandLine()
        {
//            string cmdLine = HadoopJobExecutor.MakeCommandLine<SimpleJob>();
//            Assert.IsTrue(!string.IsNullOrEmpty(cmdLine), "cmdLine generation failed");
        }

        public class SimpleJob : HadoopJob<SimpleMapper, SimpleReducer>
        {
            public override HadoopJobConfiguration Configure(ExecutorContext context)
            {
                return new HadoopJobConfiguration()
                {
                    InputPath = "dummyPath"
                };
            }
        }

        public class SimpleMapper : MapperBase
        {
            public override void Map(string inputLine, MapperContext context)
            {
                throw new NotImplementedException();
            }
        }
        public class SimpleReducer : ReducerCombinerBase
        {

            public override void Reduce(string key, IEnumerable<string> values, ReducerCombinerContext context)
            {
                throw new NotImplementedException();
            }
        }

    }

}
