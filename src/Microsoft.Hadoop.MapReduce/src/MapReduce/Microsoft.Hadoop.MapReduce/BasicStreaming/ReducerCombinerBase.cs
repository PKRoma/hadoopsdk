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
    /// Base class for a reducer/combiner 
    /// </summary>
    /// <remarks>
    /// A reducer receives all data for a group and typically emits one key/value pair per group.
    /// Each group comprises all the records that share a key.
    /// </remarks>
    public abstract class ReducerCombinerBase
    {
        /// <summary>
        /// Initializes the reducer
        /// </summary>
        /// <param name="context">The context.</param>
        public virtual void Initialize(ReducerCombinerContext context)
        {

        }

        /// <summary>
        /// Reduces the specified group.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="values">The values.</param>
       /// <param name="context">The context.</param>
        public abstract void Reduce(string key, IEnumerable<string> values, ReducerCombinerContext context);

        /// <summary>
        /// Clean up the reducer.
        /// </summary>
        /// <param name="context">The context.</param>
        public virtual void Cleanup(ReducerCombinerContext context)
        {

        }
    }
}
