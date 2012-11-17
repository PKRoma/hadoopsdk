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
using System.Threading;

namespace IQToolkit
{
    public static class TopologicalSorter
    {
        public static IEnumerable<T> Sort<T>(this IEnumerable<T> items, Func<T, IEnumerable<T>> fnItemsBeforeMe)
        {
            return Sort<T>(items, fnItemsBeforeMe, null);
        }

        public static IEnumerable<T> Sort<T>(this IEnumerable<T> items, Func<T, IEnumerable<T>> fnItemsBeforeMe, IEqualityComparer<T> comparer)
        {
            HashSet<T> seen = comparer != null ? new HashSet<T>(comparer) : new HashSet<T>();
            HashSet<T> done = comparer != null ? new HashSet<T>(comparer) : new HashSet<T>();
            List<T> result = new List<T>();
            foreach (var item in items)
            {
                SortItem(item, fnItemsBeforeMe, seen, done, result);
            }
            return result;
        }

        private static void SortItem<T>(T item, Func<T, IEnumerable<T>> fnItemsBeforeMe, HashSet<T> seen, HashSet<T> done, List<T> result)
        {
            if (!done.Contains(item))
            {
                if (seen.Contains(item))
                {
                    throw new InvalidOperationException("Cycle in topological sort");
                }
                seen.Add(item);
                var itemsBefore = fnItemsBeforeMe(item);
                if (itemsBefore != null)
                {
                    foreach (var itemBefore in itemsBefore)
                    {
                        SortItem(itemBefore, fnItemsBeforeMe, seen, done, result);
                    }
                }
                result.Add(item);
                done.Add(item);
            }
        }
    }
}
