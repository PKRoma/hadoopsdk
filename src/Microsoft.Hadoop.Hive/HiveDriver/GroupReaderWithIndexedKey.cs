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
using System.Globalization;
using System.Linq;
using System.Text;

namespace Microsoft.Hadoop.Hive
{
    using System.Diagnostics.CodeAnalysis;

    // Enumerates input and collates batches of lines into an IGrouping based on common key field. 
    // The key columns are identified by column indexes.
    // Only contiguous lines with a common key will join a group.. If there are islands with common key, 
    //   there will be multiple groups produced for that key.
    //
    // !! This class is fragile when the input is read-once such as Stdin.
    //    ie debugger view/watch, multiple reads can destabilize it.
    public class Grouper
    {
        internal string _currGroupKey; // the key associated with the most recent group
        internal string _currInputKey;
        internal string _currInputValue;

        private bool _done;
        private int[] _keyFieldIndexes = null;
        private IEnumerator<string> _inputEnumerator;

        public Grouper(int[] keyFieldIndexes, IEnumerable<string> inputEnumerable)
        {
            if (ReferenceEquals(inputEnumerable, null))
            {
                throw new ArgumentNullException("inputEnumerable");
            }
            _keyFieldIndexes = keyFieldIndexes;
            _inputEnumerator = inputEnumerable.GetEnumerator();
        }

        public IGrouping<string, string> NextGroup()
        {
            // s_currKey can be null at the start of execution and at the end
            // we use s_done to differentiate between the two.

            if (!_done)
            {
                // do one readline to kick things off.. Group.GetEnumerator() will take care of the rest.
                if (_currInputKey == null)
                {
                    ReadNextInputLine();
                    if (_done) { return null; }
                    _currGroupKey = _currInputKey;
                }
                else
                {
                    //fast-forward if the last group wasn't fully enumerated
                    while (_currInputKey == _currGroupKey)
                    {
                        ReadNextInputLine();
                        if (_done) { return null; }
                    }
                    _currGroupKey = _currInputKey;
                }

                return new GrouperGrouping(this, _currInputKey, _currInputValue);
            }
            else
            {
                return null;
            }
        }

        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "sStreamNumMapOutputKeyFields",
            Justification = "suppressing for now as this is not user facing. [tgs]")]
        public void ReadNextInputLine()
        {
            bool ok = _inputEnumerator.MoveNext();
            if (ok)
            {
                string line = _inputEnumerator.Current;
                string[] lineParts = line.Split('\t'); //@@TODO: configurable separator.
                if (lineParts.Length < _keyFieldIndexes.Length)
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Input line to reducer does not have enough parts. s_Stream_Num_Map_Output_Key_Fields={0}. Line={1}", _keyFieldIndexes.Length, line));
                }
                _currInputKey = string.Join("\t", _keyFieldIndexes.Select(index => lineParts[index]));
                _currInputValue = line;
            }
            else
            {
                _currInputKey = null;
                _currInputValue = null;
                _done = true;
            }
        }
    }

    internal class GrouperGrouping : IGrouping<string, string>
    {

        private string _key;
        private bool _getEnumeratorCalled = false;
        private Grouper _parentGrouper;
        private string _currValue;

        internal GrouperGrouping(Grouper parentGrouper, string key, string value)
        {
            _parentGrouper = parentGrouper;
            _key = key;
            _currValue = value;
        }

        public string Key
        {
            get { return _key; }
        }

        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "ToArray",
            Justification = "Suppressing for now as this is developer facing. [tgs]")]
        public IEnumerator<string> GetEnumerator()
        {
            if (_getEnumeratorCalled)
            {
                throw new InvalidOperationException(
                    "Parameter 'values' can only be enumerated once. To enumerate it multiple times, collect the list into a buffer. " +
                    "For example, var results = values.ToArray();");
            }
            _getEnumeratorCalled = true;
            do
            {
                yield return _currValue;
                _parentGrouper.ReadNextInputLine();
                _currValue = _parentGrouper._currInputValue;
            } while (_parentGrouper._currInputKey == _key);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
