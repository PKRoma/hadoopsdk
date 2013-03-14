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
    public abstract class SimpleMapper<TInput, TKey, TValue> : MapperBase
    {
        private static TypeConverter _inputConverter = TypeDescriptor.GetConverter(typeof(TInput));
        private static TypeConverter _keyConverter = TypeDescriptor.GetConverter(typeof(TKey));
        private static TypeConverter _valueConverter = TypeDescriptor.GetConverter(typeof(TValue));
        private MapperContext _currentContext;

        protected virtual TInput Parse(string inputString)
        {
            return (TInput)_inputConverter.ConvertFromInvariantString(inputString);
        }

        protected virtual string ToKeyString(TKey key)
        {
            return _keyConverter.ConvertToInvariantString(key);
        }

        protected virtual string ToValueString(TValue value)
        {
            return _valueConverter.ConvertToInvariantString(value);
        }

        protected abstract IEnumerable<KeyValuePair<TKey, TValue>> Map(TInput input);

        public override void Map(string inputLine, MapperContext context)
        {
            TInput input = Parse(inputLine);
            _currentContext = context;
            foreach (KeyValuePair<TKey, TValue> output in Map(input))
            {
                context.EmitKeyValue(ToKeyString(output.Key), ToValueString(output.Value));
            }
            _currentContext = null;
        }

        protected void Increment(string counterName, string category = null, int increment = 1)
        {
            if (_currentContext == null)
            {
                throw new InvalidOperationException("Can't increment outside the Map() method");
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

    public class IdentityMapper<TInput> : SimpleMapper<TInput, TInput, int>
    {
        protected override IEnumerable<KeyValuePair<TInput, int>> Map(TInput input)
        {
            yield return new KeyValuePair<TInput, int>(input, 1);
        }
    }
}
