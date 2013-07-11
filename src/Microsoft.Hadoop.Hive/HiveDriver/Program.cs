//  Copyright (c) Microsoft Corporation
//  All rights reserved.
// 
//  Licensed under the Apache License, Version 2.0 (the "License"); you may not
//  use this file except in compliance with the License.  You may obtain a copy
//  of the License at http://www.apache.org/licenses/LICENSE-2.0   
// 
//  THIS CODE IS PROVIDED *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
//  KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED 
//  WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, 
//  MERCHANTABLITY OR NON-INFRINGEMENT.  
// 
//  See the Apache Version 2.0 License for specific language governing 
//  permissions and limitations under the License. 


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
using System.Text.RegularExpressions;
using Microsoft.Hadoop.WebHDFS;
using Microsoft.Hadoop.WebHDFS.Adapters;
using Microsoft.Hadoop.WebClient.Common;
using System.Globalization;
using System.Diagnostics.CodeAnalysis;

enum DriverMode { Map, MapMany, Reduce, ReduceMany };

namespace Microsoft.Hadoop.Hive
{
    class HiveDriver
    {
        static WebHDFSClient client = null;
        internal static string logfile = null;

        [SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods", MessageId = "System.Reflection.Assembly.LoadFrom",
            Justification = "Suppressing for now until this can be reworked. [tgs]")]
        [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Microsoft.Hadoop.Hive.HiveDriver.WriteLog(System.String,System.String)",
            Justification = "Suppressing for now as this is developer facing. [tgs]")]
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "ClusterBy",
            Justification = "Suppressing for now as this is developer facing. [tgs]")]
        static void Main(string[] args)
        {
            try
            {
                string[] keyColumns = null;
                DriverMode mode = DriverMode.Map;
                switch (args[0].ToUpper(CultureInfo.InvariantCulture))
                {
                    case "MAP":
                        mode = DriverMode.Map;
                        break;
                    case "MAPMANY":
                        mode = DriverMode.MapMany;
                        break;
                    case "REDUCE":
                        mode = DriverMode.Reduce;
                        if (args.Length < 4)
                        {
                            throw new InvalidOperationException("REDUCE mode requires 4th parameter to be list of ClusterBy key columns, e.g. \"key1,key2,key3\".");
                        }
                        keyColumns = args[4].Split(',');
                        break;
                    case "REDUCEMANY":
                        mode = DriverMode.ReduceMany;
                        if (args.Length < 4)
                        {
                            throw new InvalidOperationException("REDUCE mode requires 4th parameter to be list of ClusterBy key columns, e.g. \"key1,key2,key3\".");
                        }
                        keyColumns = args[4].Split(',');
                        break;
                    default:
                        throw new InvalidOperationException("Incorrect first parameter, must be MAP, MAPMANY, REDUCE or REDUCEMANY");
                }

                string assembly = args[1].Replace('/', '\\');
                string classname = args[2];
                string methodname = Base64Codec.DecodeFrom64(args[3]);

                PrepareLogStorage();
                WriteLog(logfile, assembly);
                WriteLog(logfile, classname);
                WriteLog(logfile, string.Format(CultureInfo.InvariantCulture, "{0}, Original:{1}", methodname, args[3]));

                Assembly userAssembly;
                Type type;

                try
                {
                    userAssembly = Assembly.LoadFrom(assembly);
                    type = userAssembly.GetType(classname);
                    if (type == null)
                    {
                        throw new TypeLoadException(string.Format(CultureInfo.InvariantCulture, "The user type could not be loaded. DLL={0}, Type={1}", assembly, classname));
                    }
                }
                catch (Exception ex)
                {
                    throw new TypeLoadException(string.Format(CultureInfo.InvariantCulture, "The user type could not be loaded. DLL={0}, Type={1}", assembly, classname), ex);
                }

                var method = type.GetMethod(methodname, BindingFlags.Static | BindingFlags.NonPublic);
                if (method == null)
                {
                    throw new TypeLoadException(string.Format(CultureInfo.InvariantCulture, "The user type could not be loaded. DLL={0}, Type={1}", assembly, classname));
                }

                // TODO: Need to construct generic function which would call into lambda faster

                switch(mode) 
                {
                    case DriverMode.Map:
                        ProcessMap(method, method.GetParameters()[0].ParameterType, new StdinEnumerable());
                        break;
                    case DriverMode.MapMany:
                        ProcessMapMany(method, method.GetParameters()[0].ParameterType, new StdinEnumerable());
                        break;
                    case DriverMode.Reduce:
                        ProcessReduce(method, method.GetParameters()[0].ParameterType, keyColumns, new StdinEnumerable());
                        break;
                    case DriverMode.ReduceMany:
                        ProcessReduceMany(method, method.GetParameters()[0].ParameterType, keyColumns, new StdinEnumerable());
                        break;
                }
            }
            catch (Exception ex)
            {
                WriteLog(logfile, ex.ToString());
            }
        }

        private static void PrepareLogStorage()
        {
            var fs_default_name = Environment.GetEnvironmentVariable("fs_default_name");
            if (fs_default_name != null)
            {
                var regex = new Regex(@"asv://(?<container>\w+)\@(?<account>.+)");
                var match = regex.Match(fs_default_name);
                var container = match.Groups["container"].Value;
                var account = match.Groups["account"].Value;
                var key = Environment.GetEnvironmentVariable("fs_azure_account_key_" + account.Replace('.', '_'));

                using (var adapter = new BlobStorageAdapter(account, key, container, true))
                {
                    client = new WebHDFSClient(Environment.GetEnvironmentVariable("USERNAME"), adapter);

                    logfile = "/templeton-hadoop/jobs/" + Environment.GetEnvironmentVariable("mapred_job_id") + "/HiveDriverLogs/" + DateTime.Now.ToString("MM-dd-yy--hh-mm-ss", CultureInfo.InvariantCulture);

                    var envs = Environment.GetEnvironmentVariables();
                    WriteLog(logfile, string.Join("\n", envs.Keys.Cast<string>().Select(item => item + "=" + envs[item])));
                }
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
                    values.Add(string.Format(CultureInfo.InvariantCulture, "{0}", property.GetValue(result)));

                Console.WriteLine(string.Join("\t", values));
            }
        }

        internal static void ProcessMapMany(MethodInfo method, Type inputType, IEnumerable<string> inputEnumerable)
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
                        values.Add(string.Format(CultureInfo.InvariantCulture, "{0}", property.GetValue(result)));

                    Console.WriteLine(string.Join("\t", values));
                }
            }
        }

        internal static void ProcessReduce(MethodInfo method, Type inputType, string[] keyColumns, IEnumerable<string> inputEnumerable)
        {
            var valueType = inputType.GetGenericArguments()[1];
            var columns = TypeDescriptor.GetProperties(valueType).Cast<PropertyDescriptor>()
                          .Select(item => item.Name)
                          .ToArray();

            int[] keyIndexes = keyColumns.Select(keyColumn => columns.TakeWhile(column => !string.Equals(column, keyColumn, StringComparison.OrdinalIgnoreCase) ).Count()).ToArray();
            if (keyIndexes.Any(index => index >= columns.Length))
            {
                throw new InvalidOperationException("Key column not found in the list of the input columns.");
            }
            Grouper grouper = new Grouper(keyIndexes, inputEnumerable);

            IGrouping<string, string> group;
            while ((group = grouper.NextGroup()) != null)
            {
                // Materializing input
                var input = CreateGroup(inputType, group);
                var result = method.Invoke(null, new object[] { input });

                // Serializing output
                var values = new List<string>();
                foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(result.GetType()))
                    values.Add(string.Format(CultureInfo.InvariantCulture, "{0}", property.GetValue(result)));

                Console.WriteLine(string.Join("\t", values));
            }
        }

        internal static void ProcessReduceMany(MethodInfo method, Type inputType, string[] keyColumns, IEnumerable<string> inputEnumerable)
        {
            var valueType = inputType.GetGenericArguments()[1];
            var columns = TypeDescriptor.GetProperties(valueType).Cast<PropertyDescriptor>()
                          .Select(item => item.Name)
                          .ToArray();

            int[] keyIndexes = keyColumns.Select(keyColumn => columns.TakeWhile(column => !string.Equals(column, keyColumn, StringComparison.OrdinalIgnoreCase)).Count()).ToArray();
            if (keyIndexes.Any(index => index >= columns.Length))
            {
                throw new InvalidOperationException("Key column not found in the list of the input columns.");
            }
            Grouper grouper = new Grouper(keyIndexes, inputEnumerable);

            IGrouping<string, string> group;
            while ((group = grouper.NextGroup()) != null)
            {
                // Materializing input
                var input = CreateGroup(inputType, group);
                foreach (var result in (IEnumerable<object>)method.Invoke(null, new object[] { input }))
                {
                    // Serializing output
                    var values = new List<string>();
                    foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(result.GetType()))
                        values.Add(string.Format(CultureInfo.InvariantCulture, "{0}", property.GetValue(result)));

                    Console.WriteLine(string.Join("\t", values));
                }
            }
        }

        internal static object CreateObject(Type inputType, string line)
        {
            if (inputType.IsPrimitive || inputType == typeof(string) || inputType == typeof(decimal))
            {
                // The case when the key is one column simple value
                return ConvertType(line, inputType);
            }

            var lines = line.Split('\t');
            if (inputType.GetConstructor(Type.EmptyTypes) != null)
            {
                // The case with default constructor
                var target = Activator.CreateInstance(inputType, true);
                int i = 0;
                foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(inputType))
                {
                    if (i >= lines.Length)
                        break;
                    var value = ConvertType(lines[i], property.PropertyType);
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
                    if (i >= lines.Length)
                        break;
                    var value = ConvertType(lines[i], property.PropertyType);
                    props[i] = value;
                    i++;
                }
                return Activator.CreateInstance(inputType, props);
            }
        }

        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "TargetType",
            Justification = "This is correct in context.  I'll update dictionary with later checkin. [tgs]")]
        [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Microsoft.Hadoop.Hive.HiveDriver.WriteLog(System.String,System.String)",
            Justification = "Acceptable as this is developer facing. [tgs]")]
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "I'm letting this in for now, but it needs to be reworked. [tgs]")]
        internal static object ConvertType(string text, Type conversionType)
        {
            if (text == "\\N")
                return null;
            try
            {
                return Convert.ChangeType(text, conversionType, CultureInfo.InvariantCulture);
            }
            catch(Exception ex)
            {
                WriteLog(logfile, string.Format(CultureInfo.InvariantCulture, "Type Conversion failed, returning null. Value=\"{0}\", TargetType={1}. Exception:\n{2}", text, conversionType.Name, ex.ToString()));
                return null;
            }
        }

        internal static object CreateGroup(Type inputType, IGrouping<string, string> stringGroup)
        {
            Type groupType = typeof(TypedGroup<,>).MakeGenericType(inputType.GetGenericArguments());
            return Activator.CreateInstance(groupType, stringGroup);
        }

        static Dictionary<string, int> createdFiles = new Dictionary<string, int>();
        internal static void WriteLog(string file, string text)
        {
            Console.Error.WriteLine(text);

            if (client != null)
            {
                using(var buffer = new MemoryStream())
                using (var stream = new StreamWriter(buffer))
                {
                    foreach (var line in text.Split('\n'))
                    {
                        stream.WriteLine(line);
                    }
                    stream.Flush();
                    buffer.Position = 0;

                    // TODO: Switch to AppendFile when it is supported by BlobAdapter
                    int count = 0;
                    if (createdFiles.ContainsKey(file))
                    {
                        count = createdFiles[file];
                    }
                    createdFiles[file] = count + 1;
                    client.CreateFile(buffer, string.Format(CultureInfo.InvariantCulture, "{0}/{1:D3}.log", file, count)).Wait();
                }
            }
        }
    }

    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix",
        Justification = "Suppressing for now. [tgs]")]
    public class TypedGroup<TKey, TSource> : IGrouping<TKey, TSource>
    {
        private TKey _key;
        private bool _getEnumeratorCalled = false;
        private IGrouping<string, string> _parentGroup;
        private IEnumerator<string> _enumerator;

        public TypedGroup(IGrouping<string, string> parentGroup)
        {
            if (ReferenceEquals(parentGroup, null))
            {
                throw new ArgumentNullException("parentGroup");
            }
            _parentGroup = parentGroup;
            this._key = (TKey)HiveDriver.CreateObject(typeof(TKey), parentGroup.Key);
        }

        public TKey Key
        {
            get { return _key; }
        }

        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "ToArray",
            Justification = "This is acceptable in this context.  I'll update the dictionary on a later checkin. [tgs]")]
        public IEnumerator<TSource> GetEnumerator()
        {
            if (_getEnumeratorCalled)
            {
                throw new InvalidOperationException(
                    "Parameter 'values' can only be enumerated once. To enumerate it multiple times, collect the list into a buffer. " +
                    "For example, var result = values.ToArray();");
            }
            _getEnumeratorCalled = true;
            _enumerator = _parentGroup.GetEnumerator();
            while (_enumerator.MoveNext())
            {
                yield return (TSource)HiveDriver.CreateObject(typeof(TSource), _enumerator.Current);
            };
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }


    internal class StdinEnumerable : IEnumerable<string>
    {
        int lineCount = 0;
        public IEnumerator<string> GetEnumerator()
        {
            string line;
            while ((line = Console.ReadLine()) != null)
            {
                if (lineCount++ < 10)
                {
                    HiveDriver.WriteLog(HiveDriver.logfile + "-input", line);
                }
                yield return line;
            }

        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
