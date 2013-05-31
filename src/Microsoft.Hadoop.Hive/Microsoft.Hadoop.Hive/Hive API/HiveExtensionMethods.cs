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
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Hadoop.Hive
{
    public static class HiveExtensionMethods
    {
        //TODO: Convert these to async and fixup the samples

        public static HiveTable<T> CreateTable<T>(this IQueryable<T> source, string tableName)
        {
            // TODO - this will only work for projectionss right now.
            // Perhaps should test for that here.

            var table = source as HiveTable<T>;
            if (table == null)
                throw new NotSupportedException();

            var provider = (HiveQueryProvider) table.Provider;
            HiveConnection hiveConnection = provider.HiveConnection;

            // TODO - should throw here if table exists already
            string querystring = "Create Table IF NOT EXISTS " + tableName + " AS " + table.QueryString;

            var queryTask =  hiveConnection.ExecuteHiveQuery<T>(querystring);
            queryTask.Wait();

            return hiveConnection.GetTable<T>(tableName);
        }

        public static IQueryable<T> InsertIntoTable<T>(this IQueryable<T> source, string tableName, bool overwrite)
        {
            return null;
        }

        public static void Drop<T>(this HiveTable<T> source)
        {
            HiveConnection hiveConnection = ((HiveQueryProvider)(source.Provider)).HiveConnection;
            var dropTask = hiveConnection.DropTable(source);
            dropTask.Wait();
        }

        public static async Task ExecuteQuery(this IQueryable source)
        {
            var table = source as HiveTable;
            if (table == null)
            {
                throw new InvalidOperationException("Object is not a HiveTable and cannot call ExecuteQuery.");
            }

            await table.ExecuteQuery();
        }

        public static IQueryable<TResult> Map<TSource, TResult>(this IQueryable<TSource> source, 
                                                                              Func<TSource, TResult> lambda)
        {
            var method = typeof(HiveExtensionMethods).GetMethod("Map");
            var param_expr = new ParameterExpression[] { Expression.Parameter(typeof(TSource)) };
            var call_expr = Expression.Call(lambda.Method, param_expr);
            var lambda_expr = Expression.Lambda(call_expr, param_expr);

            return source.Provider.CreateQuery<TResult>(
                        Expression.Call(method.MakeGenericMethod(new Type[] { typeof(TSource), typeof(TResult) }), source.Expression, lambda_expr));
        }

        public static IQueryable<TResult> FlatMap<TSource, TResult>(this IQueryable<TSource> source,
                                                                              Func<TSource, IEnumerable<TResult>> lambda)
        {
            var method = typeof(HiveExtensionMethods).GetMethod("FlatMap");
            var param_expr = new ParameterExpression[] { Expression.Parameter(typeof(TSource)) };
            var call_expr = Expression.Call(lambda.Method, param_expr);
            var lambda_expr = Expression.Lambda(call_expr, param_expr);

            return source.Provider.CreateQuery<TResult>(
                        Expression.Call(method.MakeGenericMethod(new Type[] { typeof(TSource), typeof(TResult) }), source.Expression, lambda_expr));
        }

        public static IQueryable<IGrouping<TKey, TSource>> ClusterBy<TSource, TKey>(this IQueryable<TSource> source, 
                                                                                        Expression<Func<TSource, TKey>> keySelector)
        {
            var method = typeof(HiveExtensionMethods).GetMethod("ClusterBy");
            return source.Provider.CreateQuery<IGrouping<TKey, TSource>>(
                        Expression.Call(method.MakeGenericMethod(new Type[] { typeof(TSource), typeof(TKey) }), source.Expression, keySelector));
        }

        public static IQueryable<TResult> Reduce<TSource, TResult>(this IQueryable<TSource> source,
                                                                              Func<TSource, IEnumerable<TResult>> lambda)
        {
            var method = typeof(HiveExtensionMethods).GetMethod("Reduce");
            var param_expr = new ParameterExpression[] { Expression.Parameter(typeof(TSource)) };
            var call_expr = Expression.Call(lambda.Method, param_expr);
            var lambda_expr = Expression.Lambda(call_expr, param_expr);

            return source.Provider.CreateQuery<TResult>(
                        Expression.Call(method.MakeGenericMethod(new Type[] { typeof(TSource), typeof(TResult) }), source.Expression, lambda_expr));
        }
    }

    public class StringPair
    {
        public string Item1 { get; set; }
        public string Item2 { get; set; }
    }
}
