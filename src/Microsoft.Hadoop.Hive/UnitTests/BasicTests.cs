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
using System.Globalization;
using Microsoft.Hadoop.WebHDFS;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Hadoop.Hive;
using System.Linq;
using System.Text.RegularExpressions;

namespace UnitTests
{
    [TestClass]
    public class BasicUnitTests
    {
        [TestMethod]
        public void BasicQueryTranslation()
        {
            var db = new MyHiveDatabase(TestConfig.LocalHost, TestConfig.LocalUserName, TestConfig.LocalPassword, TestConfig.AzureStorageAccount, TestConfig.AzureStorageKey);


            var simpleQuery = from row in db.HiveSampleTable
                              where row.state == "Oregon"
                              orderby row.deviceplatform
                              select row;

            var expectedHQL =
                "SELECT t0.clientid, t0.country, t0.devicemake, t0.devicemodel, t0.deviceplatform, t0.market, " +
                            "t0.querydwelltime, t0.querytime, t0.sessionid, t0.sessionpagevieworder, t0.state " +
                            "FROM hivesampletable t0 " +
                            "WHERE (t0.state = 'Oregon') " +
                            "ORDER BY t0.deviceplatform";

            Assert.IsTrue(TestUtil.CompareQueries(simpleQuery.ToString(), expectedHQL));
        }

        [TestMethod]
        public void ExecuteSimpleHiveQueryFromConnection()
        {
            using (var db = new MyHiveDatabase(TestConfig.LocalHost, TestConfig.LocalUserName, TestConfig.LocalPassword, TestConfig.AzureStorageAccount, TestConfig.AzureStorageKey)
                )
            {
                const string query = "select awardid, category from Awards LIMIT 10;";

                var resultsTask = db.ExecuteHiveQuery<HiveAwardTableRow>(query);
                resultsTask.Wait();

                var results = resultsTask.Result.ToList();

                Assert.AreEqual(10, results.Count);
                Assert.AreEqual("3b132a46-c4a8-4129-8708-0c2189f9088b", results[0].awardid);
    }
        }

        [TestMethod]
        public void HiveConnectionHasWebClients()
        {
            var connection = new HiveConnection(new Uri(TestConfig.LocalWebHcatUri), TestConfig.LocalUserName, TestConfig.LocalPassword, TestConfig.AzureStorageAccount, TestConfig.AzureStorageKey);
            Assert.IsTrue(connection.WebHCatHttpClient != null);
            Assert.IsTrue(connection.WebHdfsClient != null);
        }

        [TestMethod]
        public void ExecuteSimpleHiveQueryFromLinqExpression()
        {
            using (var db = new MyHiveDatabase(TestConfig.LocalHost, TestConfig.LocalUserName, TestConfig.LocalPassword, TestConfig.AzureStorageAccount, TestConfig.AzureStorageKey)
                )
            {
                var query = from award in db.GetTable<HiveAwardTableRow>("Awards")
                            select new { AwardId = award.awardid + "aaa", Category = award.category };

                var queryTask = query.ExecuteQuery();
                queryTask.Wait();

                var results = query.ToList();

                Assert.IsTrue(results.Count > 0);
            }
        }

        [TestMethod]
        public void ExecuteCreateTable()
        {
            using (var db = new MyHiveDatabase(TestConfig.LocalHost, TestConfig.LocalUserName, TestConfig.LocalPassword, TestConfig.AzureStorageAccount, TestConfig.AzureStorageKey)
                )
            {
                var projectionQuery = from row in db.GetTable<HiveAwardTableRow>("Awards")
                                      where row.awardid != "3b132a46-c4a8-4129-8708-0c2189f9088b"
                                      orderby row.awardid
                                      select new { row.awardid, row.category };

                var newTable = projectionQuery.CreateTable("foo");

                var newTableQueryTask = newTable.ExecuteQuery();
                newTableQueryTask.Wait();

                //TODO: Fix the issue when calling Count() directly from newTable
                Assert.AreEqual(12382, newTable.ToList().Count());

                newTable.Drop();
            }
        }


        [ExpectedException(typeof(InvalidOperationException))]
        [TestMethod]
        public void GetEnumeratorBeforeExecute()
        {
            using (var db = new MyHiveDatabase(TestConfig.LocalHost, TestConfig.LocalUserName, TestConfig.LocalPassword, TestConfig.AzureStorageAccount, TestConfig.AzureStorageKey)
                )
            {
                var query = from award in db.GetTable<HiveAwardTableRow>("Awards")
                            select new { AwardId = award.awardid + "aaa", Category = award.category };

                query.GetEnumerator();
            }
        }

