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
using System.Linq;
using System.Text;
using System.Collections;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using System.Diagnostics;

namespace Microsoft.Hadoop.MapReduce
{
    /// <summary>
    /// Represents a Hadoop streaming job
    /// </summary>
    public class HadoopJobConfiguration
    {
        /// <summary>
        /// Gets or sets the input path for job.
        /// </summary>
        /// <remarks>
        /// This can reference a single HDFS file or a HDFS folder that contains only plain files.
        /// </remarks>
        public string InputPath { get; set; }

        
        private List<string> _additionalInputPath = new List<string>();
        
        /// <summary>
        /// Gets or sets the additional input paths.
        /// </summary>
        /// <remarks>
        /// Each path can reference a single HDFS file or a HDFS folder that contains only plain files.
        /// </remarks>
        public List<string> AdditionalInputPath { 
            get { 
                return _additionalInputPath; 
            }
            set
            {
                _additionalStreamingArguments = value; //a setter is useful if the user already has a List<string> instance.
            }
        }

        /// <summary>
        /// Gets or sets the output folder HDFS.
        /// </summary>
        /// <value>
        /// The output folder HDFS.
        /// </value>
        public string OutputFolder { get; set; }


        /// <summary>
        /// Gets or sets a value indicating whether verbose console output should be generated.
        /// </summary>
        /// <value>
        ///   <c>true</c> if verbose; otherwise, <c>false</c>.
        /// </value>
        public bool Verbose { get; set; }

        private List<string> _additionalGenericArguments = new List<string>();
        /// <summary>
        /// Additional arguments for Hadoop.
        /// </summary>
        /// <remarks>
        /// With Hadoop 1.0, generic options supported include
        ///<pre>
        ///-conf {configuration file}     specify an application configuration file
        ///-D "property=value"            use value for given property.  Note: quotation marks are critical.
        ///-fs {local|namenode:port}      specify a namenode
        ///-jt {local|jobtracker:port}    specify a job tracker
        ///-files {comma separated list of files}         (also available via JobConfiguration.FilesToInclude)
        ///-libjars {comma separated list of jars}         specify comma separated jar files to include in the classpath.
        ///-archives {comma separated list of archives}    specify comma separated archives to be unarchived on the compute machines.
        ///
        /// Key options that can be specified via -D "prop=value" include: 
        ///         io.sort.factor=10
        ///         io.sort.mb=100
        ///         io.sort.record.percent=0.05
        ///         io.sort.spill.percent=0.80
        ///         job.end.retry.attempts=0
        ///         job.end.retry.interval=30000
        ///         keep.failed.task.files=false
        ///         mapred.inmem.merge.threshold=1000
        ///         mapred.job.shuffle.input.buffer.percent=0.70
        ///         mapred.job.shuffle.merge.percent=0.66
        ///         mapred.job.reuse.jvm.num.tasks=1  
        ///         mapred.jobtracker.maxtasks.per.job=-1
        ///         mapred.line.input.format.linespermap=1
        ///         mapred.map.max.attempts=4 (available as setting on JobConfiguration)
        ///         mapred.map.tasks=2
        ///         mapred.map.tasks.speculative.execution=true
        ///         mapred.merge.recordsBeforeProgress=10000
        ///         mapred.output.compress=false
        ///         mapred.reduce.max.attempts=3 (available as setting on JobConfiguration)
        ///         mapred.reduce.parallel.copies
        ///         mapred.reduce.slowstart.completed.maps
        ///         mapred.reduce.tasks=1
        ///         mapred.reduce.tasks.speculative.execution=true
        ///         mapred.skip.attempts.to.start.skipping=2
        ///         mapred.submit.replication=10
        ///         mapred.task.timeout=600000
        ///         mapred.tasktracker.map.tasks.maximum=2
        ///         mapred.tasktracker.reduce.tasks.maximum=2
        ///         mapred.used.genericoptionsparser=true
        ///         mapred.user.jobconf.limit=5242880
        ///         mapred.working.dir=hdfs://localhost:9000/user/(user)
        ///  </pre>  
        ///  <example>
        ///  <code source="DocCodeSnippets/Snippets1.cs" region="Snippet.StreamingJobConfiguration.SettingGenericArg" lang="C#" title="Setting a generic option to control hadoop" />
        ///  </example>
        /// </remarks>
        public List<string> AdditionalGenericArguments
        {
            get { return _additionalGenericArguments; }
        }
        
        private List<string> _additionalStreamingArguments = new List<string>();

