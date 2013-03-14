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
using Microsoft.Hadoop.MapReduce;
using System.IO;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Collections.Concurrent;

namespace Microsoft.Hadoop.MapReduce.Test
{
    /// <summary>
    /// Executes Hadoop jobs with mappers, reducers and combiners of type <see cref="SimpleMapper"/> or <see cref="SimpleReducer"/>.
    /// </summary>
    static class SimpleJobExecutor
    {
        public static IHadoop hadoop = Hadoop.Connect();
        public const string InputPath = "/streamingUnitTests/simpleStreamingIn";
        public const string OutputPath = "/streamingUnitTests/simpleStreamingOut";

        public static IEnumerable<KeyValuePair<TMapKey, TMapValue>> ExecuteMapOnly<TInput, TMapKey, TMapValue, TMapper>(IEnumerable<TInput> input, out Counters counters)
            where TMapper : SimpleMapper<TInput, TMapKey, TMapValue>, new()
        {
            return Execute<TInput, TMapKey, TMapValue>(input, () => hadoop.MapReduceJob.Execute<TMapper, IdentityReducer<TMapKey, TMapValue>>(Configuration), out counters);
        }

        public static IEnumerable<KeyValuePair<TReduceKey, TReduceValue>> ExecuteReduceOnly<TInput, TReduceKey, TReduceValue, TReducer>(IEnumerable<TInput> input, out Counters counters)
            where TReducer : SimpleReducer<TInput, int, TReduceKey, TReduceValue>, new()
        {
            return Execute<TInput, TReduceKey, TReduceValue>(input, () => hadoop.MapReduceJob.Execute<IdentityMapper<TInput>, TReducer>(Configuration), out counters);
        }

        public static IEnumerable<KeyValuePair<TReduceKey, TReduceValue>> Execute<TInput, TMapKey, TMapValue, TReduceKey, TReduceValue, TMapper, TReducer>(IEnumerable<TInput> input, out Counters counters)
            where TMapper : SimpleMapper<TInput, TMapKey, TMapValue>, new()
            where TReducer : SimpleReducer<TMapKey, TMapValue, TReduceKey, TReduceValue>, new()
        {
            return Execute<TInput, TReduceKey, TReduceValue>(input, () => hadoop.MapReduceJob.Execute<TMapper, TReducer>(Configuration), out counters);
        }

        public static IEnumerable<KeyValuePair<TReduceKey, TReduceValue>> Execute<TInput, TMapKey, TMapValue, TReduceKey, TReduceValue, TMapper, TReducer, TCombiner>(IEnumerable<TInput> input, out Counters counters)
            where TMapper : SimpleMapper<TInput, TMapKey, TMapValue>, new()
            where TReducer : SimpleReducer<TMapKey, TMapValue, TReduceKey, TReduceValue>, new()
            where TCombiner : SimpleReducer<TMapKey, TMapValue, TReduceKey, TReduceValue>, new()
        {
            return Execute<TInput, TReduceKey, TReduceValue>(input, () => hadoop.MapReduceJob.Execute<TMapper, TReducer, TCombiner>(Configuration), out counters);
        }

        private static IEnumerable<KeyValuePair<TReduceKey, TReduceValue>> Execute<TInput, TReduceKey, TReduceValue>(IEnumerable<TInput> input, Action hadoopExecution, out Counters counters)
        {
            hadoop.StorageSystem.WriteAllText(InputPath, String.Join("\n", input));
            string consoleOutput = ExecuteWithOutputRedirect(hadoopExecution);
            if (consoleOutput.Contains("Streaming Job Failed!"))
            {
                Console.WriteLine(consoleOutput);
                Console.WriteLine("All logs:\r\n" +
                    String.Join("\r\n",
                        hadoop.StorageSystem.EnumerateDataInFolder(
                            String.Join("/", OutputPath, "_logs", "history"),
                            maxLines: 5000,
                            fileNamePredicate: s => s.EndsWith(".jar"))));
                throw new UnitTestException("Job failed.");
            }
            counters = Counters.CreateFromOutputPath(OutputPath);
            return hadoop.StorageSystem
                .EnumerateDataInFolder(OutputPath, maxLines: 5000)
                .Select(s => s.Split('\t'))
                .Select(ReduceOutputParsers<TReduceKey, TReduceValue>.Parse);
        }

        private static readonly HadoopJobConfiguration Configuration = new HadoopJobConfiguration()
            {                
                InputPath = InputPath,
                OutputFolder = OutputPath,
            };

        private static string ExecuteWithOutputRedirect(Action hadoopExecution)
        {
            TextWriter oldOutput = Console.Out;
            TextWriter oldErr = Console.Error;
            try
            {
                using (StringWriter output = new StringWriter())
                using (StringWriter err = new StringWriter())
                {
                    Console.SetOut(output);
                    Console.SetError(err);
                    hadoopExecution();
                    return string.Concat(output.ToString(),(err.ToString()));
                }
            }
            finally
            {
                Console.SetOut(oldOutput);
                Console.SetError(oldErr);
            }
        }

        private static class ReduceOutputParsers<TReduceKey, TReduceValue>
        {
            private static TypeConverter _reduceKeyConverter = TypeDescriptor.GetConverter(typeof(TReduceKey));
            private static TypeConverter _reduceValueConverter = TypeDescriptor.GetConverter(typeof(TReduceValue));

            public static KeyValuePair<TReduceKey, TReduceValue> Parse(string[] pair)
            {
                return new KeyValuePair<TReduceKey, TReduceValue>(ParseKey(pair[0]), ParseValue(pair[1]));
            }

            private static TReduceKey ParseKey(string keyString)
            {
                return (TReduceKey)_reduceKeyConverter.ConvertFromInvariantString(keyString);
            }

            private static TReduceValue ParseValue(string valueString)
            {
                return (TReduceValue)_reduceValueConverter.ConvertFromInvariantString(valueString);
            }
        }
    }
}
