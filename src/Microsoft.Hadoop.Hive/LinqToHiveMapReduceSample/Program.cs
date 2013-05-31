using Microsoft.Hadoop.Hive;
using Microsoft.WindowsAzure.Management.HDInsight.TestUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqToHiveMapReduceSample
{
    public class Row
    {
        public string R { get; set; }
    }

    class Program
    {
        private static IntegrationTestManager TestManager = new IntegrationTestManager();

        static void Main(string[] args)
        {
            //LinqToHiveWordCount.RunSample();
            LinqToHiveCondense.RunSample();

            //AzureTestCredentials creds = TestManager.GetCredentials("default");

            //var db = new MyHiveDatabase(new Uri(creds.Cluster), creds.AzureUserName, creds.AzurePassword, creds.DefaultStorageAccount.Name, creds.DefaultStorageAccount.Key);

            //var q = db.KVInput
            //            .Select(a => new { a.k, a.v })
            //            .Where(c => c.k == "k1" || c.k == "k2")
            //            .Map(item =>
            //            {
            //                return new
            //                {
            //                    k = string.Format("--{0}--", item.k),
            //                    v = string.Format("++{0}++", item.v)
            //                };
            //            })
            //            .GroupBy(item => item.k)
            //            .Select(g => new { k = g.Key, count = g.Count() });
            
            //var q = db.KVInput
            //            .Select(a => new { new_k = a.k })
            //            .Where(c => c.new_k == "k1" || c.new_k == "k2")
            //            .Map(item =>
            //            {
            //                return new[] { 
            //                    new { k = string.Format("--{0}--", item.new_k), 
            //                          v = string.Format("++{0}++", item.new_k) }
            //                };
            //            })
            //            .GroupBy(item => item.k)
            //            .Select(g => new { k = g.Key, count = g.Count() });
                        //.OrderByDescending( o => o.count );

            //q.ExecuteQuery().Wait();
            //var results = q.ToList();
            //foreach (var item in results)
            //{
            //    Console.WriteLine(item);
            //}
            //Console.ReadLine();
        }


    }

    [System.AttributeUsage(System.AttributeTargets.Parameter)]
    public class ColumnAttribute : System.Attribute
    {
        public string column;
        ColumnAttribute(string column)
        {
            this.column = column;
        }
    }
}
