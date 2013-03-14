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

namespace Microsoft.Hadoop.MapReduce
{
    /// <summary>
    /// Captures output from a StreamingUnit execution.
    /// </summary>
    public class StreamingUnitOutput
    {
        private StreamingUnitCounterCollection _mapperCounters = new StreamingUnitCounterCollection();
        private StreamingUnitCounterCollection _combinerCounters = new StreamingUnitCounterCollection();
        private StreamingUnitCounterCollection _reducerCounters = new StreamingUnitCounterCollection();

        private List<string> _mapperLog = new List<string>();
        private List<string> _combinerLog = new List<string>();
        private List<string> _reducerLog = new List<string>();

        private List<string> _mapperResult = new List<string>();
        private List<string> _combinerResult = new List<string>();
        private List<string> _reducerOutput = new List<string>();
        private List<string> _result = null; //will point to either _mapperOutput or _reducerOutput

        /// <summary>
        /// Gets the mapper counters.
        /// </summary>
        public StreamingUnitCounterCollection MapperCounters
        {
            get { return _mapperCounters; }
        }

        /// <summary>
        /// Gets the combiner counters.
        /// </summary>
        public StreamingUnitCounterCollection CombinerCounters
        {
            get { return _combinerCounters; }
        }

        /// <summary>
        /// Gets the reducer counters.
        /// </summary>
        internal StreamingUnitCounterCollection ReducerCounters
        {
            get { return _reducerCounters; }
        }

        /// <summary>
        /// Gets the mapper log.
        /// </summary>
        public List<string> MapperLog
        {
            get { return _mapperLog; }
        }

        /// <summary>
        /// Gets the combiner log.
        /// </summary>
        public List<string> CombinerLog
        {
            get { return _combinerLog; }
        }

        /// <summary>
        /// Gets the reducer log.
        /// </summary>
        public List<string> ReducerLog
        {
            get { return _reducerLog; }
        }

        /// <summary>
        /// Gets the mapper output.
        /// </summary>
        public List<string> MapperResult
        {
            get { return _mapperResult; }
        }
        
        /// <summary>
        /// Gets the combiner output.
        /// </summary>
        public List<string> CombinerResult
        {
            get { return _combinerResult; }
        }

        /// <summary>
        /// Gets the reducer output.
        /// </summary>
        public List<string> ReducerResult
        {
            get { return _reducerOutput; }
        }

        /// <summary>
        /// Gets the output.
        /// </summary>
        public List<string> Result
        {
            get { return _result; }
            internal set { _result = value; }
        }
    }
}
