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
using Microsoft.Hadoop.Hive;

namespace LinqToHiveMapReduceSample
{
    // generated type
    public class MyHiveDatabase : HiveConnection
    {
        public MyHiveDatabase(Uri webHcatUri, string username, string password, string azureStorageAccount, string azureStorageKey) : base(webHcatUri, username, password, azureStorageAccount, azureStorageKey) { }

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

        public HiveTable<ActorsRow> Actors
        {
            get
            {
                return this.GetTable<ActorsRow>("Actors");
            }
        }
        public HiveTable<KeyValueInputRow> KeyValueInput
        {
            get
            {
                return this.GetTable<KeyValueInputRow>("kv_input");
            }
        }
        public HiveTable<Wonderland> Wonderland
        {
            get
            {
                return this.GetTable<Wonderland>("Wonderland");
            }
        }
        public HiveTable<WebSiteLog> WebSiteLog
        {
            get
            {
                return this.GetTable<WebSiteLog>("WebSiteLog");
            }
        }    }

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

    public class ActorsRow : HiveRow
    {
        public string MovieId { get; set; }
        public string ActorId { get; set; }
        public int AwardsCount { get; set; }
        public string Name { get; set; }
    }

    public class KeyValueInputRow : HiveRow
    {
        public string K { get; set; }
        public string V { get; set; }
    }

    public class Wonderland : HiveRow
    {
        public string Paragraph { get; set; }
    }

    public class WebSiteLog : HiveRow
    {
        public DateTime RequestDate { get; set; }
        public DateTime RequestTime { get; set; }
        public string   ServerIP { get; set; }
        public string   RequestMethod { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Justification="Hive doesn't support Uri class[ml]")]
        public string   RequestUri { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Justification = "Hive doesn't support Uri class[ml]")]
        public string   RequestUriQuery { get; set; }
        public int      ServerPort { get; set; }
        public string   UserName { get; set; }
        public string   ClientIP { get; set; }
        public string   UserAgent { get; set; }
        public string   HostName { get; set; }
        public int      ResponseStatus { get; set; }
        public int      ResponseSubstatus { get; set; }
        public int      ResponseWin32Status { get; set; }
        public int      ResponseBytes { get; set; }
        public int      RequestBytes { get; set; }
        public int      TimeTaken { get; set; }
    }

}
