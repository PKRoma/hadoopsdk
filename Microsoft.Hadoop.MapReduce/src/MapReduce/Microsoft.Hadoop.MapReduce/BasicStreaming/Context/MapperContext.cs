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
    /// Base class for Mapper context
    /// </summary>
    /// <remarks>
    /// Provides runtime functionality for IMapper.
    /// </remarks>
    public abstract class MapperContext : ContextBase
    {
        /// <summary>
        /// Gets the name of the input filename.
        /// </summary>
        public abstract string InputFilename { get; }

        /// <summary>
        /// Gets the partition id for this input.
        /// </summary>
        /// <remarks>
        /// If a single input file is split into multiple pieces, this property can be used to differentiate the parts.
        /// This can be useful for constructing names when generating external entities such as HDFS files.
        /// </remarks>
        /// <example>
        /// <code source="DocCodeSnippets/Snippets1.cs" region="Snippet.MapperContext.InputPartitionId" lang="C#" title="Constructing an unique external name via InputPartitionId" />
        /// </example>
        public abstract string InputPartitionId { get; set; }
       
    }
}
