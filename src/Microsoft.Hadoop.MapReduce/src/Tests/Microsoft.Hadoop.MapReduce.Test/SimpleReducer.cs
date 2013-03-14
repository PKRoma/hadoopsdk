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
using System.ComponentModel;
using System.Linq;
using System.Text;
using Microsoft.Hadoop.MapReduce;

namespace Microsoft.Hadoop.MapReduce.Test
{
    public abstract class SimpleReducer<TKeyInput, TValueInput, TKeyOutput, TValueOutput> : ReducerCombinerBase
    {
        private static TypeConverter _inputKeyConverter = TypeDescriptor.GetConverter(typeof(TKeyInput));
        private static TypeConverter _inputValueConverter = TypeDescriptor.GetConverter(typeof(TValueInput));
        private static TypeConverter _outputKeyConverter = TypeDescriptor.GetConverter(typeof(TKeyOutput));
        private static TypeConverter _outputValueConverter = TypeDescriptor.GetConverter(typeof(TValueOutput));
        private ReducerCombinerContext _currentContext;

        protected virtual TKeyInput ParseKey(string keyString)
        {
            return (TKeyInput)_inputKeyConverter.ConvertFromInvariantString(keyString);
        }

        protected virtual TValueInput ParseValue(string valueString)
        {
            return (TValueInput)_inputValueConverter.ConvertFromInvariantString(valueString);
        }

        protected virtual string ToKeyOutputString(TKeyOutput key)
        {
            return _outputKeyConverter.ConvertToInvariantString(key);
        }

        protected virtual string ToValueOutputString(TValueOutput value)
        {
            return _outputValueConverter.ConvertToInvariantString(value);
        }

        protected abstract IEnumerable<KeyValuePair<TKeyOutput, TValueOutput>> Reduce(TKeyInput key, IEnumerable<TValueInput> values);

        public override void Reduce(string key, IEnumerable<string> group, ReducerCombinerContext context)
        {
            TKeyInput parsedKey = ParseKey(key);
            _currentContext = context;
            IEnumerable<TValueInput> parsedValues = group.Select(ParseValue);
            foreach (var output in Reduce(parsedKey, parsedValues))
            {
                context.EmitKeyValue(ToKeyOutputString(output.Key), ToValueOutputString(output.Value));
            }
            _currentContext = null;
        }

        protected void Increment(string counterName, string category = null, int increment = 1)
        {
            if (_currentContext == null)
            {
                throw new InvalidOperationException("Can't increment outside the Reduce() method");
            }
            if (category == null)
            {
                _currentContext.IncrementCounter(counterName, increment);
            }
            else
            {
                _currentContext.IncrementCounter(category, counterName, increment);
            }
        }
    }

    public class IdentityReducer<TKey, TValue> : SimpleReducer<TKey, TValue, TKey, TValue>
    {
        protected override IEnumerable<KeyValuePair<TKey, TValue>> Reduce(TKey key, IEnumerable<TValue> values)
        {
            return values.Select(v => new KeyValuePair<TKey, TValue>(key, v));
        }
    }
}
