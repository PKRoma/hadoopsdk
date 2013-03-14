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
    /// Utility implementations of IReducers
    /// </summary>
    public class Reducers
    {
        /// <summary>
        /// A reducer that directly copies its input to the output.
        /// </summary>
        public class IdentityReducer : ReducerCombinerBase
        {
            /// <summary>
            /// Reduces the specified group.
            /// </summary>
            /// <param name="key">The key.</param>
            /// <param name="values">The values.</param>
            /// <param name="context">The context.</param>
            public override void Reduce(string key, IEnumerable<string> values, ReducerCombinerContext context)
            {
                string line = key + "\t" + string.Join("\t", values);
                context.EmitLine(line);
            }
        }
    }
}
