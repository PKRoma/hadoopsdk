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
using Microsoft.Hadoop.MapReduce;

namespace Microsoft.Hadoop.MapReduce.Test.DevUnit
{
    [TestClass]
    public class LocalWordCount_Tests
    {
        /// <summary>
        /// Scenario test: using local Executor to run word-count and verify output.
        /// 
        /// This is also what a user would likely do to validate their map/reduce program.
        /// </summary>
        [TestMethod]
        [TestCategory("CheckIn")]
        public void LocalWordCount_1()
        {
            string[] input = new [] { "a", "a", "b", "c", "a" };
            StreamingUnitOutput output = StreamingUnit.Execute<WordCountMapper, WordCountReducer, WordCountReducer>(input);
            string[] expected = new [] { "a\t3", "b\t1", "c\t1" };
            Assert.IsTrue(output.Result.SequenceEqual(expected), "output doesn't match expected");
        }


        public class WordCountMapper : MapperBase
        {
            public override void Map(string inputLine, MapperContext context)
            {
                foreach (string word in inputLine.Trim().Split(' '))
                {
                    context.EmitKeyValue(word, "1");
                    context.IncrementCounter("test");
                }
            }
        }

        public class WordCountReducer : ReducerCombinerBase
        {
            public override void Reduce(string key, IEnumerable<string> values, ReducerCombinerContext context)
            {
                string sum = values.Sum(s => long.Parse(s)).ToString();
                context.EmitKeyValue(key, sum);
            }
        }
    }
}
