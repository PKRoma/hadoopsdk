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


using System.Linq;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace Microsoft.Hadoop.MapReduce.Test
{
    /// <summary>
    /// Parses and encapsulates the counters output by Hadoop jobs.
    /// </summary>
    class Counters
    {
        private ConcurrentDictionary<string, ConcurrentDictionary<string, long>> _mapCounters = new ConcurrentDictionary<string, ConcurrentDictionary<string, long>>();
        private ConcurrentDictionary<string, ConcurrentDictionary<string, long>> _reduceCounters = new ConcurrentDictionary<string, ConcurrentDictionary<string, long>>();
        private static readonly Regex _taskLogLineRegEx =
            new Regex(@"^Task .*TASK_TYPE=""(?<TaskType>[^""]*)"" .* COUNTERS=""(?<Counters>[^""]*)"".*$");
        private static readonly Regex _counterRegEx =
            new Regex(@"\{\((?<GroupName>[^)]*)\)\((?<GroupDisplayName>[^)]*)\)(?<Counter>\[\((?<CounterName>[^)]*)\)\((?<CounterDisplayName>[^)]*)\)\((?<CounterValue>[^)]*)\)\])*\}");

        /// <summary>
        /// Creates a new <see cref="Counters"/> object by parsing the logs from the job's output directory.
        /// </summary>
        /// <param name="outputPath">The path to the job's output directory in HDFS.</param>
        /// <returns>The newly created <see cref="Counters"/> object.</returns>
        public static Counters CreateFromOutputPath(string outputPath)
        {
            Counters ret = new Counters();
            foreach (string line in HdfsFile.EnumerateDataInFolder(outputPath + "/_logs/history", maxLines: 50000))
            {
                Match taskLogLine = _taskLogLineRegEx.Match(line);
                if (taskLogLine.Success)
                {
                    ConcurrentDictionary<string, ConcurrentDictionary<string, long>> properDictionary;
                    switch (taskLogLine.Groups["TaskType"].Value)
                    {
                        case "MAP": properDictionary = ret._mapCounters; break;
                        case "REDUCE": properDictionary = ret._reduceCounters; break;
                        default: continue;
                    }
                    MatchCollection counters = _counterRegEx.Matches(taskLogLine.Groups["Counters"].Value);
                    foreach (Match counterMatch in counters)
                    {
                        ConcurrentDictionary<string, long> group = properDictionary.GetOrAdd(counterMatch.Groups["GroupName"].Value, name => new ConcurrentDictionary<string, long>());
                        for (int i = 0; i < counterMatch.Groups["Counter"].Captures.Count; i++)
                        {
                            long counterValue = long.Parse(counterMatch.Groups["CounterValue"].Captures[i].Value);
                            group.AddOrUpdate(counterMatch.Groups["CounterName"].Captures[i].Value, counterValue, (n, v) => v + counterValue);
                        }
                    }
                }
            }
            return ret;
        }

        /// <summary>
        /// Gets the total value of the given counter as computed by the Mapper tasks.
        /// </summary>
        /// <param name="groupName">The group name of the counter.</param>
        /// <param name="name">The name of the counter.</param>
        /// <returns>The total value.</returns>
        public long GetMapCounter(string groupName, string name)
        {
            return GetCounter(_mapCounters, groupName, name);
        }

        /// <summary>
        /// Gets the total value of the given counter as computed by the Reducer tasks.
        /// </summary>
        /// <param name="groupName">The group name of the counter.</param>
        /// <param name="name">The name of the counter.</param>
        /// <returns>The total value.</returns>
        public long GetReduceCounter(string groupName, string name)
        {
            return GetCounter(_reduceCounters, groupName, name);
        }

        private long GetCounter(ConcurrentDictionary<string, ConcurrentDictionary<string, long>> counters, string groupName, string name)
        {
            ConcurrentDictionary<string, long> group;
            if (!counters.TryGetValue(groupName, out group))
            {
                return 0;
            }
            long ret;
            if (!group.TryGetValue(name, out ret))
            {
                return 0;
            }
            return ret;
        }
    }
}
