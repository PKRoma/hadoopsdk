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




using IQToolkit.Data.Common;
using IQToolkit;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Microsoft.Hadoop.Hive
{
    /// <summary>
    /// Converts LINQ query operators to into custom DbExpression's
    /// </summary>
    public class HiveMapBinder : QueryBinder
    {

        private HiveMapBinder(QueryMapper mapper, Expression root) : base(mapper, root)
        {

        }

        public static Expression Bind(QueryMapper mapper, Expression expression)
        {
            return new HiveMapBinder(mapper, expression).Visit(expression);
        }

        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            if (m.Method.DeclaringType == typeof(HiveExtensionMethods))
            {
                switch (m.Method.Name)
                {
                    case "Map":
                        return this.BindMap(m.Type, m.Arguments[0], m.Arguments[1]);
                }
            }
            return base.VisitMethodCall(m);
        }

        private Expression BindMap(Type resultType, Expression source, Expression lambda)
        {
            var query = 
                //@"add file ""c:/mapred-pl/identity.pl""; add file ""c:/mapred-pl/condense.pl""; from (  from kv_input  MAP k, v  USING 'cmd /c identity.pl'  as k, v cluster by k) map_output reduce k, v   using 'cmd /c condense.pl';";
                @"add file {0}; 
                  add file c:/mapred-pl/condense.pl; 
                  from (  
                    from kv_input  
                    MAP k, v  
                    USING './HiveDriver.exe {1} {2} {3}'  as k, v 
                   cluster by k) map_output 
                  reduce k, v   
                  using 'cmd /c condense.pl';";
            var exe_path = System.Reflection.Assembly.GetEntryAssembly().Location;
            var driver_path = Path.GetDirectoryName(exe_path) + "\\HiveDriver.exe";
            var method = ((MethodCallExpression)((LambdaExpression)lambda).Body).Method;
            var classname = method.DeclaringType.FullName;
            var methodname = Base64Codec.EncodeTo64(method.Name);

            query = string.Format(query, driver_path.Replace('\\', '/'), exe_path.Replace('\\', '/'), classname, methodname);

            var command = new QueryCommand(query, new List<QueryParameter> { });
            return Expression.Constant(command);

            //Console.WriteLine("ClassName=\"{0}\"\tMethodName=\"{1}\"", lambda.Method.DeclaringType.FullName, lambda.Method.Name);

            // call low-level execute directly on supplied DbQueryProvider
            //Expression result = Expression.Call(this.executor, "Execute", new Type[] { projector.Body.Type },
            //    Expression.Constant(command),
            //    projector,
            //    Expression.Constant(entity, typeof(MappingEntity)),
            //    Expression.NewArrayInit(typeof(object), values)
            //    );
            //return new MapExpression();
        }

    }

    internal class Base64Codec
    {
        static public string EncodeTo64(string toEncode)
        {
            byte[] toEncodeAsBytes
                = System.Text.ASCIIEncoding.ASCII.GetBytes(toEncode);
            string returnValue
                = System.Convert.ToBase64String(toEncodeAsBytes);
            return returnValue;
        }

        static public string DecodeFrom64(string encodedData)
        {
            byte[] encodedDataAsBytes
                = System.Convert.FromBase64String(encodedData);
            string returnValue =
               System.Text.ASCIIEncoding.ASCII.GetString(encodedDataAsBytes);
            return returnValue;
        }
    }

}
