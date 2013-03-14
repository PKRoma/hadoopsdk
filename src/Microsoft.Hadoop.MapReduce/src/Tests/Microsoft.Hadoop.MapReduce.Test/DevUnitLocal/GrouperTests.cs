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
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Distributed.DevUnitTests;

namespace Microsoft.Hadoop.MapReduce.Test.DevUnitLocal
{
    [TestClass]
    public class GrouperTests
    {
        [TestMethod]
        [TestCategory("CheckIn")]
        public void Grouper_1()
        {
            string[] input = new string[] { "a\t1", "a\t2", "b\t3" };
            dynamic grouper = ExposedObject.New(Type.GetType("Microsoft.Hadoop.MapReduce.Grouper, Microsoft.Hadoop.MapReduce"), new object[] { 1, input });
            IGrouping<string, string> g1 = grouper.NextGroup();
            Assert.IsTrue(g1.SequenceEqual(new[] { "1", "2" }));

            IGrouping<string, string> g2 = grouper.NextGroup();
            Assert.IsTrue(g2.SequenceEqual(new[] { "3" }));
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        public void Grouper_FastForward_1()
        {
            string[] input = new string[] { "a\t1", "a\t2", "b\t1" };
            dynamic grouper = ExposedObject.New(Type.GetType("Microsoft.Hadoop.MapReduce.Grouper, Microsoft.Hadoop.MapReduce"), new object[] { 1, input });
            IGrouping<string, string> group1 = grouper.NextGroup();
            // don't enumerate the first group at all
            IGrouping<string, string> group2 = grouper.NextGroup();
            Assert.IsTrue(group2.Key == "b", "the first group should fast-forward the input if it is not consumed");

        }

        [TestMethod]
        [TestCategory("CheckIn")]
        public void Grouper_FastForward_2()
        {
            string[] input = new string[] { "a\t1", "a\t2", "b\t1" };

            dynamic grouper = ExposedObject.New(Type.GetType("Microsoft.Hadoop.MapReduce.Grouper, Microsoft.Hadoop.MapReduce"), new object[] { 1, input });
            IGrouping<string, string> group1 = grouper.NextGroup();
            string g1_v1 = group1.First(); // partially enumerate the first group
            IGrouping<string, string> group2 = grouper.NextGroup();
            Assert.IsTrue(group2.Key == "b", "the first group should fast-forward the input if it is not consumed");
        }
    }
}
