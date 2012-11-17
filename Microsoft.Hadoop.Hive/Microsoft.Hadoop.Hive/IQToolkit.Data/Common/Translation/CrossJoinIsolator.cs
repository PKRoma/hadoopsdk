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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace IQToolkit.Data.Common
{
    /// <summary>
    /// Isolates cross joins from other types of joins using nested sub queries
    /// </summary>
    public class CrossJoinIsolator : DbExpressionVisitor
    {
        ILookup<TableAlias, ColumnExpression> columns;
        Dictionary<ColumnExpression, ColumnExpression> map = new Dictionary<ColumnExpression,ColumnExpression>();
        JoinType? lastJoin;

        public static Expression Isolate(Expression expression)
        {
            return new CrossJoinIsolator().Visit(expression);
        }

        protected override Expression VisitSelect(SelectExpression select)
        {
            var saveColumns = this.columns;
            this.columns = ReferencedColumnGatherer.Gather(select).ToLookup(c => c.Alias);
            var saveLastJoin = this.lastJoin;
            this.lastJoin = null;
            var result = base.VisitSelect(select);
            this.columns = saveColumns;
            this.lastJoin = saveLastJoin;
            return result;
        }

        protected override Expression VisitJoin(JoinExpression join)
        {
            var saveLastJoin = this.lastJoin;
            this.lastJoin = join.Join;
            join = (JoinExpression)base.VisitJoin(join);
            this.lastJoin = saveLastJoin;

            if (this.lastJoin != null && (join.Join == JoinType.CrossJoin) != (this.lastJoin == JoinType.CrossJoin))
            {
                var result = this.MakeSubquery(join);
                return result;
            }
            return join;
        }

        private bool IsCrossJoin(Expression expression)
        {
            var jex = expression as JoinExpression;
            if (jex != null)
            {
                return jex.Join == JoinType.CrossJoin;
            }
            return false;
        }

        private Expression MakeSubquery(Expression expression)
        {
            var newAlias = new TableAlias();
            var aliases = DeclaredAliasGatherer.Gather(expression);

            var decls = new List<ColumnDeclaration>();
            foreach (var ta in aliases) 
            {
                foreach (var col in this.columns[ta])
                {
                    string name = decls.GetAvailableColumnName(col.Name);
                    var decl = new ColumnDeclaration(name, col, col.QueryType);
                    decls.Add(decl);
                    var newCol = new ColumnExpression(col.Type, col.QueryType, newAlias, col.Name);
                    this.map.Add(col, newCol);
                }
            }

            return new SelectExpression(newAlias, decls, expression, null);
        }

        protected override Expression VisitColumn(ColumnExpression column)
        {
            ColumnExpression mapped;
            if (this.map.TryGetValue(column, out mapped))
            {
                return mapped;
            }
            return column;
        }
    }
}
