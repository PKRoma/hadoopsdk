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
using Newtonsoft.Json;

namespace Microsoft.Hadoop.MapReduce.Json
{
    /// <summary>
    /// Base class for a reducer/combiner that reads in JSON-encoded records and emits plain string values.
    /// </summary>
    /// <typeparam name="TValueIn">The type of the value in.</typeparam>
    public abstract class JsonInReducerCombinerBase<TValueIn> : ReducerCombinerBase 
    {
        /// <summary>
        /// Reduces the specified group.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="values">The values.</param>
        /// <param name="context">The context.</param>
        public sealed override void Reduce(string key, IEnumerable<string> values, ReducerCombinerContext context)
        {
            IEnumerable<TValueIn> valuesIn = values.Select(x => JsonConvert.DeserializeObject<TValueIn>(x));
            Reduce(key, valuesIn, context);
        }

        /// <summary>
        /// Reduces a group.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="values">The values.</param>
        /// <param name="context">The context.</param>
        public abstract void Reduce(string key, IEnumerable<TValueIn> values, ReducerCombinerContext context);
    }
}
