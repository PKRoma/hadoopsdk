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
            return new Executor(this);
        }

        public override Expression GetExecutionPlan(Expression expression)
        {
            return base.GetExecutionPlan(expression);
        }

        new class Executor : DbEntityProvider.Executor
        {
            HiveQueryProvider provider;

            public Executor(HiveQueryProvider provider)
                : base(provider)
            {
                this.provider = provider;
            }

            public override IEnumerable<T> Execute<T>(QueryCommand command, Func<FieldReader, T> fnProjector, MappingEntity entity, object[] paramValues)
            {
                return base.Execute<T>(command, fnProjector, entity, paramValues);
            }
        }
    }
}


