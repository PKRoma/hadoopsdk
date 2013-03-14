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
    /// Context object for a JSON mapper.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    public class JsonMapperContext<TValue>
    {
        /// <summary>
        /// Gets the core context.
        /// </summary>
        public MapperContext CoreContext {get; private set;}

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonMapperContext&lt;TValue&gt;"/> class.
        /// </summary>
        /// <param name="coreContext">The core context.</param>
        public JsonMapperContext(MapperContext coreContext)
        {
            CoreContext = coreContext;
        }

        /// <summary>
        /// Emits a key/value pair.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void EmitKeyValue(string key, TValue value)
        {
            string valueString = JsonConvert.SerializeObject(value);
            CoreContext.EmitKeyValue(key, valueString);
        }
    }
}
