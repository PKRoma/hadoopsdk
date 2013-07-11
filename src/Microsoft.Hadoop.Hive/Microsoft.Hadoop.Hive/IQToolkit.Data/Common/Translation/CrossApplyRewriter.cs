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
    /// Attempts to rewrite cross-apply and outer-apply joins as inner and left-outer joins
    /// </summary>
    public class CrossApplyRewriter : DbExpressionVisitor
    {
        QueryLanguage language;

        private CrossApplyRewriter(QueryLanguage language)
        {
            this.language = language;
        }

        public static Expression Rewrite(QueryLanguage language, Expression expression)
        {
            return new CrossApplyRewriter(language).Visit(expression);
        }

        protected override Expression VisitJoin(JoinExpression join)
        {
            join = (JoinExpression)base.VisitJoin(join);

            if (join.Join == JoinType.CrossApply || join.Join == JoinType.OuterApply)
            {
                if (join.Right is TableExpression)
                {
                    return new JoinExpression(JoinType.CrossJoin, join.Left, join.Right, null);
                }
                else
                {
                    SelectExpression select = join.Right as SelectExpression;
                    // Only consider rewriting cross apply if 
                    //   1) right side is a select
                    //   2) other than in the where clause in the right-side select, no left-side declared aliases are referenced
                    //   3) and has no behavior that would change semantics if the where clause is removed (like groups, aggregates, take, skip, etc).
                    // Note: it is best to attempt this after redundant subqueries have been removed.
                    if (select != null
                        && select.Take == null
                        && select.Skip == null
                        && !AggregateChecker.HasAggregates(select)
                        && (select.GroupBy == null || select.GroupBy.Count == 0)
                        && (select.ClusterBy == null || select.ClusterBy.Count == 0))
                    {
                        SelectExpression selectWithoutWhere = select.SetWhere(null);
                        HashSet<TableAlias> referencedAliases = ReferencedAliasGatherer.Gather(selectWithoutWhere);
                        HashSet<TableAlias> declaredAliases = DeclaredAliasGatherer.Gather(join.Left);
                        referencedAliases.IntersectWith(declaredAliases);
                        if (referencedAliases.Count == 0)
                        {
                            Expression where = select.Where;
                            select = selectWithoutWhere;
                            var pc = ColumnProjector.ProjectColumns(this.language, where, select.Columns, select.Alias, DeclaredAliasGatherer.Gather(select.From));
                            select = select.SetColumns(pc.Columns);
                            where = pc.Projector;
                            JoinType jt = (where == null) ? JoinType.CrossJoin : (join.Join == JoinType.CrossApply ? JoinType.InnerJoin : JoinType.LeftOuter);
                            return new JoinExpression(jt, join.Left, select, where);
                        }
                    }
                }
            }

            return join;
        }

        private bool CanBeColumn(Expression expr)
        {
            return expr != null && expr.NodeType == (ExpressionType)DbExpressionType.Column;
        }
    }
}
