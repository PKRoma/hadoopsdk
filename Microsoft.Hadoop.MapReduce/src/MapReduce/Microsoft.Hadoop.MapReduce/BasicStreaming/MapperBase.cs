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
    /// Base class for a mapper. 
    /// </summary>
    /// <remarks>
    /// A mapper transforms input records into key/value pairs.
    /// </remarks>
    public abstract class MapperBase
    {
        /// <summary>
        /// Initialize the mapper
        /// </summary>
        /// <param name="context">The context.</param>
        public virtual void Initialize(MapperContext context)
        {

        }

        /// <summary>
        /// Maps the specified input line.
        /// </summary>
        /// <remarks>
        /// Produce output via <see cref="Microsoft.Hadoop.MapReduce.HadoopMapperContext.EmitKeyValue"/>
        /// </remarks>
        /// <example>
        /// <code source="DocCodeSnippets/Snippets1.cs" region="Snippet.IMapper.Map" lang="C#" title="IMapper.Map Example" />
        /// </example>
        /// <param name="inputLine">The input line.</param>
        /// <param name="context">The context.</param>
        public abstract void Map(string inputLine, MapperContext context);

        /// <summary>
        /// Clean up the mapper.
        /// </summary>
        /// <param name="context">The context.</param>
        public virtual void Cleanup(MapperContext context)
        {

        }
    }
}
