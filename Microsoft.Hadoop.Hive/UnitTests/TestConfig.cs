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

public class TestConfig
{
    public static string AzureHost = "ClusterNAME";
    public static int AzurePort = 10000;
    public static string AzureUserName = "UserName";
    public static string AzurePassword = "Password";

    public static string LocalHost = "localhost";
    public static int LocalPort = 10000;
    public static string LocalUserName = null;
    public static string LocalPassword = null;
}

public class MyHiveDatabase : HiveConnection
{
    public MyHiveDatabase(string host, int port, string username, string password) : base(host, port, username, password) 
    { 
    }

    public HiveTable<HiveSampleTableRow> HiveSampleTable
    {
        get
        {
            return this.GetTable<HiveSampleTableRow>("hivesampletable");
        }
    }
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
