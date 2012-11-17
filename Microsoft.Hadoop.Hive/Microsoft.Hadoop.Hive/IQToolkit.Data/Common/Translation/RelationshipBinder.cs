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
    /// Translates accesses to relationship members into projections or joins
    /// </summary>
    public class RelationshipBinder : DbExpressionVisitor
    {
        QueryMapper mapper;
        QueryMapping mapping;
        QueryLanguage language;
        Expression currentFrom;

        private RelationshipBinder(QueryMapper mapper)
        {
            this.mapper = mapper;
            this.mapping = mapper.Mapping;
            this.language = mapper.Translator.Linguist.Language;
        }

        public static Expression Bind(QueryMapper mapper, Expression expression)
        {
            return new RelationshipBinder(mapper).Visit(expression);
        }

        protected override Expression VisitSelect(SelectExpression select)
        {
            Expression saveCurrentFrom = this.currentFrom;
            this.currentFrom = this.VisitSource(select.From);
            try
            {
                Expression where = this.Visit(select.Where);
                ReadOnlyCollection<OrderExpression> orderBy = this.VisitOrderBy(select.OrderBy);
                ReadOnlyCollection<Expression> groupBy = this.VisitExpressionList(select.GroupBy);
                Expression skip = this.Visit(select.Skip);
                Expression take = this.Visit(select.Take);
                ReadOnlyCollection<ColumnDeclaration> columns = this.VisitColumnDeclarations(select.Columns);
                if (this.currentFrom != select.From
                    || where != select.Where
                    || orderBy != select.OrderBy
                    || groupBy != select.GroupBy
                    || take != select.Take
                    || skip != select.Skip
                    || columns != select.Columns
                    )
                {
                    return new SelectExpression(select.Alias, columns, this.currentFrom, where, orderBy, groupBy, select.IsDistinct, skip, take, select.IsReverse);
                }
                return select;
            }
            finally
            {
                this.currentFrom = saveCurrentFrom;
            }
        }

        protected override Expression VisitMemberAccess(MemberExpression m)
        {
            Expression source = this.Visit(m.Expression);
            EntityExpression ex = source as EntityExpression;

            if (ex != null && this.mapping.IsRelationship(ex.Entity, m.Member))
            {
                ProjectionExpression projection = (ProjectionExpression)this.Visit(this.mapper.GetMemberExpression(source, ex.Entity, m.Member));
                if (this.currentFrom != null && this.mapping.IsSingletonRelationship(ex.Entity, m.Member))
                {
                    // convert singleton associations directly to OUTER APPLY
                    projection = this.language.AddOuterJoinTest(projection);
                    Expression newFrom = new JoinExpression(JoinType.OuterApply, this.currentFrom, projection.Select, null);
                    this.currentFrom = newFrom;
                    return projection.Projector;
                }
                return projection;
            }
            else
            {
                Expression result = QueryBinder.BindMember(source, m.Member);
                MemberExpression mex = result as MemberExpression;
                if (mex != null && mex.Member == m.Member && mex.Expression == m.Expression)
                {
                    return m;
                }
                return result;
            }
        }
    }
}
