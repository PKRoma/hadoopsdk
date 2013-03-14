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

using Microsoft.Hadoop.WebHDFS.Adapters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Hadoop.Hive;

namespace UnitTests
{
    
    // TODO - add local tests - put data in isolated database
    // var queryResults = db.ExecuteHiveQuery<HiveSampleTableRow>("select * from w3c LIMIT 10;").ToList();
    // add dynamic type tests

    [TestClass]
    class AzureIntegrationTests
    {

        [TestMethod]
        public void SimplePassThrough()
        {
            using (var db = new MyHiveDatabase(TestConfig.AzureHost, TestConfig.AzureUserName, TestConfig.AzurePassword, TestConfig.AzureStorageAccount, TestConfig.AzureStorageKey))
            {
                var resultsTask =
                    db.ExecuteHiveQuery<HiveSampleTableRow>(
                        "select * from HiveSampleTable where state = 'Oregon' LIMIT 10;");
                resultsTask.Wait();

                var results = resultsTask.Result.ToList();

                Assert.AreEqual(10, results.Count);
                Assert.AreEqual("Oregon", results[0].state);
            }
        }

        //[TestMethod]
        //public void Metadata()
        //{
        //    var db = new MyHiveDatabase(TestConfig.AzureHost, TestConfig.AzurePort, TestConfig.AzureUserName, TestConfig.AzurePassword);
        //    var metadata = db.GetMetaData();
        //    Assert.AreEqual(1, metadata.Keys.Count);
        //}

        [TestMethod]
        public void SimplePredicateQuery()
        {
            var db = new MyHiveDatabase(TestConfig.AzureHost, TestConfig.AzureUserName, TestConfig.AzurePassword, TestConfig.AzureStorageAccount, TestConfig.AzureStorageKey);

            var simpleQuery = from row in db.HiveSampleTable
                              where row.state == "Oregon"
                              orderby row.deviceplatform
                              select row;

            var results = simpleQuery.ToList();

            Assert.AreEqual(0, results.Where(r => r.state != "Oregon").Count());
        }


        [TestMethod]
        public void SimpleProjectionQuery()
        {
            var db = new MyHiveDatabase(TestConfig.AzureHost, TestConfig.AzureUserName, TestConfig.AzurePassword, TestConfig.AzureStorageAccount, TestConfig.AzureStorageKey);

            var projectionQuery = from row in db.HiveSampleTable
                                  where row.state == "Idaho"
                                  orderby row.clientid
                                  select new { row.clientid, row.state, row.country };


            var results = projectionQuery.ToList();
            
            Assert.AreEqual(results.Where(r => r.state != "Idaho").Count(), 0);
            var record = results[3];

            Assert.AreEqual("125616", record.clientid);
            Assert.AreEqual("Idaho", record.state);
            Assert.AreEqual("United States", record.country);

        }

        [TestMethod]
        public void SimpleGroupByQuery()
        {
            var db = new MyHiveDatabase(TestConfig.AzureHost, TestConfig.AzureUserName, TestConfig.AzurePassword, TestConfig.AzureStorageAccount, TestConfig.AzureStorageKey);

            var groupbyQuery = from row in db.HiveSampleTable
                                  group row by row.state into g
                                  select new { State = g.Key, Count = g.Count() };


            var results = groupbyQuery.ToDictionary(a => a.State);

            Assert.AreEqual(33, results["Alaska"].Count);
            Assert.AreEqual(42, results["Paris"].Count);

        }

        [TestMethod]
        public void SimpleCreateTable()
        {
            var db = new MyHiveDatabase(TestConfig.AzureHost, TestConfig.AzureUserName, TestConfig.AzurePassword, TestConfig.AzureStorageAccount, TestConfig.AzureStorageKey);

            var projectionQuery = from row in db.HiveSampleTable
                                  where row.state == "Idaho"
                                  orderby row.clientid
                                  select new { row.clientid, row.state, row.country };

            var newTable = projectionQuery.CreateTable("foo");

            Assert.AreEqual(60, newTable.Count());

            var query2 = from x in newTable
                         where x.clientid == "125616"
                         select x;

            Assert.AreEqual(2, query2.Count());

            var results = query2.ToList();

            Assert.AreEqual("125616", results[0].clientid);

            newTable.Drop();
        }
    }
}
