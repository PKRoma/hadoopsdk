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

namespace ProcDetailsTestApplication
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    /// <summary>
    /// This application when executed provides the details of it's execution
    /// space in a manner that can be later parsed by the test system.
    /// </summary>
    public class ProcDetails
    {
        /* 
         * The Delimiters are used to safely (uniquely) delimit the data in a mannor that is
         * safe when supplied against nearly any data format.  Guid's are used as their numerical
         * range nearly ensures that their value will not "randomly" appear in the data.
         * 
         * Note: Delimiters only follow data.
         * Note: No additional white space should ever be added in order to protect the underlying data.
         * 
         * Here are the most common examples of delimitation.
         * MajorSection[0]
         * [SectionDelimiter == Required]
         *   MajorSection[1].EntryData[0].Key
         *   [EntryDelimiter = Optional (only needed if the data has destinct entries)]
         *   MajorSection[1].EntryData[1].Key
         * [SectionDelimiter == Required]
         *     MajorSection[1].MinorSection[2].EntryData[0].Key
         *     [EntryPairDelimiter = Optional (only needed if the data is a key/value pair)]
         *     MajorSection[1].MinorSection[2].EntryData[0].Value
         *   [EntryDelimiter = Optional (only needed if the data has destinct entries)]
         *     MajorSection[1].MinorSection[2].EntryData[1].Key
         *     [EntryPairDelimiter = Optional (only needed if the data is a key/value pair)]
         *     MajorSection[1].MinorSection[2].EntryData[1].Value
         */

        // Separates Major Sections of the data.
        public static readonly Guid SectionDelimiter = new Guid("{77711443-E145-4162-8BA7-0D18D8E1BCE8}");

        // Separates Elements inside of a Major or Minor Section.
        public static readonly Guid EntryDelimiter = new Guid("{A1350BDC-2AD8-4571-B617-9AEF8650F0E5}");

        // Separates Key/Value Pairs.
        public static readonly Guid EntryPairDelimiter = new Guid("{59183EB5-F52E-4262-96F4-E8F20C27B2E3}");

        public static readonly Guid TabReplacementSequence = new Guid("{0C5183EA-9C83-4E3D-AE6A-7E958228EE86}");
        public static readonly Guid CharacterReturnReplacementSequence = new Guid("{4F54BB4B-AEC1-43AE-B3C0-461B44D4C754}");
        public static readonly Guid LineFeedReplacementSequence = new Guid("{42AEA506-8329-4188-BEB6-5D2B6218FED0}");

        private static StringBuilder stringBuilder = new StringBuilder();
        private static void Write(object value)
        {
            stringBuilder.Append(value);
        }

        private static void OutputResults()
        {
            stringBuilder.Replace("\t", TabReplacementSequence.ToString());
            stringBuilder.Replace("\r", CharacterReturnReplacementSequence.ToString());
            stringBuilder.Replace("\n", LineFeedReplacementSequence.ToString());
            Console.Write(stringBuilder);
        }

        private static void Main(string[] args)
        {
            // Environment Variable Section...
            // Enumerates each of the environment variables.
            foreach (DictionaryEntry environmentVariable in System.Environment.GetEnvironmentVariables())
            {
                Write(environmentVariable.Key);
                Write(EntryPairDelimiter);
                Write(environmentVariable.Value);
                Write(EntryDelimiter);
            }
            Write(SectionDelimiter);

            // Arguments Section...
            // Enumerates each of the arguments supplied to the application.
            foreach (var arg in args)
            {
                Write(arg);
                Write(EntryDelimiter);
            }

            Write(SectionDelimiter);

            // Working Directory Section...
            // Provides the current working directory.
            Write(Directory.GetCurrentDirectory());

            Write(SectionDelimiter);
            // Files Within the Working Directory Section...
            List<string> entries = new List<string>();
            var files = Directory.EnumerateFiles(Directory.GetCurrentDirectory(), "*.*", SearchOption.AllDirectories);
            entries.AddRange(files);
            var rootDirectory = Path.GetDirectoryName(Path.GetDirectoryName(Directory.GetCurrentDirectory()));
            var directories = Directory.EnumerateDirectories(rootDirectory, "*.*", SearchOption.AllDirectories);
            entries.AddRange(directories);
            entries.Sort(StringComparer.OrdinalIgnoreCase);
            foreach (var entry in entries)
            {
                Write(entry);
                Write(EntryDelimiter);
            }
            OutputResults();
        }
    }
}
