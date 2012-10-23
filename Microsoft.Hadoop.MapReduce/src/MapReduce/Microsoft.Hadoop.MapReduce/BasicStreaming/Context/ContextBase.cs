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
    /// Base for runtime Context classes
    /// </summary>
    public abstract class ContextBase
    {
        internal const string DEFAULT_COUNTER_CATEGORY = "Custom";
        
        //public string CustomCategoryName
        //{
        //    get { return _customCategoryName; }
        //    set { _customCategoryName = value; }
        //}

        /// <summary>
        /// Increments a Hadoop counter.
        /// </summary>
        /// <param name="counterName">Name of the counter.</param>
        /// <remarks>
        /// A default category name of "Custom" is used.
        /// </remarks>
        public abstract void IncrementCounter(string counterName);

        /// <summary>
        /// Increments a Hadoop counter.
        /// </summary>
        /// <param name="counterName">Name of the counter.</param>
        /// <param name="increment">The increment amount.</param>
        /// /// <remarks>
        /// A default category name of "Custom" is used.
        /// </remarks>
        public abstract void IncrementCounter(string counterName, int increment);

        /// <summary>
        /// Increments a Hadoop counter.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <param name="counterName">Name of the counter.</param>
        /// <param name="increment">The increment amount.</param>
        public abstract void IncrementCounter(string category, string counterName, int increment);
        
        /// <summary>
        /// Writes a message to the Hadoop task log.
        /// </summary>
        /// <param name="message">The message.</param>
        public abstract void Log(string message);
     
        
        /// <summary>
        /// Emits a raw text line.
        /// </summary>
        /// <param name="line"></param>
        /// <remarks>
        /// </remarks>
        public abstract void EmitLine(string line);
        
        /// <summary>
        /// Emits a key/value pair
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public abstract void EmitKeyValue(string key, string value);
    }
}
