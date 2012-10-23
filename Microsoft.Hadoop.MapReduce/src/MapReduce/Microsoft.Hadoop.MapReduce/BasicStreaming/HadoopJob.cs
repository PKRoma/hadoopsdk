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
    /// Defines a Hadoop streaming map-reduce job
    /// </summary>
    /// <remarks>
    /// Actual jobs should implement one of the derived types.
    /// </remarks>
    public abstract class HadoopJob
    {
        /// <summary>
        /// Initialization steps for the job
        /// </summary>
        public virtual void Initialize(ExecutorContext context)
        {

        }
        
        /// <summary>
        /// Creates the job configuration
        /// </summary>
        public abstract HadoopJobConfiguration Configure(ExecutorContext context);
        
        /// <summary>
        /// Cleans up the job
        /// </summary>
        public virtual void Cleanup(ExecutorContext context)
        {

        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TMapper">The type of the mapper.</typeparam>
    public abstract class HadoopJob<TMapper> : HadoopJob
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TMapper">The type of the mapper.</typeparam>
    /// <typeparam name="TReducer">The type of the reducer.</typeparam>
    public abstract class HadoopJob<TMapper, TReducer> : HadoopJob
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TMapper">The type of the mapper.</typeparam>
    /// <typeparam name="TCombiner">The type of the combiner.</typeparam>
    /// <typeparam name="TReducer">The type of the reducer.</typeparam>
    public abstract class HadoopJob<TMapper, TCombiner, TReducer> : HadoopJob
    {
    }
}
