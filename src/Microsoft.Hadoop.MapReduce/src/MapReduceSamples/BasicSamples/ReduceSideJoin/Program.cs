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
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace ReduceSideJoin
{
    // Note: this example is a somewhat contrived to demonstrate some aspects of the API that support reduce-side-joins.

    // An example of 1-to-Many inner-join via the "reduce-side join" pattern.
    //
    // Input files have content like:
    //    A       {ID}      {Name}
    //    B       {ID}      {Phone}
    //..
    // The A/B designator informs the mapper what 'table' the data is coming from.
    //    If data is not available in this form, a map-only job should be used on each input set to adjust it to this format.
    // Line-separated records, tab-separated columns.
    // ID/Name/Phone are each strings.
    // There should be at most one A row for a given ID
    // The can be any number of B rows for a given ID.
    //
    // Mapper:
    //   - performs a transpose of the Side/ID columns so that the sort on 1 column will use the ID field.
    // Output from the Mapper:
    // 
    //   {ID}   {Side}   Value
    //
    // Shuffle/sort
    //   - hadoop will shuffle based on the first N columns, where N = job.ReduceSideJoin_KeyPartsCommon
    //   - hadoop will sort based on the first M columns, where M = job.ReduceSideJoin_KeyPartsTotal
    //
    // Reducer:
    //
    //  Receives groups of the form {ID,Side} -> values
    //  when an A-group is received, we just cache it away
    //  when subsequent B-group is received, we can perform the real join.
    //  The output key is different to the group key as we can now eliminate the A/B designators.

    public class Program
    {
        private static IHadoop hadoop = Hadoop.Connect();

        public static void Main(string[] args)
        {
            CreateInput();
            
            //normal way to run a job is HadoopJobExecutor.ExecuteJob<JobType>();
            //here we do it a bit more manually as an example: get a config and call HadoopJobExecutor.Execute<MapperType,ReducerType>(config);
            var config = new ReduceSideJoinJob().Configure(new ExecutorContext());
            hadoop.MapReduceJob.Execute<ReduceSideJoinMapper, ReduceSideJoinReducer>(config);

            Console.WriteLine();
            Console.WriteLine("==========================");
            Console.WriteLine("Output:");
            foreach (string s in hadoop.StorageSystem.EnumerateDataInFolder(ReduceSideJoinJob.s_OuputFolder, 20))
            {
                Console.WriteLine(s);
            }
        }

        private static void CreateInput()
        {

            if (hadoop.StorageSystem.Exists(ReduceSideJoinJob.s_InputFolder))
            {
                hadoop.StorageSystem.Delete(ReduceSideJoinJob.s_InputFolder);
            }

            hadoop.StorageSystem.MakeDirectory(ReduceSideJoinJob.s_InputFolder);
            string inputFilePath_a = HdfsPath.Combine(ReduceSideJoinJob.s_InputFolder, "a");
            hadoop.StorageSystem.WriteAllLines(inputFilePath_a, 
                new string[] { 
                    " A\t1\tMike", 
                    " A\t2\tBob", 
                    " A\t3\tFrank", 
                });

            string inputFilePath_b = HdfsPath.Combine(ReduceSideJoinJob.s_InputFolder, "b");
            hadoop.StorageSystem.WriteAllLines(inputFilePath_b,
                new string[] { 
                    " B\t3\tFrank-Home", 
                    " B\t1\tMike-Home", 
                    " B\t1\tMike-Mobile", 
                });
        }
    }

    public class ReduceSideJoinJob : HadoopJob<ReduceSideJoinMapper, ReduceSideJoinReducer>
    {
        public static string s_InputFolder = "input/reduce_side_join";
        public static string s_OuputFolder = @"output/streaming_demo/reduce_side_join";

        public override HadoopJobConfiguration Configure(ExecutorContext context)
        {
            HadoopJobConfiguration config = new HadoopJobConfiguration();
            config.InputPath = s_InputFolder;
            config.OutputFolder = s_OuputFolder;
            config.SortKeyColumnCount = 2;
            config.ShuffleKeyColumnCount = 1;

            return config;
        }
    }

    public class ReduceSideJoinMapper : MapperBase
    {
        public override void Map(string inputLine, MapperContext context)
        {
            string[] lineParts = inputLine.Trim().Split('\t');
            if (lineParts.Length < 3)
            {
                context.IncrementCounter("InputLineFormatError");
                return;
            }
            string side = lineParts[0];
            string key = lineParts[1];
            string value = string.Join("\t", lineParts.Skip(2));

            string compositeKey = string.Format("{0}\t{1}", key, side); //key1 is the real data key. key2 is the side (to permit ordering in the reducer)
            context.EmitKeyValue(compositeKey, value);
        }
    }

    public class ReduceSideJoinReducer : ReducerCombinerBase
    {
        private string _currentId;
        private string _currentName;

        public override void Reduce(string key, IEnumerable<string> values, ReducerCombinerContext context)
        {
            string[] keyParts = key.Split('\t');
            string[] valuesArray = values.ToArray(); // a group can only be enumerated once only. Stashing in an array is simplest.

            string id = keyParts[0];
            string side = keyParts[1];

            if (side == "A")
            {
                if (valuesArray.Length != 1)
                {
                    throw new Exception("only expecting one A-item per key");
                }
                _currentId = id;
                _currentName = valuesArray[0]; // stash the name for now.
            }

            if (side == "B")
            {
                if (_currentName == null)
                {
                    throw new Exception("a B-group was found without a preceeding A-group. Record ID=" + id);
                }
                string phone_numbers = string.Join("\t", valuesArray);

                context.EmitKeyValue(_currentId, string.Join("\t", _currentName, phone_numbers));
                _currentName = null; // reset the name to avoid reuse.
            }
        }
    }
}
