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
    /// Removes one or more SelectExpression's by rewriting the expression tree to not include them, promoting
    /// their from clause expressions and rewriting any column expressions that may have referenced them to now
    /// reference the underlying data directly.
    /// </summary>
    public class SubqueryRemover : DbExpressionVisitor
    {
        HashSet<SelectExpression> selectsToRemove;
        Dictionary<TableAlias, Dictionary<string, Expression>> map;

        private SubqueryRemover(IEnumerable<SelectExpression> selectsToRemove)
        {
            this.selectsToRemove = new HashSet<SelectExpression>(selectsToRemove);
            this.map = this.selectsToRemove.ToDictionary(d => d.Alias, d => d.Columns.ToDictionary(d2 => d2.Name, d2 => d2.Expression));
        }

        public static SelectExpression Remove(SelectExpression outerSelect, params SelectExpression[] selectsToRemove)
        {
            return Remove(outerSelect, (IEnumerable<SelectExpression>)selectsToRemove);
        }

        public static SelectExpression Remove(SelectExpression outerSelect, IEnumerable<SelectExpression> selectsToRemove)
        {
            return (SelectExpression)new SubqueryRemover(selectsToRemove).Visit(outerSelect);
        }

        public static ProjectionExpression Remove(ProjectionExpression projection, params SelectExpression[] selectsToRemove)
        {
            return Remove(projection, (IEnumerable<SelectExpression>)selectsToRemove);
        }

        public static ProjectionExpression Remove(ProjectionExpression projection, IEnumerable<SelectExpression> selectsToRemove)
        {
            return (ProjectionExpression)new SubqueryRemover(selectsToRemove).Visit(projection);
        }

        protected override Expression VisitSelect(SelectExpression select)
        {
            if (this.selectsToRemove.Contains(select))
            {
                return this.Visit(select.From);
            }
            else
            {
                return base.VisitSelect(select);
            }
        }

        protected override Expression VisitColumn(ColumnExpression column)
        {
            Dictionary<string, Expression> nameMap;
            if (this.map.TryGetValue(column.Alias, out nameMap))
            {
                Expression expr;
                if (nameMap.TryGetValue(column.Name, out expr))
                {
                    return this.Visit(expr);
                }
                throw new Exception("Reference to undefined column");
            }
            return column;
        }
    }
}
