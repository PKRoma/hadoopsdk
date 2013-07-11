//  Copyright (c) Microsoft Corporation
//  All rights reserved.
// 
//  Licensed under the Apache License, Version 2.0 (the "License"); you may not
//  use this file except in compliance with the License.  You may obtain a copy
//  of the License at http://www.apache.org/licenses/LICENSE-2.0   
// 
//  THIS CODE IS PROVIDED *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
//  KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED 
//  WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, 
//  MERCHANTABLITY OR NON-INFRINGEMENT.  
// 
//  See the Apache Version 2.0 License for specific language governing 
//  permissions and limitations under the License. 


using Microsoft.Hadoop.Hive;
using Microsoft.Hadoop.MapReduce;
using Microsoft.WindowsAzure.Management.HDInsight.TestUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace LinqToHiveMapReduceSample
{
    class LinqToHiveWebSiteLogs
    {
        private static IntegrationTestManager testManager = new IntegrationTestManager();

        public static void RunSample()
        {
            AzureTestCredentials creds = testManager.GetCredentials("default");
            LoadData(creds);

            var db = new MyHiveDatabase(new Uri(creds.Cluster), creds.AzureUserName, creds.AzurePassword, creds.DefaultStorageAccount.Name, creds.DefaultStorageAccount.Key);

            var hitsByIP = db.WebSiteLog
                                 .Where(item => item.RequestMethod != "s-ip") // filter out header
                                 .GroupBy(item => item.ClientIP)
                                 .Select(g => new { ClientIP = g.Key, Count = g.Count() });

            var hitsByLocation = hitsByIP.Map(item =>
                                 {
                                     var location = LookupLocation(item.ClientIP);
                                     var areaLocation = LookupArea(location);
                                     return new { item.ClientIP, location.Longtitude, location.Latitude, AreaLongtitude = areaLocation.Longtitude, AreaLatitude = areaLocation.Latitude, item.Count };
                                 });

            var hitsByArea = hitsByLocation
                                 .ClusterBy(item => new { item.AreaLongtitude, item.AreaLatitude })
                                 .Reduce(g =>
                                 {
                                     var items = g.ToArray();
                                     var center = CalculateWeightedCenter(items.Select(item => new HitsByLocation
                                     {
                                         Location = new Location { Longtitude = item.Longtitude, Latitude = item.Latitude },
                                         Count = item.Count
                                     }));
                                     return new { 
                                         AreaLongtitude = center.Longtitude,
                                         AreaLatitude =   center.Latitude,
                                         Count = items.Sum(item => item.Count)
                                     };
                                 });

            Console.WriteLine(hitsByArea.ToString());
            hitsByArea.ExecuteQuery().Wait();
            var results = hitsByArea.ToList();
            Console.WriteLine("Output:");
            foreach (var item in results)
            {
                Console.WriteLine(item);
            }
            Console.ReadLine();
        }

        public class Location {
            public decimal Longtitude { get; set; }
            public decimal Latitude { get; set; }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "System.Int32.TryParse(System.String,System.Int32@)", Justification = "Leaving unparsed values as 0 [ml]")]
        static Location LookupLocation(string ip)
        {
            var strs = ip.Split('.');
            int x = 0;
            int y = 0; 
            if (strs.Length > 1)
            {
                int.TryParse(strs[0], out x);
                int.TryParse(strs[1], out y);
            }
            return new Location { Longtitude = x, Latitude = y };
        }

        static Location LookupArea(Location location)
        {
            return new Location { Longtitude = Math.Round(location.Longtitude/10.0m, 0)*10.0m, Latitude = Math.Round(location.Latitude/10.0m, 0)*10.0m };
        }

        public class HitsByLocation
        {
            public Location Location { get; set; }
            public int Count { get; set; }
        }

        static Location CalculateWeightedCenter(IEnumerable<HitsByLocation> hits)
        {
            return new Location
            {
                Longtitude = hits.Average(item => item.Location.Longtitude),
                Latitude = hits.Average(item => item.Location.Latitude)
            };
        }

        static void LoadData(AzureTestCredentials creds)
        {
            var hadoop = Hadoop.Connect(new Uri(creds.Cluster), creds.AzureUserName, creds.HadoopUserName, creds.AzurePassword, creds.DefaultStorageAccount.Name, creds.DefaultStorageAccount.Key, creds.DefaultStorageAccount.Container, true);
            var dataFile = "/input/sample-20131313.log";

            using (WebClient webclient = new WebClient())
            {
                var data1 = System.IO.File.ReadAllText(".\\sample-20131313.log");
                hadoop.StorageSystem.WriteAllText(dataFile, data1);
            }

            var db = new HiveConnection(new Uri(creds.Cluster), creds.AzureUserName, creds.AzurePassword, creds.DefaultStorageAccount.Name, creds.DefaultStorageAccount.Key);

            db.GetTable<HiveRow>("WebSiteLog").Drop();

            string command = @"CREATE TABLE IF NOT EXISTS WebSiteLog(
                                    RequestDate timestamp,
                                    RequestTime timestamp,
                                    ServerIP string,
                                    RequestMethod string,
                                    RequestUri string,
                                    RequestUriQuery string,
                                    ServerPort int,
                                    UserName string,
                                    ClientIP string,
                                    UserAgent string,
                                    HostName string,
                                    ResponseStatus int,
                                    ResponseSubstatus int,
                                    ResponseWin32Status int,
                                    ResponseBytes int,
                                    RequestBytes int,
                                    TimeTaken int)
                                    row format delimited fields 
                                    terminated by ' ';";

            db.ExecuteHiveQuery(command).Wait();

            command = string.Format(@"LOAD DATA INPATH '{0}' OVERWRITE INTO TABLE WebSiteLog", dataFile);

            db.ExecuteHiveQuery(command).Wait();
        }
    }
}