        /// <summary>
        /// Additional arguments for Hadoop streaming.
        /// </summary>
        /// <remarks>
        /// These arguments are specific to streaming jobs.  For any settings that are applicable to any hadoop job, use <see cref="AdditionalGenericArguments"/>.
        /// <pre>
        /// With Hadoop Streaming 1.0, generic options supported include
        /// -mapdebug {path}     Runs script when a map task fails
        /// -reducedebug {path}  Run script when a reduce task fails
        /// -cmdenv "key=val"    Passes environment var to the mapper/combiner/reducer EXEs. Note: quotation marks are critical.
        /// -inputformat TextInputFormat(default)|SequenceFileAsTextInputFormat|JavaClassName
        /// -outputformat TextOutputFormat(default)|JavaClassName
        /// -numReduceTasks {num}
        /// -inputreader {spec}
        /// -verbose
        /// </pre>
        /// </remarks>
        public List<string> AdditionalStreamingArguments { 
            get { return _additionalStreamingArguments;}
        }

        private int _sortKeyColumnCount = 1;


        /// <summary>
        /// Gets or sets the number of columns for the Key during the Sort phase.
        /// </summary>
        /// <value>
        /// The number of columns for the Key during the sort phase.
        /// </value>
        public int SortKeyColumnCount
        {
            get { return _sortKeyColumnCount; }
            set { _sortKeyColumnCount = value; }
        }

        private int _shuffleKeyColumnCount = 1;

        /// <summary>
        /// Gets or sets the number of columns for the Key during the Shuffle phase.
        /// </summary>
        /// <value>
        /// The number of columns for the Key during the sort phase.
        /// </value>
        public int ShuffleKeyColumnCount
        {
            get { return _shuffleKeyColumnCount; }
            set { _shuffleKeyColumnCount = value; }
        }

        private bool _deleteOutputFolder = true;

        /// <summary>
        /// Gets or sets a value indicating whether to delete the output folder if it exists when the job is executed.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if output folder should be deleted; otherwise, <c>false</c>.
        /// </value>
        public bool DeleteOutputFolder
        {
            get { return _deleteOutputFolder; }
            set { _deleteOutputFolder = value; }
        }

        private List<string> _filesToInclude = new List<string>();

        /// <summary>
        /// Gets or sets the files to include in job package.
        /// </summary>
        /// <value>
        /// Each entry should be a full path to a regular (eg NTFS) file.
        /// These files will be included in the job via "-file " parameter.
        /// </value>
        public List<string> FilesToInclude
        {
            get { return _filesToInclude; }
            set { _filesToInclude = value; }
        }

        private List<string> _filesToExclude = new List<string>();

        /// <summary>
        /// Gets or sets the files to exclude from job package.
        /// </summary>
        /// <value>
        /// Each entry can be either a full path to a local file or a partial path such as the filename. Globs/regex are not supported.
        /// Examples:  
        ///  - including @"c:\myfolder\myfile.dll" will prevent that one file from inclusion
        ///  - including @"myfile.dll" will prevent any path that includes myfile.dll from inclusion.
        /// These files will not be included in the job via "-file " parameters or other mechanisms.
        /// This provides an opt-out if files are automatically included in job and are not required.
        /// </value>
        public List<string> FilesToExclude
        {
            get { return _filesToExclude; }
            set { _filesToExclude = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to compress output as Gzip file.
        /// </summary>
        /// <value>
        ///   <c>true</c> if output will be compressed; otherwise, <c>false</c>.
        /// </value>
        public bool CompressOutput { get; set; }

        internal const int MAX_ATTEMPTS_DEFAULT = 1;
        private int _maxAttemptsMapper = MAX_ATTEMPTS_DEFAULT; //@@TODO: should probably come from mapredsite.xml

        /// <summary>
        /// Gets or sets the maximum number of attempts when invoking the Mapper.
        /// </summary>
        /// <remarks>
        /// Default=1.
        /// </remarks>
        /// <value>
        /// Maximum number of attempts that will be made to run the Mapper process.
        /// </value>
        public int MaximumAttemptsMapper
        {
            get { return _maxAttemptsMapper; }
            set { _maxAttemptsMapper = value; }
        }

        private int _maxAttemptsReducer = MAX_ATTEMPTS_DEFAULT; //@@TODO: should probably come from mapredsite.xml

        /// <summary>
        /// Gets or sets the maximum number of attempts when invoking the Reducer.
        /// </summary>
        /// <remarks>
        /// Default=1.
        /// </remarks>
        /// <value>
        /// Maximum number of attempts that will be made to run the Reducer process.
        /// </value>
        public int MaximumAttemptsReducer
        {
            get { return _maxAttemptsReducer; }
            set { _maxAttemptsReducer = value; }
        }


        /// <summary>
        /// Gets or sets a value indicating whether to retain task log files on job failure.
        /// </summary>
        /// <remarks>
        /// default = false.</remarks>
        /// <value>
        ///    <c>true</c> if logs files are retained on task failure]; otherwise, <c>false</c>.
        /// </value>
        public bool KeepFailedTaskFiles { get; set; }
    }
}
