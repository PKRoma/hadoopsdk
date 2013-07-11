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


using Microsoft.WindowsAzure.Management.HDInsight.TestUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Hadoop.Hive;
using Microsoft.Hadoop.MapReduce;

namespace LinqToHiveMapReduceSample
{
    class LinqToHiveCondense
    {
        private static IntegrationTestManager testManager = new IntegrationTestManager();

        public static void RunSample()
        {
            AzureTestCredentials creds = testManager.GetCredentials("default");

            var db = new MyHiveDatabase(new Uri(creds.Cluster), creds.AzureUserName, creds.AzurePassword, creds.DefaultStorageAccount.Name, creds.DefaultStorageAccount.Key);

            var condensed = db.KeyValueInput
                            .Where(c => c.K == "k1" || c.K == "k2" || c.K == "k3")
                            .Map(item =>
                            {
                                var k = item.K;
                                if (k == "k3")
                                    k = "k2";
                                return new
                                {
                                    k = k,
                                    v = item.V
                                };
                            })
                            .ClusterBy(item => item.k)
                            .Reduce(g => 
                            {
                                return new
                                {
                                    k = g.Key,
                                    values = string.Join(", ", g.Select(item => item.v).OrderBy(v => v))
                                };
                            });

            Console.WriteLine(condensed.ToString());
            condensed.ExecuteQuery().Wait();
            var results = condensed.ToList();
            foreach (var item in results)
            {
                Console.WriteLine(item);
            }
            Console.ReadLine();
        }
    }
}
