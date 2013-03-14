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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Hadoop.MapReduce;
using System.Diagnostics;

namespace Microsoft.Hadoop.MapReduce.Test
{
    /// <summary>
    /// Tests that figure out how we handle the different encodings.
    /// </summary>
    [TestClass]
    public class DifferentEncodings
    {
        private void TestEncoding(Encoding encoding)
        {
            var hadoop = Hadoop.Connect();
            string folder = "/streamingUnitTests/DifferentEncodings/" + encoding.WebName;
            if (hadoop.StorageSystem.Exists(folder))
            {
                hadoop.StorageSystem.Delete(folder);
            }
            const string input =
@"PureFoo
PureFoo
Bar";
            hadoop.StorageSystem.WriteAllBytes(folder + "/in", encoding.GetBytes(input));
            Hadoop.Connect().MapReduceJob.Execute<Mapper,Reducer>(MakeConfig(folder));
            var output = hadoop.StorageSystem.EnumerateDataInFolder(folder + "/out", maxLines: 5000).OrderBy(s => s).ToList();
            Trace.WriteLine("Output:");
            output.ForEach(s => Trace.WriteLine(s));
            Assert.AreEqual(2, output.Count);
            Assert.AreEqual("Bar\tN", output[0]);
            Assert.AreEqual("PureFoo\tYY", output[1]);
        }

        public static HadoopJobConfiguration MakeConfig(string folder)
        {
            return new HadoopJobConfiguration()
            {
                InputPath = folder + "/in",
                OutputFolder = folder + "/out",
            };
        }

        [TestMethod]
        public void Utf8Test()
        {
            TestEncoding(Encoding.UTF8);
        }

        [TestMethod]
        [Ignore] // This fails because of issue: https://github.com/mwinkle/hadoop-net-sdk/issues/45
        public void UnicodeTest()
        {
            TestEncoding(Encoding.Unicode);
        }

        public class Mapper : MapperBase
        {
            public override void Map(string inputLine, MapperContext context)
            {
                context.EmitKeyValue(inputLine, inputLine == "PureFoo" ? "Y" : "N");
            }
        }

        public class Reducer : ReducerCombinerBase
        {
            public override void Reduce(string key, IEnumerable<string> values, ReducerCombinerContext context)
            {
                context.EmitKeyValue(key, String.Join("", values));
            }
        }
    }
}
