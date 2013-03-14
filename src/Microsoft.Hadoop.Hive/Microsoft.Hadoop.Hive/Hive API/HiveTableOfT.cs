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

//---------------------------------------------------------------------
// <copyright file="DataServiceQueryOfT.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
// query object
// </summary>
//---------------------------------------------------------------------

using System.Threading.Tasks;

namespace Microsoft.Hadoop.Hive
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Collections;
    using IQToolkit;

    [DebuggerDisplay("QueryText = {QueryString, nq}")]
    public class HiveTable<TElement> : HiveTable, IQueryable<TElement>
    {
        internal HiveTable(HiveQueryProvider provider)
        {
            this.IntenralExpression = Expression.Constant(this); ;
            this.QueryProvider = provider;
        }

        internal HiveTable(Expression expression, HiveQueryProvider provider)
        {
            if (!typeof(IQueryable<TElement>).IsAssignableFrom(expression.Type))
            {
                throw new ArgumentOutOfRangeException("expression");
            }
            this.QueryProvider = provider;
            this.IntenralExpression = expression;
        }


        #region IQueryable implementation

        public override Type ElementType
        {
            get { return typeof(TElement); }
        }

        public override Expression Expression
        {
            get { return this.IntenralExpression; }
        }

        public override IQueryProvider Provider
        {
            get { return this.QueryProvider; }
        }

        #endregion

        public string QueryString
        {
            get
            {
                return this.QueryProvider.GetQueryText(this.IntenralExpression);
            }
        }

        public IEnumerator<TElement> GetEnumerator()
        {
            if (!this.ExecuteQueryCompleted)
            {
                throw new InvalidOperationException("ExecuteQuery must be called and complete before calling GetEnumerator.");
            }
            return ((IEnumerable<TElement>)this.QueryProvider.Execute(this.Expression)).GetEnumerator(); ;
        }

        public override string ToString()
        {
            var s = this.QueryString;
            return s;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        [DebuggerDisplay("QueryText = {QueryString}")]
        internal class HiveOrderedTable : HiveTable<TElement>, IOrderedQueryable<TElement>, IOrderedQueryable
        {
            internal HiveOrderedTable(HiveQueryProvider provider)
                : base(provider)
            {
            }

            internal HiveOrderedTable(Expression e, HiveQueryProvider provider)
                : base(e, provider)
            {
            }
        }
    }
}
