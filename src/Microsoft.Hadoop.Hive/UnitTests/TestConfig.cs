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
using Microsoft.Hadoop.Hive;
using Microsoft.Hadoop.WebHDFS.Adapters;

public class TestConfig
{
    public static string AzureHost = "ClusterNAME";
    public static int AzurePort = 10000;
    public static string AzureUserName = "UserName";
    public static string AzurePassword = "Password";

    public static string LocalHost = "localhost";
    public static string LocalWebHcatUri = "http://localhost:50111";
    public static string LocalWebHdfsUri = "http://localhost:50070";
    public static int LocalPort = 10000;
    public static string LocalUserName = "hadoop";
    public static string LocalPassword = null;

    public static string AzureStorageAccount = "";
    public static string AzureStorageKey = "";
}

public class MyHiveDatabase : HiveConnection
{
    public MyHiveDatabase(string host, string userName, string password, string storageAccount, string storageKey)
      : base(new Uri(string.Format("http://{0}:50111", host)), userName, password, storageAccount, storageKey)
    {
    }


    public HiveTable<HiveSampleTableRow> HiveSampleTable
    {
        get
        {
            return this.GetTable<HiveSampleTableRow>("hivesampletable");
        }
    }

    public HiveTable<AwardsRow> Awards
    {
        get
        {
            return this.GetTable<AwardsRow>("Awards");
        }
    }

    public HiveTable<TitlesRow> Titles
    {
        get
        {
            return this.GetTable<TitlesRow>("Titles");
        }
    }
}

public class TitlesRow : HiveRow
{
    public string MovieId { get; set; }
    public string Name { get; set; }
    public int Year { get; set; }
    public string Rating { get; set; }
}

public class AwardsRow : HiveRow
{
    public string MovieId { get; set; }
    public string AwardId { get; set; }
    public int Year { get; set; }
    public string Won { get; set; }
    public string Type { get; set; }
    public string Category { get; set; }
}

public class HiveSampleTableRow : HiveRow
{
    public string clientid { get; set; }
    public string querytime { get; set; }
    public string market { get; set; }
    public string deviceplatform { get; set; }
    public string devicemake { get; set; }
    public string devicemodel { get; set; }
    public string state { get; set; }
    public string country { get; set; }
    public double? querydwelltime { get; set; }
    public Int64? sessionid { get; set; }
    public Int64? sessionpagevieworder { get; set; }
}

public class HiveAwardTableRow : HiveRow
{
    public string category { get; set; }
    public string awardid { get; set; }
}

public class HiveAwardTableRowWithYear : HiveRow
{
    public string category { get; set; }
    public string awardid { get; set; }
    public int? year { get; set; }
}
