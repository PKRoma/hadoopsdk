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
using System.IO;

namespace Microsoft.Hadoop.MapReduce
{
    /// <summary>
    /// Utilities for working with HDFS paths
    /// </summary>
    public static class HdfsPath
    {
        private static char _directorySeparatorChar = '/';

        /// <summary>
        /// Gets or sets the directory separator char.
        /// </summary>
        /// <value>
        /// The directory separator char.
        /// </value>
        public static char DirectorySeparatorChar
        {
            get { return _directorySeparatorChar; }
            set { _directorySeparatorChar = value; }
        }


        /// <summary>
        /// Combines two HDFS path components.
        /// </summary>
        /// <param name="path1">Path component 1.</param>
        /// <param name="path2">Path component 2.</param>
        /// <returns>Combined path</returns>
        public static string Combine(string path1, string path2)
        {
            //@@TODO: check for invalid chars.
            //@@TODO: check if path2 is rooted.  for this api it would most likely yield an exception.

            if (path2.Length == 0)
            {
                return path1;
            }
            if (path1.Length == 0)
            {
                return path2;
            }
            
            char c = path1[path1.Length - 1];
            if (c != DirectorySeparatorChar)
            {
                return path1 + DirectorySeparatorChar + path2;
            }
            return path1 + path2;
        }
    }
}
