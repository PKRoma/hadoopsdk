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
    /// <summary>
    /// Enables execution of a StreamingJob
    /// </summary>
    public static class StreamingUnit
    {
        /// <summary>
        /// Executes Mapper with the specified input.
        /// </summary>
        /// <typeparam name="TMapper">The type of the mapper.</typeparam>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        public static StreamingUnitOutput Execute<TMapper>(IEnumerable<string> input) 
            where TMapper : MapperBase, new()
        {
            return ExecuteCore<TMapper, NullReducerCombiner, NullReducerCombiner>(input, new StreamingUnitOptions());
        }

        /// <summary>
        /// Executes Mapper with the specified input.
        /// </summary>
        /// <typeparam name="TMapper">The type of the mapper.</typeparam>
        /// <param name="input">The input.</param>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        public static StreamingUnitOutput Execute<TMapper>(IEnumerable<string> input, StreamingUnitOptions options) 
            where TMapper : MapperBase, new()
        {
            return ExecuteCore<TMapper, NullReducerCombiner, NullReducerCombiner>(input, options);
        }

        /// <summary>
        /// Executes Mapper and Reducer with the specified input.
        /// </summary>
        /// <typeparam name="TMapper">The type of the mapper.</typeparam>
        /// <typeparam name="TReducer">The type of the reducer.</typeparam>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        public static StreamingUnitOutput Execute<TMapper, TReducer>(IEnumerable<string> input) 
            where TMapper : MapperBase, new()
            where TReducer : ReducerCombinerBase, new()
        {
            return ExecuteCore<TMapper, NullReducerCombiner, TReducer>(input, new StreamingUnitOptions());
        }


        /// <summary>
        /// Executes Mapper and Reducer with the specified input.
        /// </summary>
        /// <typeparam name="TMapper">The type of the mapper.</typeparam>
        /// <typeparam name="TReducer">The type of the reducer.</typeparam>
        /// <param name="input">The input.</param>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        public static StreamingUnitOutput Execute<TMapper, TReducer>(IEnumerable<string> input, StreamingUnitOptions options) 
            where TMapper : MapperBase, new()
            where TReducer : ReducerCombinerBase, new()
        {
            return ExecuteCore<TMapper, NullReducerCombiner, TReducer>(input, options);
        }

        /// <summary>
        /// Executes Mapper, Combiner and Reducer with the specified input.
        /// </summary>
        /// <typeparam name="TMapper">The type of the mapper.</typeparam>
        /// <typeparam name="TCombiner">The type of the combiner.</typeparam>
        /// <typeparam name="TReducer">The type of the reducer.</typeparam>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        public static StreamingUnitOutput Execute<TMapper, TCombiner, TReducer>(IEnumerable<string> input)
            where TMapper : MapperBase, new()
            where TCombiner : ReducerCombinerBase, new()
            where TReducer : ReducerCombinerBase, new()
        {
            return ExecuteCore<TMapper, TCombiner, TReducer>(input, new StreamingUnitOptions());
        }


        /// <summary>
        /// Executes Mapper, Combiner and Reducer with the specified input.
        /// </summary>
        /// <typeparam name="TMapper">The type of the mapper.</typeparam>
        /// <typeparam name="TCombiner">The type of the combiner.</typeparam>
        /// <typeparam name="TReducer">The type of the reducer.</typeparam>
        /// <param name="input">The input.</param>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        public static StreamingUnitOutput Execute<TMapper,TCombiner, TReducer>(IEnumerable<string> input, StreamingUnitOptions options) 
            where TMapper : MapperBase, new()
            where TCombiner : ReducerCombinerBase, new()
            where TReducer : ReducerCombinerBase, new()
        {
            return ExecuteCore<TMapper, TCombiner, TReducer>(input, options);
        }

        
        internal static StreamingUnitOutput ExecuteCore<TMapper, TCombiner, TReducer>(IEnumerable<string> input, StreamingUnitOptions options)
            where TMapper: MapperBase, new()
            where TCombiner: ReducerCombinerBase, new()
            where TReducer: ReducerCombinerBase, new()
        {
            StreamingUnitOutput outputCollector = new StreamingUnitOutput();
            List<string> currOutput = null;
            //Run Mapper
            StreamingUnitMapperContext mapperContext = new StreamingUnitMapperContext(outputCollector);
            mapperContext.InputPartitionId = "0";
            TypeSystem.CheckUserType(typeof(TMapper), typeof(MapperBase));
            TMapper mapper = new TMapper();
            MapperMain.Process(mapper, input, mapperContext);

            currOutput = outputCollector.MapperResult;

            //Run Combiner
            //@@NOTE: streaming combiner usage might be largely ineffectual.. no sorting grouping takes place so groups passed to combiner are ofter 1-element long.
            //        does hadoop streaming do any sort/collation before calling the combiner exe?
            if (typeof(TCombiner) != typeof(NullReducerCombiner))
            {
                StreamingUnitReducerCombinerContext combinerContext = new StreamingUnitReducerCombinerContext(outputCollector, true);

                TypeSystem.CheckUserType(typeof(TCombiner), typeof(ReducerCombinerBase));
                TCombiner combiner = new TCombiner();
                ReducerCombinerMain.Process(combiner, currOutput, options.SortKeyColumnCount, combinerContext);
                currOutput = outputCollector.CombinerResult;
            }

            if (typeof(TReducer) != typeof(NullReducerCombiner))
            {
                // Sort the map/combiner output.
                currOutput.Sort(new MRLineComparer(options.SortKeyColumnCount));

                //Run Reducer
                StreamingUnitReducerCombinerContext reducerContext = new StreamingUnitReducerCombinerContext(outputCollector, false);
                TypeSystem.CheckUserType(typeof(TReducer), typeof(ReducerCombinerBase));
                TReducer reducer = new TReducer();
                ReducerCombinerMain.Process(reducer, currOutput, options.SortKeyColumnCount, reducerContext);
                currOutput = outputCollector.ReducerResult;
            }

            outputCollector.Result = currOutput;

            return outputCollector;
        }
    }
}
