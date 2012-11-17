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
using System.Linq.Expressions;
using System.Data;
using System.Data.Odbc;
using IQToolkit.Data.Common;
using IQToolkit.Data.Mapping;
using System.Reflection;
using System.Data.Linq;
using System.Data.Common;

namespace Microsoft.Hadoop.Hive
{
    public class HiveConnection
    {
        public int Port { get; set; }
        public string HostName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        internal OdbcConnection InternalConnection { get; set; }

        private DataContext materializer;
        private Dictionary<Type, string> typeToTableMap;
        private HiveMapping mapping;

        public HiveConnection(string hostName, int port) : this(hostName, port, null, null) {}

        public HiveConnection(string hostName, int port, string userName, string password)
        {
            this.Port = port;
            this.HostName = hostName;
            this.UserName = userName;
            this.Password = password;

            var builder = new OdbcConnectionStringBuilder();
            
            // authenticated
            //"DRIVER={HIVE};Description=;HOST=aconrad99.cloudapp.net;DATABASE=;PORT=10000;AUTH_DATA=aconrad;AUTHENTICATION=3;FRAMED=0;PWD=Chelsea99;UID=aconrad");

            // not authenticated
            //"DRIVER={HIVE};Description=;HOST=localhost;DATABASE=;PORT=10000;FRAMED=0;AUTHENTICATION=0;PWD=Chelsea99;UID=aconrad");

            // localhost connection string
            builder.Driver = "HIVE";
            builder.Add("PORT", this.Port);
            builder.Add("HOST", this.HostName);
            builder.Add("DATABASE", "");
            builder.Add("FRAMED", 0);
            builder.Add("AUTH_DATA", "aconrad");

            if (!String.IsNullOrEmpty(this.UserName) && !String.IsNullOrEmpty(this.Password))
            {
                builder.Add("AUTHENTICATION", 3);
                builder.Add("pwd", this.Password);
                builder.Add("Uid", this.UserName);
            }
            else
            {
                builder.Add("AUTHENTICATION", 0);
            }

            this.InternalConnection = new OdbcConnection(builder.ToString());
            materializer = new DataContext(this.InternalConnection);  // hack
            typeToTableMap = new Dictionary<Type, string>();
            mapping = new HiveMapping(this);
        }

        public DbDataReader ExecuteHiveQuery(string query)
        {
            if (this.InternalConnection.State != ConnectionState.Open)
            {
                this.InternalConnection.Open();
            }

            OdbcCommand command = this.InternalConnection.CreateCommand();
            command.CommandText = query;
            OdbcDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection);
            return reader;
        }

        public IEnumerable<T> ExecuteHiveQuery<T>(string query)
        {
            DbDataReader reader = this.ExecuteHiveQuery(query);
            return this.materializer.Translate<T>(reader);  // TODO - hack
        }

        public IDictionary<string, DataTable> GetMetaData()
        {
            Dictionary<string, DataTable> tables = new Dictionary<string, DataTable>();

            DbDataReader reader = this.ExecuteHiveQuery("SHOW Tables;");

            while (reader.Read())
            {
                tables.Add((string)(reader.GetValue(0)), null);
            }
            reader.Close();

            foreach (string tableName in tables.Keys.ToList())
            {
                reader = this.ExecuteHiveQuery("select * from " + tableName + " limit 1;");
                tables[tableName] = reader.GetSchemaTable();
            }
            reader.Close();

            return tables;
        }

        public HiveTable<T> GetTable<T>(string tableName)
        {
            if (!typeToTableMap.ContainsKey(typeof(T)))
            {
                typeToTableMap.Add(typeof(T), tableName);
            }

            return new HiveTable<T>.HiveOrderedTable(new HiveQueryProvider(this, mapping, new QueryPolicy()));
        }

        internal void DropTable<T>(HiveTable<T> table)
        {
            string tableName;

            if (typeToTableMap.TryGetValue(typeof(T), out tableName))
            {
                string statement = "DROP TABLE IF EXISTS " + tableName;
                this.ExecuteHiveQuery(statement);
            }
        }

        internal string FindTableName(Type t)
        {
            string name = null;
            this.typeToTableMap.TryGetValue(t, out name);
            return name;
        }
    }
}
