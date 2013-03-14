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
// <copyright file="HiveTable.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
// table base object
// </summary>
//---------------------------------------------------------------------

using System.Threading.Tasks;

namespace Microsoft.Hadoop.Hive
{
    using System.Collections;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System;

    /// <summary>non-generic placeholder for generic implementation</summary>
    public abstract class HiveTable : IQueryable
    {
        protected HiveQueryProvider QueryProvider { get; set; }
        protected Expression IntenralExpression { get; set; }
        protected bool ExecuteQueryCompleted { get; set; }

        /// <summary>internal constructor so that only our assembly can provide an implementation</summary>
        internal HiveTable()
        {
           this.ExecuteQueryCompleted = false;
        }

        /// <summary>Linq Expression</summary>
        public abstract Expression Expression
        {
            get;
        }

        /// <summary>Linq Query Provider</summary>
        public abstract IQueryProvider Provider
        {
            get;
        }

        public abstract Type ElementType
        {
            get;
        }

        //TODO: clean up this exception we need to throw something or an assert. 
        public async Task ExecuteQuery()
        {
            if(this.QueryProvider == null)
            { 
                throw new ArgumentNullException("queryProvider");
            }
            this.ExecuteQueryCompleted = await this.QueryProvider.ExecuteQuery(this.Expression);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
