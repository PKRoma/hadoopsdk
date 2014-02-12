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
namespace Microsoft.Hadoop.Avro.Specifications
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Runtime.Serialization;

    public static class Utilities
    {
        [DataContract]
        public enum RandomEnumeration
        {
            Value0,
            Value1,
            Value2,
            Value3,
            Value4,
            Value5,
            Value6,
            Value7,
            Value8,
            Value9
        }

        private static readonly Random Random = new Random(13);

        public static Dictionary<Type, object> RandomGenerator = new Dictionary<Type, object>
        {
            { typeof(int), new Func<int>(Random.Next) },
            { typeof(double), new Func<double>(Random.NextDouble) },
            { typeof(string),  new Func<string>(() => "string" + Random.Next()) },
            { typeof(float), new Func<float>(() => (float)Random.NextDouble()) },
            { typeof(bool), new Func<bool>(() => Random.Next() % 2 == 0) },
            { typeof(long), new Func<long>(() => (long)Random.Next()) },
            { typeof(char), new Func<char>(() => (char)Random.Next('a', 'z')) },
            { typeof(byte), new Func<byte>(() => unchecked((byte)Random.Next())) },
            { typeof(sbyte), new Func<sbyte>(() => unchecked((sbyte)Random.Next())) },
            { typeof(short), new Func<short>(() => unchecked((short)Random.Next())) },
            { typeof(ushort), new Func<ushort>(() => unchecked((ushort)Random.Next())) },
            { typeof(uint), new Func<uint>(() => unchecked((uint)Random.Next())) },
            { typeof(ulong), new Func<ulong>(() => unchecked((ulong)Random.Next())) },
            { typeof(decimal), new Func<decimal>(() => new decimal(Random.NextDouble())) },
            { typeof(Guid), new Func<Guid>(Guid.NewGuid) },
            { typeof(DateTime), new Func<DateTime>(() => new DateTime(Random.Next())) },
        };

        public static T GetRandom<T>(bool nullablesCanBeNull)
        {
            var type = typeof(T);
            if (type == typeof(RandomEnumeration))
            {
                var array = Enum.GetValues(typeof(RandomEnumeration));
                return (T)array.GetValue(Random.Next(array.Length));
            }

            if (type.IsArray)
            {
                var elementType = type.GetElementType();
                var arraySize = Random.Next(100);
                var array = Array.CreateInstance(elementType, arraySize);
                var randomCall = typeof(Utilities).GetMethod("GetRandom").MakeGenericMethod(elementType);
                for (var i = 0; i < arraySize; i++)
                {
                    array.SetValue(randomCall.Invoke(null, new object[] { nullablesCanBeNull }), i);
                }
                return (T)Convert.ChangeType(array, type);
            }

            if (type.IsGenericType && typeof(Dictionary<,>).IsAssignableFrom(type.GetGenericTypeDefinition()))
            {
                var keyType = type.GetGenericArguments()[0];
                var keyRandomCall = typeof(Utilities).GetMethod("GetRandom").MakeGenericMethod(keyType);
                var valueType = type.GetGenericArguments()[1];
                var valueRandomCall = typeof(Utilities).GetMethod("GetRandom").MakeGenericMethod(valueType);
                var dictionary = Activator.CreateInstance(type);
                var addMethod = type.GetMethod("Add", new[] { keyType, valueType });
                var dictionarySize = Random.Next(100);
                for (var i = 0; i < dictionarySize; i++)
                {
                    addMethod.Invoke(dictionary, new[] { keyRandomCall.Invoke(null, new object[] { nullablesCanBeNull }), valueRandomCall.Invoke(null, new object[] { nullablesCanBeNull }) });
                }
                return (T)dictionary;
            }

            if (type != typeof(string))
            {
                Type enumerableType = null;
                foreach (var aType in type.GetInterfaces())
                {
                    if (aType.IsGenericType && aType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                    {
                        enumerableType = aType.GetGenericArguments()[0];
                    }
                }
                if (enumerableType != null)
                {
                    var enumerable = Activator.CreateInstance(type);
                    var arraySize = Random.Next(100);
                    var enumerableItemTypeCall = typeof(Utilities).GetMethod("GetRandom").MakeGenericMethod(enumerableType);
                    var addMethod = type.GetMethod("Add");
                    var temp = Enumerable.Repeat(0, arraySize).Select(i => enumerableItemTypeCall.Invoke(null, new object[] { nullablesCanBeNull })).ToArray();
                    foreach (var value in temp)
                    {
                        addMethod.Invoke(enumerable, new[] { value });
                    }
                    return (T)enumerable;
                }
            }

            var underlyingType = Nullable.GetUnderlyingType(type);
            if (underlyingType != null)
            {
                if (nullablesCanBeNull)
                {
                    var shouldBeNull = GetRandom<bool>(false);
                    if (shouldBeNull)
                    {
                        return default(T);
                    }
                }
                var method = typeof(Utilities).GetMethod("GetRandom").MakeGenericMethod(underlyingType);
                return (T)method.Invoke(null, new object[] { nullablesCanBeNull });
            }

            if (type == typeof(Uri))
            {
                return (T)Activator.CreateInstance(typeof(Uri), new object[] { "http://whatever" + GetRandom<string>(nullablesCanBeNull) });
            }

            if (type.IsClass)
            {
                var createMethod = type.GetMethod("Create", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
                if (createMethod != null)
                {
                    if (nullablesCanBeNull)
                    {
                        var shouldBeNull = GetRandom<bool>(false);
                        if (shouldBeNull)
                        {
                            return default(T);
                        }
                    }
                    return (T)createMethod.Invoke(null, new object[] { nullablesCanBeNull });
                }
            }

            object result;
            if (!RandomGenerator.TryGetValue(type, out result))
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Do not know how to generate a random value of type '{0}'", type));
            }
            return ((Func<T>)result)();
        }

        public static bool DictionaryEquals<TK, TV>(IDictionary<TK, TV> first, IDictionary<TK, TV> second)
        {
            if (first == null && second == null)
            {
                return true;
            }

            if (first == null || second == null)
            {
                return false;
            }

            if (first.Count != second.Count)
            {
                return false;
            }

            if (!first.Keys.SequenceEqual(second.Keys))
            {
                return false;
            }

            return first.Where(entry => !second[entry.Key].Equals(entry.Value)).Count() == 0;
        }

        public static bool JaggedEquals<T>(T[][] first, T[][] second)
        {
            if (first == null && second == null)
            {
                return true;
            }

            if (first == null || second == null)
            {
                return false;
            }

            if (first.Length != second.Length)
            {
                return false;
            }

            for (var i = 0; i < first.Length; i++)
            {
                if (first[i].Length != second[i].Length)
                {
                    return false;
                }

                if (!first[i].SequenceEqual(second[i]))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
