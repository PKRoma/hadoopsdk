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
using System.IO;

namespace Microsoft.Hadoop.MapReduce
{
    internal static class ExecutorUtils
    {
        /// <summary>
        /// Deletes the output folder if it exists.
        /// </summary>
        internal static void DeleteOutputFolder(HadoopJobConfiguration config)
        {
            if (config.DeleteOutputFolder)
            {
                if (HdfsFile.Exists(config.OutputFolder))
                {
                    Logger.WriteLine("Output folder exists.. deleting.");
                    HdfsFile.Delete(config.OutputFolder);
                }
            }
        }

        /// <summary>
        /// Extracts the appropriate parameters to put in for the streaming job to get the appropriate sort key and shuffle key counts requested.
        /// </summary>
        /// <param name="config">The job configuraiton.</param>
        /// <param name="additionalStreamingArguments">The extracted streaming arguments.</param>
        /// <param name="additionalDefines">The extracted defines (-D) needed.</param>
        internal static void ExtractShuffleSortParameters(HadoopJobConfiguration config,
            out List<string> additionalStreamingArguments, out Dictionary<string, string> additionalDefines)
        {
            additionalStreamingArguments = new List<string>();
            additionalDefines = new Dictionary<string, string>();
            if (config.ShuffleKeyColumnCount != 1 || config.SortKeyColumnCount != 1)
            {
                if (config.SortKeyColumnCount < config.ShuffleKeyColumnCount)
                {
                    throw new StreamingException(string.Format("KeyPartsSort is less than KeyPartsShuffle.  This would produce nondeterministic groups to the reducer. KeyPartsSort:{0} KeyPartsShuffle{1}",
                        config.SortKeyColumnCount, config.ShuffleKeyColumnCount));
                }

                //partitioner settings
                additionalStreamingArguments.Add("-partitioner org.apache.hadoop.mapred.lib.KeyFieldBasedPartitioner");
                additionalDefines.Add("mapreduce.job.output.key.comparator.class", "org.apache.hadoop.mapred.lib.KeyFieldBasedComparator");
                additionalDefines.Add("mapreduce.partition.keypartitioner.options", String.Format("-k{0},{0}", config.ShuffleKeyColumnCount));

                //sorter settings
                additionalDefines.Add("stream.num.map.output.key.fields", config.SortKeyColumnCount.ToString());
                additionalDefines.Add("mapreduce.partition.keycomparator.options", String.Format("-k1,{0}n", config.SortKeyColumnCount)); // n=numeric comparison, if appropriate.
            }
        }

        /// <summary>
        /// Checks the user-defined map/reduce/combine types to make sure they're valid - and throw if not.
        /// </summary>
        /// <param name="mapperType">The mapper type.</param>
        /// <param name="combinerType">The combiner type.</param>
        /// <param name="reducerType">The reducer type.</param>
        internal static void CheckUserTypes(Type mapperType, Type combinerType, Type reducerType)
        {
            if (mapperType != null)
            {
                TypeSystem.CheckUserType(mapperType, typeof(MapperBase));
            }
            else
            {
                throw new StreamingException("Mapper cannot be null");
            }

            if (combinerType != null)
            {
                TypeSystem.CheckUserType(combinerType, typeof(ReducerCombinerBase));
            }

            if (reducerType != null)
            {
                TypeSystem.CheckUserType(reducerType, typeof(ReducerCombinerBase));
            }
        }
    }
}
