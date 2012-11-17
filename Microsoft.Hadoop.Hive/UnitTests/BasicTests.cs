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
        public void BasicQueries()
        {
            var db = new MyHiveDatabase(TestConfig.LocalHost, TestConfig.LocalPort, TestConfig.LocalUserName, TestConfig.LocalPassword);

            var simpleQuery = from row in db.HiveSampleTable
                              where row.state == "Oregon"
                              orderby row.deviceplatform
                              select row;

            var expectedHQL = "SELECT t0.clientid, t0.country, t0.devicemake, t0.devicemodel, t0.deviceplatform, t0.market, "+
                            "t0.querydwelltime, t0.querytime, t0.sessionid, t0.sessionpagevieworder, t0.state " +
                            "FROM hivesampletable t0 " +
                            "WHERE (t0.state = 'Oregon') " +
                            "ORDER BY t0.deviceplatform";

            Assert.IsTrue(TestUtil.CompareQueries(simpleQuery.ToString(), expectedHQL));
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
