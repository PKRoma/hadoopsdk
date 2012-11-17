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
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace IQToolkit.Data.Common
{
    /// <summary>
    /// Removes joins expressions that are identical to joins that already exist
    /// </summary>
    public class RedundantJoinRemover : DbExpressionVisitor
    {
        Dictionary<TableAlias, TableAlias> map;

        private RedundantJoinRemover()
        {
            this.map = new Dictionary<TableAlias, TableAlias>();
        }

        public static Expression Remove(Expression expression)
        {
            return new RedundantJoinRemover().Visit(expression);
        }

        protected override Expression VisitJoin(JoinExpression join)
        {
            Expression result = base.VisitJoin(join);
            join = result as JoinExpression;
            if (join != null)
            {
                AliasedExpression right = join.Right as AliasedExpression;
                if (right != null)
                {
                    AliasedExpression similarRight = (AliasedExpression)this.FindSimilarRight(join.Left as JoinExpression, join);
                    if (similarRight != null)
                    {
                        this.map.Add(right.Alias, similarRight.Alias);
                        return join.Left;
                    }
                }
            }
            return result;
        }

        private Expression FindSimilarRight(JoinExpression join, JoinExpression compareTo)
        {
            if (join == null)
                return null;
            if (join.Join == compareTo.Join)
            {
                if (join.Right.NodeType == compareTo.Right.NodeType
                    && DbExpressionComparer.AreEqual(join.Right, compareTo.Right))
                {
                    if (join.Condition == compareTo.Condition)
                        return join.Right;
                    var scope = new ScopedDictionary<TableAlias, TableAlias>(null);
                    scope.Add(((AliasedExpression)join.Right).Alias, ((AliasedExpression)compareTo.Right).Alias);
                    if (DbExpressionComparer.AreEqual(null, scope, join.Condition, compareTo.Condition))
                        return join.Right;
                }
            }
            Expression result = FindSimilarRight(join.Left as JoinExpression, compareTo);
            if (result == null)
            {
                result = FindSimilarRight(join.Right as JoinExpression, compareTo);
            }
            return result;
        }

        protected override Expression VisitColumn(ColumnExpression column)
        {
            TableAlias mapped;
            if (this.map.TryGetValue(column.Alias, out mapped))
            {
                return new ColumnExpression(column.Type, column.QueryType, mapped, column.Name);
            }
            return column;
        }
    }
}
