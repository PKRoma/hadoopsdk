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
    internal class MRLineComparer : IComparer<string>
    {
        private int _sortKeyColumnCount;

        public MRLineComparer(int sortKeyColumnCount)
        {
            _sortKeyColumnCount = sortKeyColumnCount;
        }
        
        public int Compare(string x, string y)
        {
            string[] xCols = x.Split('\t');
            string[] yCols = y.Split('\t');

            for (int i = 0; i < _sortKeyColumnCount; i++)
            {
                int cmp = string.Compare(xCols[i], yCols[i], StringComparison.Ordinal);
                if (cmp < 0)
                {
                    return -1;
                }

                if (cmp > 0)
                {
                    return +1;
                }
            }

            return 0;
        }
    }
}
