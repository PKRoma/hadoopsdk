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
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Data;
using System.Data.Odbc;
using System.Threading.Tasks;
using IQToolkit.Data.Common;
using IQToolkit.Data.Mapping;
using System.Reflection;
using System.Data.Linq;
using System.Data.Common;
using Microsoft.Hadoop.WebHCat.Protocol;
using System.Net;
using System.Net.Http;
using Microsoft.Hadoop.WebHDFS;
using Microsoft.Hadoop.WebHDFS.Adapters;
using Newtonsoft.Json.Linq;

namespace Microsoft.Hadoop.Hive
{
    public class HiveConnection : IDisposable
    {
        public string HostName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string HadoopUserName { get; set; }
        
        internal OdbcConnection InternalConnection { get; set; }
        
        internal WebHCatHttpClient WebHCatHttpClient { get; set; }
        internal WebHDFSClient WebHdfsClient { get; set; }
        internal BlobStorageAdapter Adapter { get; set; }
        internal char HiveColumnDelimeter = '\t';
        internal StreamReader HiveQueryResults { get; set; }
        internal Dictionary<Type, string> TypeToTableMap { get; set; }
        internal List<string> JobFolders { get; set; }
        internal string baseDirectory = "asv://{0}@{1}";

        internal readonly HiveMapping Mapping;
        private bool disposed;

        internal List<KeyValuePair<string, string>> hiveHeaderParam = new List<KeyValuePair<string, string>>() { new KeyValuePair<string, string>("hive.cli.print.header", "true") };

        public HiveConnection(Uri webHCatUri, string userName, string password, string hadoopUserName, string storageAccount, string storageKey)
            : this(webHCatUri, userName, password, hadoopUserName)
        {
            var container = Guid.NewGuid().ToString();

            this.Adapter = new BlobStorageAdapter(storageAccount, storageKey, container, true);

            this.baseDirectory = string.Format(baseDirectory, container, storageAccount);

            this.WebHdfsClient = new WebHDFSClient(this.HadoopUserName, this.Adapter);
        }

        public HiveConnection(Uri webHCatUri, string userName, string password, string storageAccount, string storageKey) : this(webHCatUri, userName, password)
        {
            var container = Guid.NewGuid().ToString();
            this.Adapter = new BlobStorageAdapter(storageAccount, storageKey, container, true);

            this.baseDirectory = string.Format(baseDirectory, container, storageAccount);

            this.WebHdfsClient = new WebHDFSClient(this.HadoopUserName, Adapter);
        }

        public HiveConnection(string storageEndpoint, string storageAccount, string storageKey, Uri webHCatUri, string userName, string password )
            : this(webHCatUri, userName, password)
        {
            var container = Guid.NewGuid().ToString();

            this.Adapter = new BlobStorageAdapter(storageAccount, storageKey, storageEndpoint, container, true);

            this.baseDirectory = string.Format(baseDirectory, container, string.Format("{0}.{1}", storageAccount, storageEndpoint));

            this.WebHdfsClient = new WebHDFSClient(this.HadoopUserName, Adapter);
        }

        public HiveConnection(Uri webHCatUri, string userName, string password)
        {
            this.UserName = userName;
            this.Password = password;
            this.HadoopUserName = userName;

            this.WebHCatHttpClient = new WebHCatHttpClient(webHCatUri, this.UserName, this.Password);

            this.JobFolders = new List<string>();
            this.TypeToTableMap = new Dictionary<Type, string>();
        }

        public HiveConnection(Uri webHCatUri, string userName, string password, string hadoopUserName)
        {
            this.UserName = userName;
            this.Password = password;
            this.HadoopUserName = hadoopUserName;

            this.WebHCatHttpClient = new WebHCatHttpClient(webHCatUri, this.UserName, this.Password, this.HadoopUserName);

            this.JobFolders = new List<string>();
            this.TypeToTableMap = new Dictionary<Type, string>();
        }

        public async Task ExecuteHiveQuery(string query)
        {
            await ExecuteQuery(query);
        }

        public async Task<IEnumerable<T>> ExecuteHiveQuery<T>(string query)
        {
            var queryResults = await ExecuteQuery(query, this.hiveHeaderParam);

            var results = await queryResults.ReadToEndAsync();

            var resultTable = this.CreateHiveResultTable(results);

            return HiveMaterializer.Materialize<T>(resultTable);
        }

