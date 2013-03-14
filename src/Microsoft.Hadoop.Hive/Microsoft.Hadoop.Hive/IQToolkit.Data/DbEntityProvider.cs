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
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;

namespace IQToolkit.Data
{
   using System.Configuration;
    using Common;
    using Mapping;

    public class DbEntityProvider : EntityProvider
    {
        DbConnection connection;
        DbTransaction transaction;
        IsolationLevel isolation = IsolationLevel.ReadCommitted;

        int nConnectedActions = 0;
        bool actionOpenedConnection = false;

        public DbEntityProvider(DbConnection connection, QueryLanguage language, QueryMapping mapping, QueryPolicy policy)
            : base(language, mapping, policy)
        {
            //TODO: Figure out if we can actually do this... 
            //if (connection == null)
            //    throw new InvalidOperationException("Connection not specified");
            this.connection = connection;
        }

        public virtual DbConnection Connection
        {
            get { return this.connection; }
        }

        public virtual DbTransaction Transaction
        {
            get { return this.transaction; }
            set
            {
                if (value != null && value.Connection != this.connection)
                    throw new InvalidOperationException("Transaction does not match connection.");
                this.transaction = value;
            }
        }

        public IsolationLevel Isolation
        {
            get { return this.isolation; }
            set { this.isolation = value; }
        }

        public virtual DbEntityProvider New(DbConnection connection, QueryMapping mapping, QueryPolicy policy)
        {
            return (DbEntityProvider)Activator.CreateInstance(this.GetType(), new object[] { connection, mapping, policy });
        }

        public virtual DbEntityProvider New(DbConnection connection)
        {
            var n = New(connection, this.Mapping, this.Policy);
            n.Log = this.Log;
            return n;
        }

        public virtual DbEntityProvider New(QueryMapping mapping)
        {
            var n = New(this.Connection, mapping, this.Policy);
            n.Log = this.Log;
            return n;
        }

        public virtual DbEntityProvider New(QueryPolicy policy)
        {
            var n = New(this.Connection, this.Mapping, policy);
            n.Log = this.Log;
            return n;
        }

        public static DbEntityProvider FromApplicationSettings()
        {
            throw new NotImplementedException();
        }

        public static DbEntityProvider From(string connectionString, string mappingId)
        {
            throw new NotImplementedException();
        }

        public static DbEntityProvider From(string connectionString, string mappingId, QueryPolicy policy)
        {
            throw new NotImplementedException();
        }

        public static DbEntityProvider From(string connectionString, QueryMapping mapping, QueryPolicy policy)
        {
            throw new NotImplementedException();
        }

        public static DbEntityProvider From(string provider, string connectionString, string mappingId)
        {
            throw new NotImplementedException();
        }

        public static DbEntityProvider From(string provider, string connectionString, string mappingId, QueryPolicy policy)
        {
            throw new NotImplementedException();
        }

        public static DbEntityProvider From(string provider, string connectionString, QueryMapping mapping, QueryPolicy policy)
        {
            throw new NotImplementedException();
                }

        public static DbEntityProvider From(Type providerType, string connectionString, QueryMapping mapping, QueryPolicy policy)
        {
            throw new NotImplementedException();
                }

        protected void StartUsingConnection()
        {
            if (this.connection.State == ConnectionState.Closed)
            {
                this.connection.Open();
                this.actionOpenedConnection = true;
            }
            this.nConnectedActions++;
        }

        protected void StopUsingConnection()
        {
            System.Diagnostics.Debug.Assert(this.nConnectedActions > 0);
            this.nConnectedActions--;
            if (this.nConnectedActions == 0 && this.actionOpenedConnection)
            {
                this.connection.Close();
                this.actionOpenedConnection = false;
            }
        }

        public override void DoConnected(Action action)
        {
            throw new NotImplementedException();
            }

        public override void DoTransacted(Action action)
        {
            throw new NotImplementedException();
                    }

        public override int ExecuteCommand(string commandText)
        {
            throw new NotImplementedException();
            }

        protected override QueryExecutor CreateExecutor()
        {
            return new Executor(this);
        }

        public class Executor : QueryExecutor
        {
            DbEntityProvider provider;
            int rowsAffected = 0;

            public Executor(DbEntityProvider provider)
            {
                this.provider = provider;
            }

            public DbEntityProvider Provider
            {
                get { return this.provider; }
            }

            public override int RowsAffected
            {
                get { return this.rowsAffected; }
            }

            protected virtual bool BufferResultRows
            {
                get { return false; }
            }

            protected bool ActionOpenedConnection
            {
                get { return this.provider.actionOpenedConnection; }
            }

            protected void StartUsingConnection()
            {
                this.provider.StartUsingConnection();
            }

            protected void StopUsingConnection()
            {
                this.provider.StopUsingConnection();
            }

            public override object Convert(object value, Type type)
            {
                throw new NotImplementedException();
                }

            public override IEnumerable<T> Execute<T>(QueryCommand command, Func<FieldReader, T> fnProjector, MappingEntity entity, object[] paramValues)
            {
                throw new NotImplementedException();
                    }

            public override int ExecuteCommand(QueryCommand query, object[] paramValues)
            {
                throw new NotImplementedException();
                }

            public override IEnumerable<int> ExecuteBatch(QueryCommand query, IEnumerable<object[]> paramSets, int batchSize, bool stream)
            {
                throw new NotImplementedException();
                    }

            public override IEnumerable<T> ExecuteBatch<T>(QueryCommand query, IEnumerable<object[]> paramSets, Func<FieldReader, T> fnProjector, MappingEntity entity, int batchSize, bool stream)
            {
                throw new NotImplementedException();
                    }

            public override IEnumerable<T> ExecuteDeferred<T>(QueryCommand query, Func<FieldReader, T> fnProjector, MappingEntity entity, object[] paramValues)
            {
                throw new NotImplementedException();
                        }
                    }
                    }
                }
