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

namespace IQToolkit
{
    public class CompoundKey : IEquatable<CompoundKey>, IEnumerable<object>, IEnumerable
    {
        object[] values;
        int hc;

        public CompoundKey(params object[] values)
        {
            this.values = values;
            for (int i = 0, n = values.Length; i < n; i++)
            {
                object value = values[i];
                if (value != null)
                {
                    hc ^= (value.GetHashCode() + i);
                }
            }
        }

        public override int GetHashCode()
        {
            return hc;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public bool Equals(CompoundKey other)
        {
            if (other == null || other.values.Length != values.Length)
                return false;
            for (int i = 0, n = other.values.Length; i < n; i++)
            {
                if (!object.Equals(this.values[i], other.values[i]))
                    return false;
            }
            return true;
        }

        public IEnumerator<object> GetEnumerator()
        {
            return ((IEnumerable<object>)values).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
