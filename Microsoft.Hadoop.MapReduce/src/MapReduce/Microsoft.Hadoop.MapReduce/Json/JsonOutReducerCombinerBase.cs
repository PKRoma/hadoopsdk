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
    /// Base class for a reducer/combiner that reads in plain string values and emits JSON-encoded values.
    /// </summary>
    /// <typeparam name="TValueOut">The type of the value out.</typeparam>
    public abstract class JsonOutReducerCombinerBase<TValueOut> : ReducerCombinerBase 
    {
        /// <summary>
        /// Reduces the specified group.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="values">The values.</param>
        /// <param name="context">The context.</param>
        public sealed override void Reduce(string key, IEnumerable<string> values, ReducerCombinerContext context)
        {
            JsonReducerCombinerContext<TValueOut> ctx = new JsonReducerCombinerContext<TValueOut>(context);
            Reduce(key, values, ctx);
        }

        /// <summary>
        /// Reduces a group.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="values">The values.</param>
        /// <param name="context">The context.</param>
        public abstract void Reduce(string key, IEnumerable<string> values, JsonReducerCombinerContext<TValueOut> context);
    }
}
