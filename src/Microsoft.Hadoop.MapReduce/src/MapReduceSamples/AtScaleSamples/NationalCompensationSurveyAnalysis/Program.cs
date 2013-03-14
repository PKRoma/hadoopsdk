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
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Hadoop.MapReduce;

namespace NationalCompensationSurveyAnalysis
{
    /// <summary>
    /// This sample analyzes the data from the National Compensation Survey conducted by the US Bureau of Labor Statistics
    /// (more info here: https://explore.data.gov/Labor-Force-Employment-and-Earnings/National-Compensation-Survey/zyj3-8yk4).
    /// </summary>
    public class Program
    {
        const string HdfsDataFolder = "NCSData";
        const string HdfsOutputFolder = "NCSAnalyzed";

        /// <summary>
        /// A state and area as represented in the nw.starea file.
        /// </summary>
        private class StateArea
        {
            private readonly string _stateCode;
            private readonly string _areaCode;
            private readonly string _areaText;

            public string StateCode
            {
                get { return _stateCode; }
            }

            public string AreaCode
            {
                get { return _areaCode; }
            }

            public string AreaText
            {
                get { return _areaText; }
            }

            private StateArea(string stateCode, string areaCode, string areaText)
            {
                _stateCode = stateCode;
                _areaCode = areaCode;
                _areaText = areaText;
            }

            public static List<StateArea> Read(IEnumerable<string> lines)
            {
                return lines
                    .Skip(1) // header
                    .Select(l => l.Split('\t'))
                    .Select(s => new StateArea(s[0], s[1], s[2]))
                    .ToList();
            }
        }

        /// <summary>
        /// An industry as represented in the nw.industry file.
        /// </summary>
        private class Industry
        {
            private readonly string _industryCode;
            private readonly string _industryText;

            public string IndustryText
            {
                get { return _industryText; }
            }

            public string IndustryCode
            {
                get { return _industryCode; }
            }

            private Industry(string industryCode, string industryText)
            {
                _industryCode = industryCode;
                _industryText = industryText;
            }

            public static List<Industry> Read(IEnumerable<string> lines)
            {
                return lines
                    .Skip(1) // header
                    .Select(l => l.Split('\t'))
                    .Select(s => new Industry(s[0].Substring(0, 4), s[1]))
                    .ToList();
            }
        }

        static void Main(string[] args)
        {
            if (args[0] == "-local")
            {
                LocalRun(args[1]);
            }
            else
            {
                ClusterRun();
            }
        }

        /// <summary>
        /// Reads the given constant file name from the embedded resources in the assembly.
        /// </summary>
        /// <param name="name">The name of the file, e.g. nw.starea</param>
        /// <returns></returns>
        private static IEnumerable<string> ReadConstantFile(string name)
        {
            using (Stream s = typeof(Program).Assembly.GetManifestResourceStream("NationalCompensationSurveyAnalysis.Constants." + name))
            using (TextReader r = new StreamReader(s))
            {
                string line;
                while ((line = r.ReadLine()) != null)
                {
                    yield return line;
                }
            }
        }

        private static void ClusterRun()
        {
            var hadoop = Hadoop.Connect();
            hadoop.MapReduceJob.ExecuteJob<Job>();

            Console.WriteLine();
            Console.WriteLine("==========================");
            Console.WriteLine("Output:");
            foreach (string s in hadoop.StorageSystem.EnumerateDataInFolder(HdfsOutputFolder, 200))
            {
                Console.WriteLine(s);
            }
        }

        private static void LocalRun(string localDataFolder)
        {
            List<string> input = File.ReadAllLines(Path.Combine(localDataFolder, "nw.data.1.AllData")).ToList();
            StreamingUnit.Execute<Mapper, Reducer>(input).Result.ForEach(s => Console.WriteLine(s));
        }

        public class Job : HadoopJob<Mapper, Reducer>
        {
            public override HadoopJobConfiguration Configure(ExecutorContext context)
            {
                return new HadoopJobConfiguration()
                {
                    InputPath = HdfsDataFolder + "/nw.data.1.AllData",
                    OutputFolder = HdfsOutputFolder
                };
            }
        }

        /// <summary>
        /// Maps every line from the data file, filters out only median wage data,
        /// puts the key as the state-area-industry and puts the value as the year-month-value of this data.
        /// </summary>
        public class Mapper : MapperBase
        {
            private const string _hourlyMedianWageId = "14";
            private const string _allWorkersOccupation = "000000";
            private const string _allWorkersSubcell = "00";
            private const string _allWorkersLevel = "00";

