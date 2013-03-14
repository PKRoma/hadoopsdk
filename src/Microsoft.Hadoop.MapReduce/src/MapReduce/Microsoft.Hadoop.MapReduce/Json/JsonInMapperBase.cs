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
    /// Base class for a mapper that reads in JSON-encoded records and emits plain string values.
    /// </summary>
    /// <typeparam name="TValueIn">The type of the value in.</typeparam>
    public abstract class JsonInMapperBase<TValueIn> : MapperBase
    {
        /// <summary>
        /// Maps the specified input line.
        /// </summary>
        /// <param name="inputLine">The input line.</param>
        /// <param name="context">The context.</param>
        /// <remarks>
        /// This method delegates to a more specialized map method.
        /// </remarks>
        public sealed override void Map(string inputLine, MapperContext context)
        {
            TValueIn valueIn = JsonConvert.DeserializeObject<TValueIn>(inputLine);
            Map(valueIn, context);
        }


        /// <summary>
        /// Maps the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="context">The context.</param>
        public abstract void Map(TValueIn value, MapperContext context);
    }
}
