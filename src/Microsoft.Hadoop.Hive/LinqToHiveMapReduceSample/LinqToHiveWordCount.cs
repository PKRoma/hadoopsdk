using Microsoft.Hadoop.Hive;
using Microsoft.WindowsAzure.Management.HDInsight.TestUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Microsoft.Hadoop.MapReduce;

namespace LinqToHiveMapReduceSample
{
    class LinqToHiveWordCount
    {
        private static IntegrationTestManager TestManager = new IntegrationTestManager();
        private static char[] _punctuationChars = new[] { 
            ' ', '!', '\"', '#', '$', '%', '&', '\'', '(', ')', '*', '+', ',', '-', '.', '/',   //ascii 23--47
            ':', ';', '<', '=', '>', '?', '@', '[', ']', '^', '_', '`', '{', '|', '}', '~' };   //ascii 58--64 + misc.

        public static void RunSample()
        {
            AzureTestCredentials creds = TestManager.GetCredentials("default");

            //LoadData(creds);

            var db = new MyHiveDatabase(new Uri(creds.Cluster), creds.AzureUserName, creds.AzurePassword, creds.DefaultStorageAccount.Name, creds.DefaultStorageAccount.Key);

            var counts = db.Wonderland
                         .FlatMap(item => item.Paragraph.Split(_punctuationChars).Select(w => new { Word = w }))
                         .GroupBy(item => item.Word)
                         .Select(g => new { Word = g.Key, Count = g.Count() });

            Console.WriteLine(counts.ToString());
            counts.ExecuteQuery().Wait();
            var results = counts.ToList();
            foreach (var item in results)
            {
                Console.WriteLine(item);
            }
            Console.ReadLine();
        }

        static void LoadData(AzureTestCredentials creds)
        {
            var hadoop = Hadoop.Connect(new Uri(creds.Cluster), creds.AzureUserName, creds.HadoopUserName, creds.AzurePassword, creds.DefaultStorageAccount.Name, creds.DefaultStorageAccount.Key, creds.DefaultStorageAccount.Container, true);
            var dataFile = "/input/pg11.txt";

            using (WebClient webclient = new WebClient())
            {
                var data1 = webclient.DownloadString("http://www.gutenberg.org/cache/epub/11/pg11.txt");
                hadoop.StorageSystem.WriteAllText(dataFile, data1);
            }

            var db = new HiveConnection(new Uri(creds.Cluster), creds.AzureUserName, creds.AzurePassword, creds.DefaultStorageAccount.Name, creds.DefaultStorageAccount.Key);

            db.GetTable<HiveRow>("Wonderland").Drop();

            string command = @"CREATE TABLE Wonderland(
                                    Paragraph string) 
                                    row format delimited fields 
                                    terminated by '\t';";

            db.ExecuteHiveQuery(command).Wait();

            command = string.Format(@"LOAD DATA INPATH '{0}' OVERWRITE INTO TABLE Wonderland", dataFile);

            db.ExecuteHiveQuery(command).Wait();
        }
    }
}
