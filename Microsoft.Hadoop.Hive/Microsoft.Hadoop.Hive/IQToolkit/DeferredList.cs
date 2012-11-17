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
    /// <summary>
    /// Common interface for controlling defer-loadable types
    /// </summary>
    public interface IDeferLoadable
    {
        bool IsLoaded { get; }
        void Load();
    }

    public interface IDeferredList : IList, IDeferLoadable
    {
    }

    public interface IDeferredList<T> : IList<T>, IDeferredList
    {
    }

    /// <summary>
    /// A list implementation that is loaded the first time the contents are examined
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DeferredList<T> : IDeferredList<T>, ICollection<T>, IEnumerable<T>, IList, ICollection, IEnumerable, IDeferLoadable
    {
        IEnumerable<T> source;
        List<T> values;

        public DeferredList(IEnumerable<T> source)
        {
            this.source = source;
        }

        public void Load()
        {
            this.values = new List<T>(this.source);
        }

        public bool IsLoaded
        {
            get { return this.values != null; }
        }

        private void Check()
        {
            if (!this.IsLoaded)
            {
                this.Load();
            }
        }

        #region IList<T> Members

        public int IndexOf(T item)
        {
            this.Check();
            return this.values.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            this.Check();
            this.values.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            this.Check();
            this.values.RemoveAt(index);
        }

        public T this[int index]
        {
            get
            {
                this.Check();
                return this.values[index];
            }
            set
            {
                this.Check();
                this.values[index] = value;
            }
        }

        #endregion

        #region ICollection<T> Members

        public void Add(T item)
        {
            this.Check();
            this.values.Add(item);
        }

        public void Clear()
        {
            this.Check();
            this.values.Clear();
        }

        public bool Contains(T item)
        {
            this.Check();
            return this.values.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            this.Check();
            this.values.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { this.Check(); return this.values.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(T item)
        {
            this.Check();
            return this.values.Remove(item);
        }

        #endregion

        #region IEnumerable<T> Members

        public IEnumerator<T> GetEnumerator()
        {
            this.Check();
            return this.values.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        #region IList Members

        public int Add(object value)
        {
            this.Check();
            return ((IList)this.values).Add(value);
        }

        public bool Contains(object value)
        {
            this.Check();
            return ((IList)this.values).Contains(value);
        }

        public int IndexOf(object value)
        {
            this.Check();
            return ((IList)this.values).IndexOf(value);
        }

        public void Insert(int index, object value)
        {
            this.Check();
            ((IList)this.values).Insert(index, value);
        }

        public bool IsFixedSize
        {
            get { return false; }
        }

        public void Remove(object value)
        {
            this.Check();
            ((IList)this.values).Remove(value);
        }

        object IList.this[int index]
        {
            get
            {
                this.Check();
                return ((IList)this.values)[index];
            }
            set
            {
                this.Check();
                ((IList)this.values)[index] = value;
            }
        }

        #endregion

        #region ICollection Members

        public void CopyTo(Array array, int index)
        {
            this.Check();
            ((IList)this.values).CopyTo(array, index);
        }

        public bool IsSynchronized
        {
            get { return false; }
        }

        public object SyncRoot
        {
            get { return null; }
        }

        #endregion
    }
}
