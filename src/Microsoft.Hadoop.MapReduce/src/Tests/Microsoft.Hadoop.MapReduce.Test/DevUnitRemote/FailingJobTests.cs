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

namespace Microsoft.Hadoop.MapReduce.Test.DevUnitRemote
{
    [TestClass]
    public class FailingJobTests : IntegrationTestBase
    {
        [TestMethod]
        [ExpectedException(typeof(StreamingException))]
        [TestCategory("CheckIn")]
        [TestCategory("Integration")]
        public void JobFailureMashalTest()
        {
            string inputPath = "hadoopAPIUnitTest/input/JobFailureMashalTest/file.txt";
            string outputFolder = "hadoopAPIUnitTest/output/JobFailureMashalTest";
            var hadoop = Hadoop.Connect();
            hadoop.StorageSystem.WriteAllText(inputPath, "dummy text");
            hadoop.MapReduceJob.Execute<BadMapper>(new HadoopJobConfiguration { InputPath = inputPath, OutputFolder = outputFolder });
        }

        public class BadMapper : MapperBase
        {
            public override void Map(string inputLine, MapperContext context)
            {
                throw new NotImplementedException();
            }
        }
    }
}
