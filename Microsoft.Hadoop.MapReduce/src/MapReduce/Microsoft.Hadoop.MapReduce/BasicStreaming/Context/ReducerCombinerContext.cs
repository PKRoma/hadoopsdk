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
    /// Base class for ReducerCombiner context
    /// </summary>
    /// <remarks>
    /// Provides runtime functionality for IReducer.
    /// </remarks>
    public abstract class ReducerCombinerContext : ContextBase
    {
        private bool _isCombiner;

        /// <summary>
        /// Gets or sets a value indicating whether this instance is combiner.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is for a combiner; otherwise, <c>false</c>.
        /// </value>
        public bool IsCombiner
        {
            get { return _isCombiner; }
            set { _isCombiner = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReducerCombinerContext"/> class.
        /// </summary>
        /// <param name="isCombiner">Indicates whether this instance is for a combiner.</param>
        public ReducerCombinerContext(bool isCombiner)
        {
            _isCombiner = isCombiner;
        }
        
        /// <summary>
        /// Emits a key/value pair.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public override abstract void EmitKeyValue(string key, string value);

        /// <summary>
        /// Emits a raw text line.
        /// </summary>
        /// <param name="line"></param>
        /// <remarks>
        /// </remarks>
        public override abstract void EmitLine(string line);

        /// <summary>
        /// Increments a Hadoop counter.
        /// </summary>
        /// <param name="counterName">Name of the counter.</param>
        public abstract override void IncrementCounter(string counterName);
    }
}
