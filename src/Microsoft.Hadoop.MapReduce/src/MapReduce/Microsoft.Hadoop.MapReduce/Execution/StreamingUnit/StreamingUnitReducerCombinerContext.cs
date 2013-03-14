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
    /// Context for Local execution of a reducer/combiner
    /// </summary>
    public class StreamingUnitReducerCombinerContext : ReducerCombinerContext
    {
        private StreamingUnitOutput _outputCollector;
        private bool _isCombiner;

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamingUnitReducerCombinerContext"/> class.
        /// </summary>
        /// <param name="outputCollector">The output collector.</param>
        /// <param name="isCombiner">Indicates whether this instance is for a combiner.</param>
        public StreamingUnitReducerCombinerContext(StreamingUnitOutput outputCollector, bool isCombiner) 
            : base(isCombiner)
        {
            _outputCollector = outputCollector;
            _isCombiner = isCombiner;
        }
       
        /// <summary>
        /// Emits a key/value pair.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public override void EmitKeyValue(string key, string value)
        {
            EmitLine(string.Format("{0}\t{1}", key, value));
        }

        /// <summary>
        /// Emits a raw text line.
        /// </summary>
        /// <param name="line"></param>
        /// <remarks>
        /// </remarks>
        public override void EmitLine(string line)
        {
            (_isCombiner ? _outputCollector.CombinerResult : _outputCollector.ReducerResult).Add(line);
        }

        /// <summary>
        /// Increments a Hadoop counter.
        /// </summary>
        /// <param name="counterName">Name of the counter.</param>
        public override void IncrementCounter(string counterName)
        {
            IncrementCounter(ContextBase.DEFAULT_COUNTER_CATEGORY, counterName, 1);
        }

        /// <summary>
        /// Increments a Hadoop counter.
        /// </summary>
        /// <param name="counterName">Name of the counter.</param>
        /// <param name="increment">The increment amount.</param>
        /// ///
        public override void IncrementCounter(string counterName, int increment)
        {
            IncrementCounter(ContextBase.DEFAULT_COUNTER_CATEGORY, counterName, increment);
        }

        /// <summary>
        /// Increments a Hadoop counter.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <param name="counterName">Name of the counter.</param>
        /// <param name="increment">The increment amount.</param>
        public override void IncrementCounter(string category, string counterName, int increment)
        {
            (_isCombiner ? _outputCollector.CombinerCounters : _outputCollector.ReducerCounters).IncrementCounter(category + "|" + counterName, 1);
        }

        /// <summary>
        /// Writes a message to the Hadoop task log.
        /// </summary>
        /// <param name="message">The message.</param>
        public override void Log(string message)
        {
            (_isCombiner ? _outputCollector.CombinerLog : _outputCollector.ReducerLog).Add(message);
        }
    }
}
