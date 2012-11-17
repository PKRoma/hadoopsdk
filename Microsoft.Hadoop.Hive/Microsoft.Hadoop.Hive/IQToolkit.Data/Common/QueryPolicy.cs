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
    /// Defines query execution & materialization policies. 
    /// </summary>
    public class QueryPolicy
    {
        public QueryPolicy()
        {
        }

        /// <summary>
        /// Determines if a relationship property is to be included in the results of the query
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public virtual bool IsIncluded(MemberInfo member)
        {
            return false;
        }

        /// <summary>
        /// Determines if a relationship property is included, but the query for the related data is 
        /// deferred until the property is first accessed.
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public virtual bool IsDeferLoaded(MemberInfo member)
        {
            return false;
        }

        public virtual QueryPolice CreatePolice(QueryTranslator translator)
        {
            return new QueryPolice(this, translator);
        }

        public static readonly QueryPolicy Default = new QueryPolicy();
    }

    public class QueryPolice
    {
        QueryPolicy policy;
        QueryTranslator translator;

        public QueryPolice(QueryPolicy policy, QueryTranslator translator)
        {
            this.policy = policy;
            this.translator = translator;
        }

        public QueryPolicy Policy
        {
            get { return this.policy; }
        }

        public QueryTranslator Translator
        {
            get { return this.translator; }
        }

        public virtual Expression ApplyPolicy(Expression expression, MemberInfo member)
        {
            return expression;
        }

        /// <summary>
        /// Provides policy specific query translations.  This is where choices about inclusion of related objects and how
        /// heirarchies are materialized affect the definition of the queries.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public virtual Expression Translate(Expression expression)
        {
            // add included relationships to client projection
            var rewritten = RelationshipIncluder.Include(this.translator.Mapper, expression);
            if (rewritten != expression)
            {
                expression = rewritten;
                expression = UnusedColumnRemover.Remove(expression);
                expression = RedundantColumnRemover.Remove(expression);
                expression = RedundantSubqueryRemover.Remove(expression);
                expression = RedundantJoinRemover.Remove(expression);
            }

            // convert any singleton (1:1 or n:1) projections into server-side joins (cardinality is preserved)
            rewritten = SingletonProjectionRewriter.Rewrite(this.translator.Linguist.Language, expression);
            if (rewritten != expression)
            {
                expression = rewritten;
                expression = UnusedColumnRemover.Remove(expression);
                expression = RedundantColumnRemover.Remove(expression);
                expression = RedundantSubqueryRemover.Remove(expression);
                expression = RedundantJoinRemover.Remove(expression);
            }

            // convert projections into client-side joins
            rewritten = ClientJoinedProjectionRewriter.Rewrite(this.policy, this.translator.Linguist.Language, expression);
            if (rewritten != expression)
            {
                expression = rewritten;
                expression = UnusedColumnRemover.Remove(expression);
                expression = RedundantColumnRemover.Remove(expression);
                expression = RedundantSubqueryRemover.Remove(expression);
                expression = RedundantJoinRemover.Remove(expression);
            }

            return expression;
        }

        /// <summary>
        /// Converts a query into an execution plan.  The plan is an function that executes the query and builds the
        /// resulting objects.
        /// </summary>
        /// <param name="projection"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public virtual Expression BuildExecutionPlan(Expression query, Expression provider)
        {
            return ExecutionBuilder.Build(this.translator.Linguist, this.policy, query, provider);
        }
    }
}
