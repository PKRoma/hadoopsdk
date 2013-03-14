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
    public struct DeferredValue<T> : IDeferLoadable
    {
        IEnumerable<T> source;
        bool loaded;
        T value;

        public DeferredValue(T value)
        {
            this.value = value;
            this.source = null;
            this.loaded = true;
        }

        public DeferredValue(IEnumerable<T> source)
        {
            this.source = source;
            this.loaded = false;
            this.value = default(T);
        }

        public void Load()
        {
            if (this.source != null)
            {
                this.value = this.source.SingleOrDefault();
                this.loaded = true;
            }
        }

        public bool IsLoaded
        {
            get { return this.loaded; }
        }

        public bool IsAssigned
        {
            get { return this.loaded && this.source == null; }
        }

        private void Check()
        {
            if (!this.IsLoaded)
            {
                this.Load();
            }
        }

        public T Value
        {
            get
            {
                this.Check();
                return this.value;
            }

            set
            {
                this.value = value;
                this.loaded = true;
                this.source = null;
            }
        }
    }
}
