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
    /// Context for Hadoop execution of a Reducer/Combiner
    /// </summary>
    public class HadoopReducerCombinerContext : ReducerCombinerContext
    {
        private const string CUSTOM_CATEGORY_NAME = "Custom";

        /// <summary>
        /// Initializes a new instance of the <see cref="HadoopReducerCombinerContext"/> class.
        /// </summary>
        /// <param name="isCombiner">Indicates whether this instance is for a combiner.</param>
        public HadoopReducerCombinerContext(bool isCombiner)
            : base(isCombiner)
        {
        }

        /// <summary>
        /// Emits a key/value pair.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public override void EmitKeyValue(string key, string value)
        {
            Console.WriteLine("{0}\t{1}", key, value);
        }

        /// <summary>
        /// Emits a raw text line.
        /// </summary>
        /// <param name="line">The line.</param>
        public override void EmitLine(string line)
        {
            Console.WriteLine(line);
        }

        /// <summary>
        /// Increments a Hadoop counter.
        /// </summary>
        /// <param name="counterName">Name of the counter.</param>
        public override void IncrementCounter(string counterName)
        {
            Console.Error.WriteLine("reporter:counter:{0},{1},{2}", CUSTOM_CATEGORY_NAME, counterName, 1);
        }

        /// <summary>
        /// Increments a Hadoop counter.
        /// </summary>
        /// <param name="counterName">Name of the counter.</param>
        /// <param name="increment">The increment amount.</param>
        /// ///
        public override void IncrementCounter(string counterName, int increment)
        {
            Console.Error.WriteLine("reporter:counter:{0},{1},{2}", CUSTOM_CATEGORY_NAME, counterName, increment);
        }

        /// <summary>
        /// Increments a Hadoop counter.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <param name="counterName">Name of the counter.</param>
        /// <param name="increment">The increment amount.</param>
        public override void IncrementCounter(string category, string counterName, int increment)
        {
            Console.Error.WriteLine("reporter:counter:{0},{1},{2}", category, counterName, increment);
        }

        /// <summary>
        /// Writes a message to the Hadoop task log.
        /// </summary>
        /// <param name="message">The message.</param>
        public override void Log(string message)
        {
            Console.Error.WriteLine(message);
        }
    }
}
