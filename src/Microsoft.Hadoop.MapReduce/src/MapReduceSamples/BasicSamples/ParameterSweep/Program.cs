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
using System.Windows.Forms;
using System.Diagnostics;

namespace ParameterSweep
{
    using System.Globalization;

    public class Program
    {
        public static void Main(string[] args)
        {
            var hadoop = Hadoop.Connect();
            //create input
            hadoop.StorageSystem.WriteAllLines(ParameterSweepJob.s_inputFileHDFS, Enumerable.Range(0, 10).Select(x => x.ToString()));

            //run
            var result = hadoop.MapReduceJob.ExecuteJob<ParameterSweepJob>();

            foreach (string line in hadoop.StorageSystem.EnumerateDataInFolder(ParameterSweepJob.s_outputFolderHDFS, 20))
            {
                Console.WriteLine(line);
            }
        }
    }

    public class ParameterSweepJob : HadoopJob<ParameterSweepMapper>
    {
        public static string s_inputFolderHDFS = "input/ParameterSweep";
        public static string s_inputFileHDFS = "input/ParameterSweep/input0.txt";
        public static string s_outputFolderHDFS = "output/ParameterSweep";

        public override HadoopJobConfiguration Configure(ExecutorContext context)
        {
            HadoopJobConfiguration config = new HadoopJobConfiguration();
            config.InputPath = s_inputFileHDFS;
            config.OutputFolder = s_outputFolderHDFS;
            return config;
        }
    }

    public class ParameterSweepMapper : MapperBase
    {
        public override void Map(string inputLine, MapperContext context)
        {
            int inputValue = int.Parse(inputLine);

            // Perform the work.
            double sqrt = Math.Sqrt((double)inputValue);

            // Write true output to HDFS manually 
            string outputFileHdfs = HdfsPath.Combine(ParameterSweepJob.s_outputFolderHDFS, inputValue.ToString());
            Hadoop.Connect().StorageSystem.WriteAllLines(outputFileHdfs, new[] { string.Format("{0}\t{1}", inputValue, sqrt) });

            // Write summary info to the Mapper output.
            context.EmitKeyValue(inputValue.ToString(), outputFileHdfs);
        }
    }
}

