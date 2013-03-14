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


using Microsoft.Hadoop.MapReduce;
using Microsoft.Hadoop.MapReduce.Json;
using Newtonsoft.Json;
using System; 
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;


namespace JsonEncodedValues
{
    using Microsoft.Hadoop.MapReduce.HdfsExtras.Hdfs;

    public class Program
    {
        private static IHadoop hadoop = Hadoop.Connect();

        public static void Main(string[] args)
        {
            CreateInput();
            hadoop.MapReduceJob.ExecuteJob<JsonEncodedValuesJob>();
            foreach (string line in HdfsFile.EnumerateDataInFolder(JsonEncodedValuesJob.s_outputFolderHDFS, 20))
            {
                Console.WriteLine(line);
            }
        }

        private static void CreateInput()
        {
            
            if (hadoop.StorageSystem.Exists(JsonEncodedValuesJob.s_inputFolderHDFS))
            {
                hadoop.StorageSystem.Delete(JsonEncodedValuesJob.s_inputFolderHDFS);
            }

            hadoop.StorageSystem.MakeDirectory(JsonEncodedValuesJob.s_inputFolderHDFS);
            hadoop.StorageSystem.WriteAllLines(JsonEncodedValuesJob.s_inputFileHDFS, new string[] { "Eastern Standard Time", "Pacific Standard Time" });
        }
    }

    public class JsonEncodedValuesJob : HadoopJob<MyMapper, MyReducer>
    {
        public static string s_inputFolderHDFS = "input/JsonEncodedValues";
        public static string s_inputFileHDFS = "input/JsonEncodedValues/input0.txt";
        public static string s_outputFolderHDFS = "output/JsonEncodedValues";

        public override HadoopJobConfiguration Configure(ExecutorContext context)
        {
            HadoopJobConfiguration config = new HadoopJobConfiguration();
            config.InputPath = s_inputFolderHDFS;
            config.OutputFolder = s_outputFolderHDFS;
            return config;
        }
    }

    public class MyMapper : JsonOutMapperBase<TimeZoneInfo>
    {
        public override void Map(string valueIn, JsonMapperContext<TimeZoneInfo> context)
        {
            string zone = valueIn.Trim();
            TimeZoneInfo info = TimeZoneInfo.FindSystemTimeZoneById(zone);
            context.EmitKeyValue(info.IsDaylightSavingTime(DateTime.Now).ToString(), info);
        }
    }

    public class MyReducer : JsonInReducerCombinerBase<TimeZoneInfo>
    {
        public override void Reduce(string key, IEnumerable<TimeZoneInfo> values, ReducerCombinerContext context)
        {
            string result = string.Join("\t", values.Select(tz => tz.DisplayName));
            context.EmitKeyValue(key, result);
        }
    }
}
