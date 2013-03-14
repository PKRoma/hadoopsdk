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
    /// Determines if a SelectExpression contains any aggregate expressions
    /// </summary>
    class AggregateChecker : DbExpressionVisitor
    {
        bool hasAggregate = false;
        private AggregateChecker()
        {
        }

        internal static bool HasAggregates(SelectExpression expression)
        {
            AggregateChecker checker = new AggregateChecker();
            checker.Visit(expression);
            return checker.hasAggregate;
        }

        protected override Expression VisitAggregate(AggregateExpression aggregate)
        {
            this.hasAggregate = true;
            return aggregate;
        }

        protected override Expression VisitSelect(SelectExpression select)
        {
            // only consider aggregates in these locations
            this.Visit(select.Where);
            this.VisitOrderBy(select.OrderBy);
            this.VisitColumnDeclarations(select.Columns);
            return select;
        }

        protected override Expression VisitSubquery(SubqueryExpression subquery)
        {
            // don't count aggregates in subqueries
            return subquery;
        }
    }
}
