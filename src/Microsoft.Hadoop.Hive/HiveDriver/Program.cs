using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using Microsoft.Hadoop.MapReduce;

enum DriverMode { Map, FlatMap, Reduce };

//FLATMAP c:\dev\hdx\eng\src\Microsoft.Hadoop.Hive\LinqToHiveMapReduceSample\bin\Debug\LinqToHiveMapReduceSample.exe LinqToHiveMapReduceSample.LinqToHiveWordCount PFJ1blNhbXBsZT5iX18w

namespace Microsoft.Hadoop.Hive
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                DriverMode mode = DriverMode.Map;
                switch (args[0].ToUpper())
                {
                    case "MAP":
                        mode = DriverMode.Map;
                        break;
                    case "FLATMAP":
                        mode = DriverMode.FlatMap;
                        break;
                    case "REDUCE":
                        mode = DriverMode.Reduce;
                        break;
                    default:
                        throw new Exception("Incorrect first parameter, must be MAP, FLATMAP or REDUCE");
                }

                string assembly = args[1].Replace('/', '\\');
                string classname = args[2];
                string methodname = Base64Codec.DecodeFrom64(args[3]);
                //@"LinqToHiveMapReduceSample.Program"
                //@"<Main>b__0";
                //string assembly = @"C:\dev\hdx\eng\src\Microsoft.Hadoop.Hive\LinqToHiveMapReduceSample\bin\Debug\LinqToHiveMapReduceSample.exe";

                var logfile = /*System.IO.Path.GetTempPath() +*/ "HiveDriver-" + DateTime.Now.ToString("MM-dd-yy--hh-mm-ss") + ".txt";
                Trace.Listeners.Add(new TextWriterTraceListener(new FileStream(logfile, FileMode.Create)));
                Trace.AutoFlush = true;
                Trace.WriteLine(assembly);
                Trace.WriteLine(classname);
                Trace.WriteLine(string.Format("{0}, Original:{1}", methodname, args[2]));

                Assembly userAssembly;
                Type type;

                try
                {
                    userAssembly = Assembly.LoadFrom(assembly);
                    type = userAssembly.GetType(classname);
                    if (type == null)
                    {
                        throw new Exception(string.Format("The user type could not be loaded. DLL={0}, Type={1}", assembly, classname));
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("The user type could not be loaded. DLL={0}, Type={1}", assembly, classname), ex);
                }

                var method = type.GetMethod(methodname, BindingFlags.Static | BindingFlags.NonPublic);
                if (method == null)
                {
                    throw new Exception(string.Format("The user type could not be loaded. DLL={0}, Type={1}", assembly, classname));
                }

                // Need to construct generic function which would call into lambda faster
                //var param_expr = new ParameterExpression[] { Expression.Parameter(method.GetParameters()[0].ParameterType) };
                //var call_expr = Expression.Call(method, param_expr);
                //var lambda_expr = Expression.Lambda(call_expr, param_expr);
                //var func = (Func<dynamic, IEnumerable<dynamic>>)lambda_expr.Compile();

                switch(mode) 
                {
                    case DriverMode.Map:
                        ProcessMap(method, method.GetParameters()[0].ParameterType, new StdinEnumerable());
                        break;
                    case DriverMode.FlatMap:
                        ProcessFlatMap(method, method.GetParameters()[0].ParameterType, new StdinEnumerable());
                        break;
                    case DriverMode.Reduce:
                        ProcessReduce(method, method.GetParameters()[0].ParameterType, new StdinEnumerable());
                        break;
                }
            }
            catch (Exception ex)
            {
                var logfile = System.IO.Path.GetTempPath() + "HiveDriver-" + DateTime.Now.ToString("MM-dd-yy--hh-mm-ss") + ".txt";
                //Trace.Listeners.Add(new TextWriterTraceListener(new FileStream(logfile, FileMode.Create)));
                Console.WriteLine(ex.ToString());
                Trace.AutoFlush = true;
                Trace.WriteLine(ex.ToString());
            }
        }

        internal static void ProcessMap(MethodInfo method, Type inputType, IEnumerable<string> inputEnumerable)
        {
            foreach (string line in inputEnumerable)
            {
                // Materializing input
                var input = CreateObject(inputType, line);
                var result = method.Invoke(null, new object[] { input });

                // Serializing output
                var values = new List<string>();
                foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(result.GetType()))
                    values.Add(property.GetValue(result).ToString());

                Console.WriteLine(string.Join("\t", values));
            }
        }

        internal static void ProcessFlatMap(MethodInfo method, Type inputType, IEnumerable<string> inputEnumerable)
        {
            foreach (string line in inputEnumerable)
            {
                // Materializing input
                var input = CreateObject(inputType, line);
                foreach (var result in (IEnumerable<object>)method.Invoke(null, new object[] { input }))
                {
                    // Serializing output
                    var values = new List<string>();
                    foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(result.GetType()))
                        values.Add(property.GetValue(result).ToString());

                    Console.WriteLine(string.Join("\t", values));
                }
            }
        }

        internal static void ProcessReduce(MethodInfo method, Type inputType, IEnumerable<string> inputEnumerable)
        {
            Grouper grouper = new Grouper(1, inputEnumerable);

            IGrouping<string, string> group;
            while ((group = grouper.NextGroup()) != null)
            {
                // Materializing input
                var input = CreateGroup(1, inputType, group);
                foreach (var result in (IEnumerable<object>)method.Invoke(null, new object[] { input }))
                {
                    // Serializing output
                    var values = new List<string>();
                    foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(result.GetType()))
                        values.Add(property.GetValue(result).ToString());

                    Console.WriteLine(string.Join("\t", values));
                }
            }
        }

        internal static object CreateObject(Type inputType, string line)
        {
            if (inputType.IsPrimitive || inputType == typeof(string) || inputType == typeof(decimal))
            {
                // The case when the key is one column simple value
                return Convert.ChangeType(line, inputType);
            }

            var lines = line.Split('\t');
            if ( inputType.GetConstructor(Type.EmptyTypes) != null )
            {
                // The case with default constructor
                var target = Activator.CreateInstance(inputType, true);
                int i = 0;
                foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(inputType))
                {
                    if ( i >= lines.Length )
                        break;
                    var value = Convert.ChangeType(lines[i], property.PropertyType);
                    property.SetValue(target, value);
                    i++;
                }
                return target;
            }
            else
            {
                // The case of no default constructor
                var props = new object[lines.Length];
                int i = 0;
                foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(inputType))
                {
                    if ( i >= lines.Length )
                        break;
                    var value = Convert.ChangeType(lines[i], property.PropertyType);
                    props[i] = value;
                    i++;
                }
                return Activator.CreateInstance(inputType, props);
            }
        }

        internal static object CreateGroup(int numberKeyElements, Type inputType, IGrouping<string, string> stringGroup)
        {
            Type groupType = typeof(TypedGroup<,>).MakeGenericType(inputType.GetGenericArguments());
            return Activator.CreateInstance(groupType, stringGroup);
        }
    }

    public class TypedGroup<TKey, TSource> : IGrouping<TKey, TSource>
    {
        private TKey _key;
        private bool _getEnumeratorCalled = false;
        private IGrouping<string, string> _parentGroup;
        private IEnumerator<string> _enumerator;

        public TypedGroup(IGrouping<string, string> parentGroup)
        {
            _parentGroup = parentGroup;
            this._key = (TKey)Program.CreateObject(typeof(TKey), parentGroup.Key);
        }

        public TKey Key
        {
            get { return _key; }
        }

        public IEnumerator<TSource> GetEnumerator()
        {
            if (_getEnumeratorCalled)
            {
                throw new InvalidOperationException(
                    "Parameter 'values' can only be enumerated once. To enumerate it multiple times, collect the list into a buffer. " +
                    "For example, var arr = values.ToArray();");
            }
            _getEnumeratorCalled = true;
            _enumerator = _parentGroup.GetEnumerator();
            while (_enumerator.MoveNext())
            {
                yield return (TSource)Program.CreateObject(typeof(TSource), string.Join("\t", _key, _enumerator.Current));
            };
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }


    internal class StdinEnumerable : IEnumerable<string>
    {
        public IEnumerator<string> GetEnumerator()
        {
            string line;
            while ((line = Console.ReadLine()) != null)
            {
                yield return line;
            }

        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
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
