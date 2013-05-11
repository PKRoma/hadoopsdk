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
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Hadoop.MapReduce;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Hadoop.MapReduce.Test
{
    /// <summary>
    /// Tests basic correctness for the streaming API.
    /// </summary>
    [TestClass]
    public class BasicStreamingCorrectness
    {
        private Counters _lastCounters;

        /// <summary>
        /// Tests mapping ints.
        /// </summary>
        [TestMethod]
        public void MappingInts()
        {
            List<int> input = new List<int> { 3, 5, 7 };
            var jobOutput = SimpleJobExecutor.ExecuteMapOnly<int, int, int, Mul2Mul4Mapper>(input, out _lastCounters).OrderBy(p => p.Key).ToList();
            CollectionAssert.AreEquivalent(input.Select(i => new KeyValuePair<int, int>(i * 2, i * 4)).ToList(), jobOutput);
        }

        /// <summary>
        /// Tests reducing ints.
        /// </summary>
        [TestMethod]
        public void ReducingInts()
        {
            List<int> input = Enumerable.Range(0, 1000).Select(i => i / 10).ToList();
            var jobOutput = SimpleJobExecutor.ExecuteReduceOnly<int, int, int, CountReducer>(input, out _lastCounters).ToList();
            Assert.AreEqual(input.Distinct().Count(), jobOutput.Count);
            Assert.IsTrue(jobOutput.Select(v => v.Value).All(v => v == 10));
        }

        /// <summary>
        /// Tests a simple map-reduce job.
        /// </summary>
        [TestMethod]
        public void SimpleMapAndReduce()
        {
            List<DateTime> input = new[] { "2/2/2001", "2/3/2001", "2/4/2002", "2/2/2003", "2/3/2002" }.Select(s => DateTime.Parse(s, CultureInfo.InvariantCulture)).ToList();
            var output = SimpleJobExecutor.Execute<DateTime, int, DateTime, int, int, GetYearPart, CountByYear>(input, out _lastCounters).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            Assert.AreEqual(2, output[2001]);
            Assert.AreEqual(2, output[2002]);
            Assert.AreEqual(1, output[2003]);
            Assert.AreEqual(3, output.Keys.Count);
        }

        /// <summary>
        /// Tests a simple map-reduce job with a combiner.
        /// </summary>
        [TestMethod]
        public void SimpleMapCombineAndReduce()
        {
            List<int> input = Enumerable.Range(0, 1000).ToList();
            var output = SimpleJobExecutor.Execute<int, int, int, int, int, Div10Mapper, CountReducer, CountReducer>(input, out _lastCounters).OrderBy(p => p.Key).ToList();
            Assert.AreEqual(100, output.Count);
            Assert.IsTrue(output.Select(v => v.Value).All(v => v == 10));
        }

        /// <summary>
        /// Tests incrementing counters in the mapper and reducer
        /// </summary>
        [TestMethod]
        public void MapAndReduceWithCounters()
        {
            List<string> input = new[] { "foo", "bar", "foobar", "foo", "foo", "x", "y" }.ToList(); ;
            var jobOutput = SimpleJobExecutor.Execute<string, string, int, string, int, MapperWithCounters, ReducerWithCounters>(input, out _lastCounters);
            long mapNumFoos = _lastCounters.GetMapCounter("Custom", "NumberOfFoos");
            long reduceNumFoos = _lastCounters.GetReduceCounter("TestReduction", "TenTimesNumberOfFoos") / 10;
            long reduceNumBars = _lastCounters.GetReduceCounter("TestReduction", "FiveHundredTimesNumberOfBars") / 500;
            Assert.AreEqual(4, mapNumFoos);
            Assert.AreEqual(2, reduceNumFoos);
            Assert.AreEqual(2, reduceNumBars);
        }

        /// <summary>
        /// Tests throwing in the mapper.
        /// </summary>
        [TestMethod]
        [TestCategory("CheckIn")]
        public void ThrowInMapper()
        {
            try
            {
                SimpleJobExecutor.ExecuteMapOnly<int, int, int, ExceptionalMapper>(Enumerable.Range(1, 100), out _lastCounters);
                Assert.Fail("The previous call was expected to throw an exception but failed to do so.");
            }
            catch (StreamingException)
            {
                // TODO: The exception is correct, we should evaluate the message to ensure it is thrown in the right place.
            }
        }

        /// <summary>
        /// Tests throwing in the reducer.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(StreamingException))]
        [TestCategory("CheckIn")]
        public void ThrowInReducer()
        {
            SimpleJobExecutor.ExecuteReduceOnly<int, int, int, ExceptionalReducer>(Enumerable.Range(1, 100), out _lastCounters);
        }

        /// <summary>
        /// Tests throwing in the combiner.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(StreamingException))]
        [TestCategory("CheckIn")]
        public void ThrowInCombinerReducer()
        {
            SimpleJobExecutor.Execute<int, int, int, int, int, IdentityMapper<int>, IdentityReducer<int, int>, ExceptionalReducer>(Enumerable.Range(1, 100), out _lastCounters);
        }

        public class ExceptionalMapper : SimpleMapper<int, int, int>
        {
            protected override IEnumerable<KeyValuePair<int, int>> Map(int input)
            {
                throw new NotImplementedException();
            }
        }

        public class ExceptionalReducer : SimpleReducer<int, int, int, int>
        {
            protected override IEnumerable<KeyValuePair<int, int>> Reduce(int key, IEnumerable<int> values)
            {
                throw new NotImplementedException();
            }
        }

        public class GetYearPart : SimpleMapper<DateTime, int, DateTime>
        {
            protected override IEnumerable<KeyValuePair<int, DateTime>> Map(DateTime input)
            {
                yield return new KeyValuePair<int, DateTime>(input.Year, input);
            }
        }

        public class CountByYear : SimpleReducer<int, DateTime, int, int>
        {
            protected override IEnumerable<KeyValuePair<int, int>> Reduce(int key, IEnumerable<DateTime> values)
            {
                yield return new KeyValuePair<int, int>(key, values.Count());
            }
        }

        public class Div10Mapper : SimpleMapper<int, int, int>
        {
            protected override IEnumerable<KeyValuePair<int, int>> Map(int input)
            {
                yield return new KeyValuePair<int, int>(input / 10, 1);
            }
        }

        public class Mul2Mul4Mapper : SimpleMapper<int, int, int>
        {
            protected override IEnumerable<KeyValuePair<int, int>> Map(int input)
            {
                yield return new KeyValuePair<int, int>(input * 2, input * 4);
            }
        }

        public class CountReducer : SimpleReducer<int, int, int, int>
        {
            protected override IEnumerable<KeyValuePair<int, int>> Reduce(int key, IEnumerable<int> values)
            {
                yield return new KeyValuePair<int, int>(key, values.Sum());
            }
        }

        public class MapperWithCounters : SimpleMapper<string, string, int>
        {
            protected override IEnumerable<KeyValuePair<string, int>> Map(string input)
            {
                if (input.Contains("foo"))
                {
                    Increment("NumberOfFoos");
                }
                yield return new KeyValuePair<string, int>(input, input.Length);
            }
        }

        public class ReducerWithCounters : SimpleReducer<string, int, string, int>
        {
            protected override IEnumerable<KeyValuePair<string, int>> Reduce(string key, IEnumerable<int> values)
            {
                if (key.Contains("bar"))
                {
                    Increment("FiveHundredTimesNumberOfBars", "TestReduction", 500);
                }
                if (key.Contains("foo"))
                {
                    Increment("TenTimesNumberOfFoos", "TestReduction", 10);
                }
                yield return new KeyValuePair<string, int>(key, values.Sum());
            }
        }
    }
}
