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
using System.Text;
using System.Reflection;
using System.Dynamic;
using System.Diagnostics.Contracts;

namespace Microsoft.Distributed.DevUnitTests
{
    /// <summary>
    /// A test helper class for accessing non-public static members of a class
    /// </summary>
    public class ExposedClass : DynamicObject
    {
        private Type m_type;
        private Dictionary<string, Dictionary<int, List<MethodInfo>>> m_staticMethods;
        private Dictionary<string, Dictionary<int, List<MethodInfo>>> m_genStaticMethods;

        /// <summary>
        /// A private constructor - use the From static method to construct instances
        /// </summary>
        /// <param name="type"></param>
        private ExposedClass(Type type)
        {
            Contract.Assert(type != null);

            m_type = type;

            m_staticMethods =
                m_type
                    .GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static)
                    .Where(m => !m.IsGenericMethod)
                    .GroupBy(m => m.Name)
                    .ToDictionary(
                        p => p.Key,
                        p => p.GroupBy(r => r.GetParameters().Length).ToDictionary(r => r.Key, r => r.ToList()));

            m_genStaticMethods =
                m_type
                    .GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static)
                    .Where(m => m.IsGenericMethod)
                    .GroupBy(m => m.Name)
                    .ToDictionary(
                        p => p.Key,
                        p => p.GroupBy(r => r.GetParameters().Length).ToDictionary(r => r.Key, r => r.ToList()));
        }

        /// <summary>
        /// Constructs an ExposedClass over the provided type.
        /// </summary>
        public static dynamic From(Type type)
        {
            if (type == null) throw new ArgumentNullException("type");
            return new ExposedClass(type);
        }


        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            // Get type args of the call
            Type[] typeArgs = ExposedObjectHelper.GetTypeArgs(binder);
            if (typeArgs != null && typeArgs.Length == 0) typeArgs = null;

            //
            // Try to call a non-generic static method
            //
            if (typeArgs == null
                    && m_staticMethods.ContainsKey(binder.Name)
                    && m_staticMethods[binder.Name].ContainsKey(args.Length)
                    && ExposedObjectHelper.InvokeBestMethod(args, null, m_staticMethods[binder.Name][args.Length], out result))
            {
                return true;
            }

            //
            // Try to call a generic static method
            //
            if (m_genStaticMethods.ContainsKey(binder.Name)
                    && m_genStaticMethods[binder.Name].ContainsKey(args.Length))
            {
                List<MethodInfo> methods = new List<MethodInfo>();

                foreach (var method in m_genStaticMethods[binder.Name][args.Length])
                {
                    if (method.GetGenericArguments().Length == typeArgs.Length)
                    {
                        methods.Add(method.MakeGenericMethod(typeArgs));
                    }
                }

                if (ExposedObjectHelper.InvokeBestMethod(args, null, methods, out result))
                {
                    return true;
                }
            }

            result = null;
            return false;
        }
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            var propertyInfo = m_type.GetProperty(
                binder.Name,
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);

            if (propertyInfo != null)
            {
                propertyInfo.SetValue(null, value, null);
                return true;
            }

            var fieldInfo = m_type.GetField(
                binder.Name,
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);

            if (fieldInfo != null)
            {
                fieldInfo.SetValue(null, value);
                return true;
            }

            return false;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var propertyInfo = m_type.GetProperty(
                binder.Name,
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);

            if (propertyInfo != null)
            {
                result = propertyInfo.GetValue(null, null);
                return true;
            }

            var fieldInfo = m_type.GetField(
                binder.Name,
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);

            if (fieldInfo != null)
            {
                result = fieldInfo.GetValue(null);
                return true;
            }

            result = null;
            return false;
        }
    }

}
