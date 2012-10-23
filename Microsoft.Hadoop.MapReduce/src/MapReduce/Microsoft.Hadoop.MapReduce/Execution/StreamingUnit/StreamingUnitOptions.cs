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
    /// Configuration for local execution.
    /// </summary>
    public class StreamingUnitOptions
    {
        private int _sortKeyColumnCount = 1;

        /// <summary>
        /// Gets or sets the sort key column count.
        /// </summary>
        /// <value>
        /// The sort key column count.
        /// </value>
        public int SortKeyColumnCount
        {
            get { return _sortKeyColumnCount; }
            set { _sortKeyColumnCount = value; }
        }
    }
}
