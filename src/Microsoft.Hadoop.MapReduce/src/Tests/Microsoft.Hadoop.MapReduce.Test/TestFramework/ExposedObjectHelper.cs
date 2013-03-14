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

namespace Microsoft.Distributed.DevUnitTests
{
    /// <summary>
    /// Helper methods for ExposedObject and ExposedClass
    /// </summary>
    internal class ExposedObjectHelper
    {
        private static Type s_csharpInvokePropertyType =
            typeof(Microsoft.CSharp.RuntimeBinder.RuntimeBinderException)
                .Assembly
                .GetType("Microsoft.CSharp.RuntimeBinder.ICSharpInvokeOrInvokeMemberBinder");

        internal static bool InvokeBestMethod(object[] args, object target, List<MethodInfo> instanceMethods, out object result)
        {
            if (instanceMethods.Count == 1)
            {
                // Just one matching instance method - call it
                if (TryInvoke(instanceMethods[0], target, args, out result))
                {
                    return true;
                }
            }
            else if (instanceMethods.Count > 1)
            {
                // Find a method with best matching parameters
                MethodInfo best = null;
                Type[] bestParams = null;
                Type[] actualParams = args.Select(p => p == null ? typeof(object) : p.GetType()).ToArray();

                Func<Type[], Type[], bool> isAssignableFrom = (a, b) =>
                {
                    for (int i = 0; i < a.Length; i++)
                    {
                        if (!a[i].IsAssignableFrom(b[i])) return false;
                    }
                    return true;
                };


                foreach (var method in instanceMethods.Where(m => m.GetParameters().Length == args.Length))
                {
                    Type[] mParams = method.GetParameters().Select(x => x.ParameterType).ToArray();
                    if (isAssignableFrom(mParams, actualParams))
                    {
                        if (best == null || isAssignableFrom(bestParams, mParams))
                        {
                            best = method;
                            bestParams = mParams;
                        }
                    }
                }

                if (best != null && TryInvoke(best, target, args, out result))
                {
                    return true;
                }
            }

            result = null;
            return false;
        }

        internal static bool TryInvoke(MethodInfo methodInfo, object target, object[] args, out object result)
        {
            try
            {
                result = methodInfo.Invoke(target, args);
                return true;
            }
            catch (TargetParameterCountException)
            {
                // Wrong number of arguments was passed into the method.
            }
            catch (ArgumentException)
            {
                // An argument of a wrong type was passed into the method. Note that this exception
                // would be thrown by the reflection framework, not by the target method itself.
                // (In the latter case, TargetInvocationException(ArgumentException) would be thrown
                // instead.
            }

            result = null;
            return false;

        }

        internal static Type[] GetTypeArgs(InvokeMemberBinder binder)
        {
            if (s_csharpInvokePropertyType.IsInstanceOfType(binder))
            {
                PropertyInfo typeArgsProperty = s_csharpInvokePropertyType.GetProperty("TypeArguments");
                return ((IEnumerable<Type>)typeArgsProperty.GetValue(binder, null)).ToArray();
            }
            return null;
        }

    }

}
