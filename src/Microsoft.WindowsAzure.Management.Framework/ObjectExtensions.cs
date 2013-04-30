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

namespace Microsoft.WindowsAzure.Management.Framework
{
    using System.Collections.Generic;

    /// <summary>
    ///     Provides extensions methods to the Object class.
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        ///     Casts the specified obj.
        /// </summary>
        /// <typeparam name="T"> The target type of the cast. </typeparam>
        /// <param name="inputValue"> The object. </param>
        /// <returns> The result of the cast. </returns>
        public static T CastTo<T>(this object inputValue)
        {
            return (T)inputValue;
        }

        /// <summary>
        ///     Evaluates type compatibility.
        /// </summary>
        /// <typeparam name="T"> The type to evaluate against. </typeparam>
        /// <param name="inputValue"> The object to evaluate compatibility for. </param>
        /// <returns> True if the object is compatible otherwise false. </returns>
        public static bool Is<T>(this object inputValue)
        {
            return inputValue is T;
        }

        /// <summary>
        ///     Performs an as operation on the supplied object.
        /// </summary>
        /// <typeparam name="T"> The target type of the as operation. </typeparam>
        /// <param name="inputValue"> The object. </param>
        /// <returns> The result of the as operation. </returns>
        public static T As<T>(this object inputValue) where T : class
        {
            return inputValue as T;
        }

        /// <summary>
        ///     Determines whether the specified object is null.
        /// </summary>
        /// <param name="inputValue"> The object. </param>
        /// <returns>
        ///     <c>true</c> if the specified object is null; otherwise, <c>false</c> .
        /// </returns>
        public static bool IsNull([ValidatedNotNull] this object inputValue)
        {
            return ReferenceEquals(inputValue,
                                   null);
        }

        /// <summary>
        ///     Determines whether the specified object is not null.
        /// </summary>
        /// <param name="inputValue"> The object. </param>
        /// <returns>
        ///     <c>true</c> if the specified object is not null; otherwise, <c>false</c> .
        /// </returns>
        public static bool IsNotNull([ValidatedNotNull] this object inputValue)
        {
            return !ReferenceEquals(inputValue,
                                    null);
        }

        /// <summary>
        /// Returns an IEnumerable(of T) containing the supplied object as it's only entry.
        /// </summary>
        /// <typeparam name="T">
        /// The type of IEnumerable to return.
        /// </typeparam>
        /// <param name="item">
        /// The item to be used to create the enumerable.
        /// </param>
        /// <returns>
        /// An IEnumerable(of T) containing the supplied object.
        /// </returns>
        public static IEnumerable<T> MakeEnumeration<T>(this T item)
        {
            List<T> results = new List<T>();
            results.Add(item);
            return results;
        }
    }
}