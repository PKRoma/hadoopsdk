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
        private static IntegrationTestManager TestManager = new IntegrationTestManager();

        public static void RunSample()
        {
            AzureTestCredentials creds = TestManager.GetCredentials("default");

            var db = new MyHiveDatabase(new Uri(creds.Cluster), creds.AzureUserName, creds.AzurePassword, creds.DefaultStorageAccount.Name, creds.DefaultStorageAccount.Key);

            var condensed = db.KeyValueInput
                            .Where(c => c.k == "k1" || c.k == "k2" || c.k == "k3")
                            .Map(item =>
                            {
                                var k = item.k;
                                if (k == "k3")
                                    k = "k2";
                                return new
                                {
                                    k = k,
                                    v = item.v
                                };
                            })
                            .ClusterBy(item => item.k)
                            .Reduce(g => 
                            {
                                return new[] { 
                                    new { k = g.Key,
                                          values = string.Join(", ", g.Select(item => item.v).OrderBy(v => v) ) }
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