        [TestMethod]
        public void ExecuteSimpleHiveQuery()
        {
            var connection = new HiveConnection(new Uri(TestConfig.LocalWebHcatUri), TestConfig.LocalUserName, TestConfig.LocalPassword, TestConfig.AzureStorageAccount, TestConfig.AzureStorageKey);
            var result = connection.ExecuteQuery("Show Tables;");

            Assert.IsTrue(result != null);
        }

        [TestMethod]
        public void ExecuteHiveQueryWithAggregates()
        {
            using (var db = new MyHiveDatabase(TestConfig.LocalHost, TestConfig.LocalUserName, TestConfig.LocalPassword, TestConfig.AzureStorageAccount, TestConfig.AzureStorageKey)
                )
            {
                // get the sum of lifetime award counts for movies in 19994
                var query4 = from x in db.GetTable<HiveAwardTableRowWithYear>("Awards")
                             group x by x.category
                             into g
                             select new { Name = g.Key, Sum = g.Sum(p => p.year) };

                var queryTask = query4.ExecuteQuery();
                queryTask.Wait();

                var results = query4.ToList();

                Assert.IsTrue(results.Count > 0);
                Assert.IsTrue(string.Compare(results[3].Name, "Best Actor", true, CultureInfo.InvariantCulture) == 0);
                Assert.IsTrue(results[3].Sum == 1246888);
            }
        }

        [TestMethod]
        public void ExecuteHiveQueryWithJoin()
        {
            using (var db = new MyHiveDatabase(TestConfig.LocalHost, TestConfig.LocalUserName, TestConfig.LocalPassword, TestConfig.AzureStorageAccount, TestConfig.AzureStorageKey)
                )
            {
                // get the sum of lifetime award counts for movies in 19994
                var projectionQuery = from aw in db.Awards
                                      join t in db.Titles
                                          on aw.MovieId equals t.MovieId
                                      where t.Year == 1994 && aw.Won == "True"
                                      select
                                          new
                                              {
                                                  MovieId = t.MovieId,
                                                  Name = t.Name,
                                                  Type = aw.Type,
                                                  Category = aw.Category,
                                                  Year = t.Year
                                              };

                var queryTask = projectionQuery.ExecuteQuery();
                queryTask.Wait();

                var results = projectionQuery.ToList();


                Assert.IsTrue(results.Count > 0);
                Assert.IsTrue(string.Compare(results[2].Name, "Crumb", true, CultureInfo.InvariantCulture) == 0);
                Assert.IsTrue(string.Compare(results[10].Name, "Pulp Fiction", true, CultureInfo.InvariantCulture) == 0);
                Assert.IsTrue(results[2].Year == 1994);
                Assert.IsTrue(results[10].MovieId == "7tEu");
            }
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void ExecuteHiveQueryWithSingletonFails()
        {
            using (var db = new MyHiveDatabase(TestConfig.LocalHost, TestConfig.LocalUserName, TestConfig.LocalPassword, TestConfig.AzureStorageAccount, TestConfig.AzureStorageKey))
            {
                var query = from award in db.GetTable<HiveAwardTableRow>("Awards")
                            select new { AwardId = award.awardid + "aaa", Category = award.category };

                query.Count();
            }
        }

        [TestMethod]
        public void JobFoldersAreCleanedUpOnDispose()
        {
            var jobFolders = new List<string>();
            using (var db = new MyHiveDatabase(TestConfig.LocalHost, TestConfig.LocalUserName, TestConfig.LocalPassword, TestConfig.AzureStorageAccount, TestConfig.AzureStorageKey))
            {
                var query = from award in db.GetTable<HiveAwardTableRow>("Awards")
                            select new { AwardId = award.awardid + "aaa", Category = award.category };

                var queryTask = query.ExecuteQuery();
                queryTask.Wait();
                jobFolders = db.JobFolders;
            }

            var webHdfsClient = new WebHDFSClient(new Uri(TestConfig.LocalWebHdfsUri), null);

            foreach (string folder in jobFolders)
            {
                var dirStatusTask = webHdfsClient.GetDirectoryStatus(folder);
                try
                {
                    dirStatusTask.Wait();
                }
                catch (Exception e)
                {
                    Assert.IsTrue(dirStatusTask.IsFaulted);
                    Assert.IsTrue(e.InnerException.Message.Contains("404"));
                    continue;
                }
                
                Assert.IsTrue(dirStatusTask.Result == null);
            }
        }
    }

    public class TestUtil
    {
        public static bool CompareQueries(string l, string r)
        {
            l = Regex.Replace(l, @"\r\n", " ");
            r = Regex.Replace(l, @"\r\n", " ");

            return String.Equals(l, r);
        }
    }
}