        public async Task<StreamReader> ExecuteQuery(string query)
        {
            return await this.ExecuteQuery(query, null);
        }

        public async Task<StreamReader> ExecuteQuery(string query, List<KeyValuePair<string, string>> queryParams)
        {
            var jobFolder = "/" + Guid.NewGuid();
            var asvPath = baseDirectory + jobFolder;

            var jobId = await this.QueueHiveQueryJob(query, asvPath, queryParams);
            this.JobFolders.Add(jobFolder);

            var results = await GetHiveJobResults(jobId, jobFolder);

            return new StreamReader(results);
        }

        internal async Task<string> QueueHiveQueryJob(string query, string jobFolder, List<KeyValuePair<string, string>> queryParams)
        {
            query = query.Replace(Environment.NewLine, " "); //NEIN, this needs to be fixed in the query writer. 

            var response = await this.WebHCatHttpClient.CreateHiveJob(query, null, queryParams, jobFolder, null);
            
            var result = await response.Content.ReadAsAsync<JObject>();
            response.EnsureSuccessStatusCode();

            return result["id"].ToString();
        }

        internal async Task<Stream> GetHiveJobResults(string jobId, string jobFolder)
        {
            var resultFileName = string.Format("{0}/stdout", jobFolder);

            await this.WebHCatHttpClient.WaitForJobToCompleteAsync(jobId);
            
            var stdOutFile= await this.WebHdfsClient.OpenFile(resultFileName);

            var resultText = await stdOutFile.Content.ReadAsStreamAsync();

            return resultText;
        }

        //TODO: Need DDL over WebHcat to flesh out the schema
        public async Task<IDictionary<string, DataTable>> GetMetaData()
        {
            var tables = new Dictionary<string, DataTable>();

            var reader = await this.ExecuteQuery("SHOW Tables;", hiveHeaderParam);

            var results = CreateHiveResultTable(reader.ReadToEnd());

            foreach (DataRow row in results.Rows)
            {
                tables.Add(row[0].ToString(), new DataTable());

            }

            //while (reader.Read())
            //{
            //    tables.Add((string)(reader.GetValue(0)), null);
            //}
            //reader.Close();

            //foreach (string tableName in tables.Keys.ToList())
            //{
            //    var dbReader = new DataTable().CreateDataReader();
            //    tables[tableName] = dbReader.GetSchemaTable();  <--------- Need to figure out how to get this over webHCat
            //}
            //reader.Close();

            return tables;
        }

        internal DataTable CreateHiveResultTable(string results)
        {
            var resultTable = new DataTable();

            if (!string.IsNullOrEmpty(results))
            {
                var reader = new StringReader(results);

                var row = string.Empty;
                while ((row = reader.ReadLine()) != null)
                {
                    var cols = row.Split(this.HiveColumnDelimeter);
                    if (resultTable.Columns.Count == 0)
                    {
                        resultTable.Columns.AddRange(cols.Select(c => new DataColumn(c)).ToArray());
                        continue;
                    }

                    var dataRow = resultTable.NewRow();
                    for (var i = 0; i < cols.Length; i++)
                    {
                        dataRow[i] = cols[i];
                    }
                    resultTable.Rows.Add(dataRow);
                }
            }
            return resultTable;
        }

        public HiveTable<T> GetTable<T>(string tableName)
        {
            if (!TypeToTableMap.ContainsKey(typeof(T)))
            {
                TypeToTableMap.Add(typeof(T), tableName);
            }

            return new HiveTable<T>.HiveOrderedTable(new HiveQueryProvider(this, new HiveMapping(this), new QueryPolicy()));
        }

        internal async Task DropTable<T>(HiveTable<T> table)
        {
            string tableName;

            if (TypeToTableMap.TryGetValue(typeof(T), out tableName))
            {
                string statement = "DROP TABLE IF EXISTS " + tableName;
                await this.ExecuteQuery(statement);
            }
        }

        internal string FindTableName(Type t)
        {
            string name = null;
            this.TypeToTableMap.TryGetValue(t, out name);
            return name;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    this.CleanUp();
                }
            }
            disposed = true;
        }

        protected void CleanUp()
        {
            foreach (var jobFolder in this.JobFolders)
            {
                var deleteTask = this.WebHdfsClient.DeleteDirectory(jobFolder,true);
                deleteTask.Wait();
            }
            if (Adapter != null)
            {
                Adapter.DeleteContainer();
            }
        }
    }
}
