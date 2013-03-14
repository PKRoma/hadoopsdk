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
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace IQToolkit
{
    /// <summary>
    /// Simple implementation of the IGrouping<TKey, TElement> interface
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TElement"></typeparam>
    public class Grouping<TKey, TElement> : IGrouping<TKey, TElement>
    {
        TKey key;
        IEnumerable<TElement> group;

        public Grouping(TKey key, IEnumerable<TElement> group)
        {
            this.key = key;
            this.group = group;
        }

        public TKey Key
        {
            get { return this.key; }
        }

        public IEnumerator<TElement> GetEnumerator()
        {
            if (!(group is List<TElement>))
                group = group.ToList();
            return this.group.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.group.GetEnumerator();
        }
    }   
}
