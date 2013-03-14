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
using System.Dynamic;
using System.Reflection;
using System.Globalization;

namespace Microsoft.Distributed.DevUnitTests
{

    //Debugging Hint
    // - to debug into the TryInvokeMember() code, set "Options|Debugging|Just My Code -> true"

    // Usage note:
    // - when using dynamic object, calls to methods defined on the class itself should work.
    // - implemented interfaces, base-class methods etc may not work --> if they are public, cast the dynamic object to the public type, then call.

    //Example usage:
    //
    // dynamic obj = ExposedObject.New(Type.GetType("{Namespace.TypeName}, {AssemblyName}"), new object [] {params});
    // {TRetval} val = obj.{MethodName}({args});


    /// <summary>
    /// A test helper class for accessing non-public members of an object
    /// </summary>
    public class ExposedObject : DynamicObject
    {
        private object m_object;
        private Type m_type;
        private Dictionary<string, Dictionary<int, List<MethodInfo>>> m_instanceMethods;
        private Dictionary<string, Dictionary<int, List<MethodInfo>>> m_genInstanceMethods;

        /// <summary>
        /// A private constructor - use the From static method to construct instances
        /// </summary>
        private ExposedObject(object obj)
        {
            m_object = obj;
            m_type = obj.GetType();

            m_instanceMethods =
                m_type
                    .GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                    .Where(m => !m.IsGenericMethod)
                    .GroupBy(m => m.Name)
                    .ToDictionary(
                        p => p.Key,
                        p => p.GroupBy(r => r.GetParameters().Length).ToDictionary(r => r.Key, r => r.ToList()));

            m_genInstanceMethods =
                m_type
                    .GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                    .Where(m => m.IsGenericMethod)
                    .GroupBy(m => m.Name)
                    .ToDictionary(
                        p => p.Key,
                        p => p.GroupBy(r => r.GetParameters().Length).ToDictionary(r => r.Key, r => r.ToList()));
        }

        /// <summary>
        /// Exposes internals of an existing object
        /// </summary>
        public static dynamic From(object obj)
        {
            return new ExposedObject(obj);
        }

        /// <summary>
        /// Constructs a new object of some type and wraps it with an ExposedObject.
        /// </summary>
        public static dynamic New<T>()
        {
            return New(typeof(T));
        }

        /// <summary>
        /// Constructs a new object of some type and wraps it with an ExposedObject. The type is allowed 
        /// to refer to a type that is not visible to the executing code. For example, it can be an internal 
        /// type in another assembly.
        /// </summary>
        public static dynamic New(Type type, params object[] args)
        {
            return new ExposedObject(Activator.CreateInstance(type, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public, null, args, CultureInfo.CurrentCulture));
        }

        /// <summary>
        /// Returns the instance of the wrapped object 
        /// </summary>
        public object Object { get { return m_object; } }

        /// <summary>
        /// Casts the wrapped object to a base class or an interface
        /// </summary>
        public static T Cast<T>(ExposedObject t)
        {
            return (T)t.m_object;
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            // Get type args of the call
            Type[] typeArgs = ExposedObjectHelper.GetTypeArgs(binder);
            if (typeArgs != null && typeArgs.Length == 0) typeArgs = null;

            //
            // Try to call a non-generic instance method
            //
            if (typeArgs == null
                    && m_instanceMethods.ContainsKey(binder.Name)
                    && m_instanceMethods[binder.Name].ContainsKey(args.Length)
                    && ExposedObjectHelper.InvokeBestMethod(args, m_object, m_instanceMethods[binder.Name][args.Length], out result))
            {
                return true;
            }

            //
            // Try to call a generic instance method
            //
            if (m_genInstanceMethods.ContainsKey(binder.Name)
                    && m_genInstanceMethods[binder.Name].ContainsKey(args.Length))
            {
                List<MethodInfo> methods = new List<MethodInfo>();

                foreach (var method in m_genInstanceMethods[binder.Name][args.Length])
                {
                    if (method.GetGenericArguments().Length == typeArgs.Length)
                    {
                        methods.Add(method.MakeGenericMethod(typeArgs));
                    }
                }

                if (ExposedObjectHelper.InvokeBestMethod(args, m_object, methods, out result))
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
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

            if (propertyInfo != null)
            {
                propertyInfo.SetValue(m_object, value, null);
                return true;
            }

            var fieldInfo = m_type.GetField(
                binder.Name,
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

            if (fieldInfo != null)
            {
                fieldInfo.SetValue(m_object, value);
                return true;
            }

            return false;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var propertyInfo = m_object.GetType().GetProperty(
                binder.Name,
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

            if (propertyInfo != null)
            {
                result = propertyInfo.GetValue(m_object, null);
                return true;
            }

            var fieldInfo = m_object.GetType().GetField(
                binder.Name,
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

            if (fieldInfo != null)
            {
                result = fieldInfo.GetValue(m_object);
                return true;
            }

            result = null;
            return false;
        }

        public override bool TryConvert(ConvertBinder binder, out object result)
        {
            result = m_object;
            return true;
        }
    }
}