            public override void Map(string inputLine, MapperContext context)
            {
                if (String.IsNullOrWhiteSpace(inputLine))
                {
                    return;
                }

                // Input file format documented in: ftp://ftp.bls.gov/pub/time.series/nw/nw.txt

                string[] split = inputLine.Trim('\r', ' ', '\t').Split(new[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
                if (split.Length == 0)
                {
                    return;
                }
                string seriesId = split[0];
                if (seriesId == "series_id")
                {
                    // Header
                    return;
                }
                if (!seriesId.StartsWith("NW")) // Every series ID should begin with the survey abbreviation, in this case it's NW.
                {
                    context.Log("Found an unexpected series ID: " + seriesId);
                    return;
                }
                if (seriesId.Length < 29)
                {
                    context.Log("Series ID too short: " + seriesId);
                    return;
                }
                string stateCode = seriesId.Substring(3, 2), areaCode = seriesId.Substring(5, 5),
                    industryCode = seriesId.Substring(13, 4), occupationCode = seriesId.Substring(17, 6),
                    subCellIdCode = seriesId.Substring(23, 2), dataTypeCode = seriesId.Substring(25, 2),
                    workerLevelCode = seriesId.Substring(27, 2);
                if (dataTypeCode != _hourlyMedianWageId || occupationCode != _allWorkersOccupation ||
                    subCellIdCode != _allWorkersSubcell || workerLevelCode != _allWorkersLevel)
                {
                    return;
                }
                string year = split[1], period = split[2], value = split[3];
                if (period.Length < 3)
                {
                    context.Log("period too short: " + period);
                    return;
                }
                if (!period.StartsWith("M"))
                {
                    return;
                }
                int month = Int32.Parse(period.Substring(1));
                if (month == 13) // annual average
                {
                    return;
                }
                context.EmitKeyValue(String.Join(",", stateCode, areaCode, industryCode), String.Join(",", year, month, value));
            }
        }

        /// <summary>
        /// Reduces the data for every area/industry to give the earliest and latest median wage in the survey.
        /// </summary>
        public class Reducer : ReducerCombinerBase
        {
            List<StateArea> _areas;
            List<Industry> _industries;
            HashSet<string> _unknownAreas, _unknownIndustries;

            public override void Initialize(ReducerCombinerContext context)
            {
                base.Initialize(context);
                _areas = StateArea.Read(ReadConstantFile("nw.starea"));
                _industries = Industry.Read(ReadConstantFile("nw.industry"));
                _unknownAreas = new HashSet<string>();
                _unknownIndustries = new HashSet<string>();
            }

            public override void Reduce(string key, IEnumerable<string> values, ReducerCombinerContext context)
            {
                string[] keySplit = key.Split(',');
                string stateCode = keySplit[0], areaCode = keySplit[1], industryCode = keySplit[2];
                StateArea area = _areas.SingleOrDefault(a => a.AreaCode == areaCode && a.StateCode == stateCode);
                if (area == null)
                {
                    if (_unknownAreas.Add(areaCode))
                    {
                        context.Log(String.Format("No area found for state code {0} and area code {1}.", stateCode, areaCode));
                    }
                    return;
                }
                Industry industry = _industries.SingleOrDefault(i => i.IndustryCode == industryCode);
                if (industry == null)
                {
                    if (_unknownIndustries.Add(industryCode))
                    {
                        context.Log(String.Format("No industry found with industry code {0}.", industryCode));
                    }
                    return;
                }
                var parsed = values
                    .Select(v => v.Split(','))
                    .Select(a => new { Year = Int32.Parse(a[0]), Month = Int32.Parse(a[1]), MedianWage = Decimal.Parse(a[2]) })
                    .OrderBy(o => o.Year * 12 + (o.Month - 1))
                    .ToList();
                context.EmitKeyValue(area.AreaText + " " + industry.IndustryText,
                    String.Format("{0}/{1}: {2} - {3}/{4}: {5}",
                        parsed[0].Month, parsed[0].Year, parsed[0].MedianWage,
                        parsed[parsed.Count - 1].Month, parsed[parsed.Count - 1].Year, parsed[parsed.Count - 1].MedianWage));
            }
        }

    }
}
