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
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using IQToolkit.Data;
using IQToolkit.Data.Common;
using IQToolkit.Data.Mapping;
using System.Collections;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Data.Odbc;
using IQToolkit;

namespace Microsoft.Hadoop.Hive
{
    public class HiveQueryProvider : DbEntityProvider, IQueryProvider
    {
        internal HiveConnection HiveConnection { get; set; }
        internal StreamReader QueryResults { get; set; }

        internal HiveQueryProvider(HiveConnection hiveConnection, QueryMapping mapping, QueryPolicy policy)
            : base(hiveConnection.InternalConnection, new HiveLanguage(), mapping, policy)
        {
            this.HiveConnection = hiveConnection;
        }

        IQueryable<S> IQueryProvider.CreateQuery<S>(Expression expression)
        {
            return new HiveTable<S>.HiveOrderedTable(expression, this);
        }

        IQueryable IQueryProvider.CreateQuery(Expression expression)
        {
            Type elementType = TypeHelper.GetElementType(expression.Type);
            try
            {
                return (IQueryable)Activator.CreateInstance(typeof(HiveTable<>.HiveOrderedTable).MakeGenericType(elementType), new object[] { this, expression });
            }
            catch (TargetInvocationException tie)
            {
                throw tie.InnerException;
            }
        }

        public override DbEntityProvider New(DbConnection connection, QueryMapping mapping, QueryPolicy policy)
        {
            return new HiveQueryProvider(this.HiveConnection, mapping, policy);
        }

        protected override QueryExecutor CreateExecutor()
        {
            return new Executor(this, this.QueryResults);
        }

        public override Expression GetExecutionPlan(Expression expression)
        {
            return base.GetExecutionPlan(expression);
        }

        public async Task<bool> ExecuteQuery(Expression expression)
        {
            var queryText = this.GetQueryText(expression);
            this.QueryResults = await this.HiveConnection.ExecuteQuery(queryText);
            return true;
        }

        new class Executor : DbEntityProvider.Executor
        {
            private HiveQueryProvider provider;
            private StreamReader reader;

            public Executor(HiveQueryProvider provider)
                : base(provider)
            {
                this.provider = provider;
            }
            
            public Executor(HiveQueryProvider provider, StreamReader reader)
                : base(provider)
            {
                this.provider = provider;
                this.reader = reader;
            }

            public override IEnumerable<T> Execute<T>(QueryCommand command, Func<FieldReader, T> fnProjector,
                                                      MappingEntity entity, object[] paramValues)
            {
                var result = Project(this.reader, fnProjector, entity, true);

                result = result.ToList();

                return result;
            }

            private IEnumerable<T> Project<T>(StreamReader reader, Func<FieldReader, T> fnProjector, MappingEntity entity, bool closeReader)
            {
                try
                {
                    var line = string.Empty;
                    while ((line = reader.ReadLine()) != null)
                    {
                        yield return fnProjector(new HiveFieldReader(this, line));
                    }
                }
                finally
                {
                    if (closeReader)
                    {
                        reader.Close();
                    }
                }
            }

            public override object Convert(object value, Type type)
            {
                if (value == null)
                {
                    return TypeHelper.GetDefault(type);
                }
                type = TypeHelper.GetNonNullableType(type);
                Type vtype = value.GetType();
                if (type != vtype)
                {
                    if (type.IsEnum)
                    {
                        if (vtype == typeof(string))
                        {
                            return Enum.Parse(type, (string)value);
                        }
                        else
                        {
                            Type utype = Enum.GetUnderlyingType(type);
                            if (utype != vtype)
                            {
                                value = System.Convert.ChangeType(value, utype);
                            }
                            return Enum.ToObject(type, value);
                        }
                    }
                    return System.Convert.ChangeType(value, type);
                }
                return value;
            }
        }

        protected class HiveFieldReader : FieldReader
        {
            QueryExecutor executor;
            private string[] fields;

            public HiveFieldReader(QueryExecutor executor, string row)
            {
                this.executor = executor;
                this.fields = row.Split('\t');
                this.Init();
            }

            protected override int FieldCount
            {
                get { return fields.Count(); }
            }

            protected override Type GetFieldType(int ordinal)
            {
                return this.fields[ordinal].GetType();
            }

            protected override bool IsDBNull(int ordinal)
            {
                return string.IsNullOrEmpty(this.fields[ordinal]);
            }

            protected override T GetValue<T>(int ordinal)
            {
                return (T)this.executor.Convert(this.fields[ordinal], typeof(T));
            }

            protected override Byte GetByte(int ordinal)
            {
                return Convert.ToByte(fields[ordinal]);
            }

            protected override Char GetChar(int ordinal)
            {
                return Convert.ToChar(this.fields[ordinal]);
            }

            protected override DateTime GetDateTime(int ordinal)
            {
                return DateTime.Parse(this.fields[ordinal]);
            }

            protected override Decimal GetDecimal(int ordinal)
            {
                return Convert.ToDecimal(this.fields[ordinal]);
            }

            protected override Double GetDouble(int ordinal)
            {
                return Convert.ToDouble(this.fields[ordinal]);
            }

            protected override Single GetSingle(int ordinal)
            {
                return Convert.ToSingle(this.fields[ordinal]);
            }

            protected override Guid GetGuid(int ordinal)
            {
                throw new NotImplementedException();
            }

            protected override Int16 GetInt16(int ordinal)
            {
                return Convert.ToInt16(this.fields[ordinal]);
            }

            protected override Int32 GetInt32(int ordinal)
            {
                return Convert.ToInt32(this.fields[ordinal]);
            }

            protected override Int64 GetInt64(int ordinal)
            {
                return Convert.ToInt64(this.fields[ordinal]);
            }

            protected override String GetString(int ordinal)
            {
                return this.fields[ordinal];
            }
        }
    }
}


