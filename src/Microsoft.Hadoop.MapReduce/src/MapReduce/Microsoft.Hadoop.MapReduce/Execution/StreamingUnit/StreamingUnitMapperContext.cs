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
    /// Context for Local execution of a Mapper
    /// </summary>
    public class StreamingUnitMapperContext : MapperContext
    {
        private List<string> _output = new List<string>();
        private string _inputPartitionId = "0";
        private StreamingUnitOutput _outputCollector;

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamingUnitMapperContext"/> class.
        /// </summary>
        /// <param name="outputCollector">The output collector.</param>
        public StreamingUnitMapperContext(StreamingUnitOutput outputCollector)
        {
            _outputCollector = outputCollector;
        }

        /// <summary>
        /// Gets the name of the input filename.
        /// </summary>
        public override string InputFilename
        {
            get
            {
                return "inproc";
            }
        }

        /// <summary>
        /// Gets the partition id for this input.
        /// </summary>
        /// <remarks>
        /// If a single input file is split into multiple pieces, this property can be used to differentiate the parts.
        /// This can be useful for constructing names when generating external entities such as HDFS files.
        /// </remarks>
        /// <example>
        /// <code source="DocCodeSnippets/Snippets1.cs" region="Snippet.MapperContext.InputPartitionId" lang="C#" title="Constructing an unique external name via InputPartitionId" />
        /// </example>
        public override string InputPartitionId
        {
            get
            {
                return _inputPartitionId;
            }
            set
            {
                _inputPartitionId = value;
            }
        }

        /// <summary>
        /// Increments a Hadoop counter.
        /// </summary>
        /// <param name="counterName">Name of the counter.</param>
        public override void IncrementCounter(string counterName)
        {
            _outputCollector.MapperCounters.IncrementCounter(ContextBase.DEFAULT_COUNTER_CATEGORY + "|" + counterName, 1);
        }

        /// <summary>
        /// Increments a Hadoop counter.
        /// </summary>
        /// <param name="counterName">Name of the counter.</param>
        /// <param name="increment">The increment amount.</param>
        /// ///
        public override void IncrementCounter(string counterName, int increment)
        {
            _outputCollector.MapperCounters.IncrementCounter(ContextBase.DEFAULT_COUNTER_CATEGORY + "|" + counterName, increment);
        }

        /// <summary>
        /// Increments a Hadoop counter.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <param name="counterName">Name of the counter.</param>
        /// <param name="increment">The increment amount.</param>
        public override void IncrementCounter(string category, string counterName, int increment)
        {
            _outputCollector.MapperCounters.IncrementCounter(category + "|" + counterName, 1);
        }

        /// <summary>
        /// Writes a message to the Hadoop task log.
        /// </summary>
        /// <param name="message">The message.</param>
        public override void Log(string message)
        {
            _outputCollector.MapperLog.Add(message);
        }

        /// <summary>
        /// Emits a raw text line.
        /// </summary>
        /// <param name="line"></param>
        public override void EmitLine(string line)
        {
            _outputCollector.MapperResult.Add(line);
        }

        /// <summary>
        /// Emits a key/value pair
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public override void EmitKeyValue(string key, string value)
        {
            _outputCollector.MapperResult.Add(string.Format("{0}\t{1}", key, value));
        }
    }
}
