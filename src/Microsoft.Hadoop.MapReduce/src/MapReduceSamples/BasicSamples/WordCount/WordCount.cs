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


// An example of hadoop word count
// Input: a text file, downloaded if required from project Gutenberg during WordCount.Configure()
//
// Execution: 1. >WordCount.exe
//            OR
//            2. Can also be run via >MRRunner.exe -dll WordCount.exe

namespace WordCountSampleApplication
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Hadoop.MapReduce;
    using System.Net;
    using Microsoft.Hadoop.MapReduce.HdfsExtras.Hdfs;

    public class Driver
    {
        public static void Main(string[] args)
        {
            Uri myAzureCluster = new Uri("http://myAzureCluster");
            string myAzureUserName = "{Your Azure user name}";
            string myAzurePassword = "{Your Azure password}";

            // This is the name of the account under which Hadoop will execute jobs.
            // Normally this is just "Hadoop".
            string myHadoopUserName = "{Hadoop Account}";

            // Azure Storage Information.
            string myAzureStorageAccount = "{Your Azure Storage Account}";
            string myAzureStorageKey = "{Your Azure Storage Key}";
            string myAzureStorageContainer = "{Your Azure Storage Container}";
            bool createContinerIfNotExist = true;

            var hadoop = Hadoop.Connect(myAzureCluster,
                                        myAzureUserName,
                                        myHadoopUserName,
                                        myAzurePassword,
                                        myAzureStorageAccount,
                                        myAzureStorageKey,
                                        myAzureStorageContainer,
                                        createContinerIfNotExist);

            var result = hadoop.MapReduceJob.ExecuteJob<WordCount>();

            Console.WriteLine();
            Console.WriteLine("Job Run Information");
            Console.WriteLine();
            Console.WriteLine("Job Id: {0}", result.Id);
            Console.WriteLine("Exit Code: {0}", result.Info.ExitCode);
            Console.WriteLine("Standard Out");
            Console.WriteLine(result.Info.StandardOut);
            Console.WriteLine();
            Console.WriteLine("Standard Err");
            Console.WriteLine(result.Info.StandardError);
        }
    }
    
    public class WordCount : HadoopJob<WordCountMapper, WordCountReducer, WordCountReducer>
    {
        public static string _input1HDFS = "/input/test.txt";

        public static string s_input1WebSource = "http://www.gutenberg.org/cache/epub/11/pg11.txt";
        public static string s_outputFolderHDFS = "/output";

        public override void Initialize(ExecutorContext context)
        {
            CreateInput();
        }

        public override void Cleanup(ExecutorContext context)
        {
            foreach (string s in HdfsFile.EnumerateDataInFolder(s_outputFolderHDFS, 20))
            {
                Console.WriteLine(s);
            }
        }

        public override HadoopJobConfiguration Configure(ExecutorContext context)
        {
            HadoopJobConfiguration config = new HadoopJobConfiguration();
            config.Verbose = true;
            config.InputPath = _input1HDFS;
            config.OutputFolder = s_outputFolderHDFS;
            config.AdditionalGenericArguments.Add("-D \"mapred.map.tasks=3\""); // example of controlling arbitrary hadoop options.
            return config;
        }

        private static void CreateInput()
        {
            using (WebClient client = new WebClient())
            {
                if (!HdfsFile.Exists(_input1HDFS))
                {
                    string data1 = client.DownloadString(s_input1WebSource);
                    HdfsFile.WriteAllText(_input1HDFS, data1);
                }
            }
        }
    }

    public class WordCountMapper : MapperBase
    {
        private char[] _punctuationChars = new[] { 
            ' ', '!', '\"', '#', '$', '%', '&', '\'', '(', ')', '*', '+', ',', '-', '.', '/',   //ascii 23--47
            ':', ';', '<', '=', '>', '?', '@', '[', ']', '^', '_', '`', '{', '|', '}', '~' };   //ascii 58--64 + misc.
        
        public override void Map(string inputLine, MapperContext context)
        {
            foreach (string word in inputLine.Trim().Split(this._punctuationChars))
            {
                context.IncrementCounter("mapInputs");
                context.Log(string.Format("Map::  {0},{1}", word, "1"));
                context.EmitKeyValue(word, "1");
            }
        }
    }

    public class WordCountReducer : ReducerCombinerBase
    {
        public override void Reduce(string key, IEnumerable<string> values, ReducerCombinerContext context)
        {
            context.IncrementCounter(context.IsCombiner ? "combineInputs" : "reduceInputs");
            string sum = values.Sum(s => long.Parse(s)).ToString();
            context.Log(string.Format("Combine/Reduce::  {0},{1}", key, sum));
            context.EmitKeyValue(key, sum);
        }
    }
}

