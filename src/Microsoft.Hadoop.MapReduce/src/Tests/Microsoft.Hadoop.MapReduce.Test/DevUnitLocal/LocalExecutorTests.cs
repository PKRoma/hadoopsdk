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
    public class LocalExecutorTests
    {
        [TestMethod]
        [ExpectedException(typeof(StreamingException))]
        [TestCategory("CheckIn")]
        public void PrivateMapperShouldFail()
        {
            StreamingUnit.Execute<PrivateMapper>(new string[] { "a" });
        }

        [TestMethod]
        [ExpectedException(typeof(StreamingException))]
        [TestCategory("CheckIn")]
        public void PrivateCombinerShouldFail()
        {
            StreamingUnit.Execute<GoodMapper, PrivateReducerCombiner, GoodReducerCombiner>(new string[] { "a" });
        }

        [TestMethod]
        [ExpectedException(typeof(StreamingException))]
        [TestCategory("CheckIn")]
        public void PrivateReducerShouldFail()
        {
            StreamingUnit.Execute<GoodMapper, PrivateReducerCombiner>(new string[] { "a" });
        }

        private class PrivateMapper : MapperBase
        {
            public override void Map(string inputLine, MapperContext context)
            {
            }
        }

        internal class PrivateReducerCombiner : ReducerCombinerBase
        {
            public override void Reduce(string key, IEnumerable<string> values, ReducerCombinerContext context)
            {
                
            }
        }

        public class GoodMapper : MapperBase
        {
            public override void Map(string inputLine, MapperContext context)
            {

            }
        }
        
        public class GoodReducerCombiner : ReducerCombinerBase
        {
            public override void Reduce(string key, IEnumerable<string> values, ReducerCombinerContext context)
            {
                
            }
        }
    }
}
