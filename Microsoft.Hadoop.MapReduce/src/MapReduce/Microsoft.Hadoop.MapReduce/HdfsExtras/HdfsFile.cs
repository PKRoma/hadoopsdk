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
using System.IO;
using System.Text.RegularExpressions;

namespace Microsoft.Hadoop.MapReduce
{
    /// <summary>
    /// Utilities for working with HDFS files
    /// </summary>
    public static class HdfsFile
    {
        /// <summary>
        /// Writes text lines to a HDFS file
        /// </summary>
        /// <param name="hdfsPath">The HDFS path.</param>
        /// <param name="lines">The lines.</param>
        public static void WriteAllLines(string hdfsPath, IEnumerable<string> lines)
        {
            string tempPath = System.IO.Path.GetTempFileName();
            File.WriteAllLines(tempPath, lines);
            ProcessUtil.RunHadoopCommand_ThrowOnError(EnvironmentUtils.PathToHadoopExe, string.Format("fs -moveFromLocal \"{0}\" \"{1}\"", tempPath, hdfsPath), false);
            File.Delete(tempPath);
        }

        /// <summary>
        /// Writes all text from a string to a HDFS file.
        /// </summary>
        /// <param name="hdfsPath">The HDFS path.</param>
        /// <param name="data">The data.</param>
        public static void WriteAllText(string hdfsPath, string data)
        {
            string tempPath = System.IO.Path.GetTempFileName();
            File.WriteAllText(tempPath, data);
            ProcessUtil.RunHadoopCommand_ThrowOnError(EnvironmentUtils.PathToHadoopExe, string.Format("fs -moveFromLocal \"{0}\" \"{1}\"", tempPath, hdfsPath), false);
        }

        /// <summary>
        /// Writes all bytes.
        /// </summary>
        /// <param name="hdfsPath">The HDFS path.</param>
        /// <param name="bytes">The bytes.</param>
        public static void WriteAllBytes(string hdfsPath, byte[] bytes)
        {
            string tempPath = System.IO.Path.GetTempFileName();
            File.WriteAllBytes(tempPath, bytes);
            ProcessUtil.RunHadoopCommand_ThrowOnError(EnvironmentUtils.PathToHadoopExe, string.Format("fs -moveFromLocal \"{0}\" \"{1}\"", tempPath, hdfsPath), false);
        }

        /// <summary>
        /// Tests if a HDFS file/folder exists.
        /// </summary>
        /// <param name="hdfsPath">The HDFS path.</param>
        /// <returns>true if the hdfsPath exists</returns>
        public static bool Exists(string hdfsPath)
        {
            int exitCode;
            string stdout;
            string stderr;
            ProcessUtil.RunCommand(EnvironmentUtils.PathToHadoopExe, string.Format("fs -ls \"{0}\"", hdfsPath), out exitCode, out stdout, out stderr, false);
            bool found = !stderr.Contains("Cannot access");
            return found;
        }

        /// <summary>
        /// Make directory.
        /// </summary>
        /// <param name="hdfsPath">The HDFS path.</param>
        public static void MakeDirectory(string hdfsPath)
        {
            int exitCode;
            string stdout;
            string stderr;
            ProcessUtil.RunCommand(EnvironmentUtils.PathToHadoopExe, string.Format("fs -mkdir \"{0}\"", hdfsPath), out exitCode, out stdout, out stderr, false);
            if (stderr.Contains("cannot create"))
            {
                throw new StreamingException(stderr);
            }
        }

        //private helper
        private static string CopyToTemp(string hdfsPath)
        {
            string tempFilePath = System.IO.Path.GetTempFileName();
            CopyToLocal(hdfsPath, tempFilePath);
            return tempFilePath;
        }

        /// <summary>
        /// Copies HDFS file to local path
        /// </summary>
        /// <param name="hdfsPath">The HDFS path to the file to read.</param>
        /// <param name="localPath">The local path destination</param>
        public static void CopyToLocal(string hdfsPath, string localPath)
        {
            if (File.Exists(localPath))
            {
                File.Delete(localPath);
            }
            ProcessUtil.RunHadoopCommand_ThrowOnError(EnvironmentUtils.PathToHadoopExe, string.Format("fs -copyToLocal \"{0}\" \"{1}\"", hdfsPath, localPath), false);

            if (!File.Exists(localPath))
            {
                throw new StreamingException("Copy to local did not succeed.");
            }
        }

        /// <summary>
        /// Copies data to HDFS from a local file.
        /// </summary>
        /// <param name="localPath">The local path source.</param>
        /// <param name="hdfsPath">The HDFS path destination.</param>
        public static void CopyFromLocal(string localPath, string hdfsPath)
        {
            ProcessUtil.RunHadoopCommand_ThrowOnError(EnvironmentUtils.PathToHadoopExe, string.Format("fs -copyFromLocal \"{0}\" \"{1}\"", localPath, hdfsPath), false);
        }


        /// <summary>
        /// Reads all the lines in the given HDFS file path.
        /// </summary>
        /// <param name="hdfsPath">The HDFS path to the file to read.</param>
        /// <returns>The lines read.</returns>
        public static string[] ReadAllLines(string hdfsPath)
        {
            string tempPath = CopyToTemp(hdfsPath);
            string[] lines = File.ReadAllLines(tempPath);
            File.Delete(tempPath);
            return lines;
        }



        /// <summary>
        /// Reads all the bytes in the given HDFS file path.
        /// </summary>
        /// <param name="hdfsPath">The HDFS path to the file to read.</param>
        /// <returns>The bytes read.</returns>
        public static byte[] ReadAllBytes(string hdfsPath)
        {
            string tempPath = CopyToTemp(hdfsPath);
            byte[] bytes = File.ReadAllBytes(tempPath);
            File.Delete(tempPath);
            return bytes;
        }

        /// <summary>
        /// Delete a Hdfs file/folder
        /// </summary>
        /// <param name="hdfsPath">The HDFS path.</param>
        public static void Delete(string hdfsPath)
        {
            ProcessUtil.RunHadoopCommand_ThrowOnError(EnvironmentUtils.PathToHadoopExe, string.Format("fs -rmr \"{0}\"", hdfsPath), false);
        }


        /// <summary>
        /// List all the files in folder.
        /// </summary>
        /// <param name="folderName">Folder name.</param>
        /// <returns>List of file paths</returns>
        public static string[] EnumerateFilesInFolder(string folderName)
        {
            if (!HdfsFile.Exists(folderName))
            {
                throw new ArgumentException("folder does not exist", "folderName");
            }

            List<string> files = new List<string>();
            int exitCode;
            string stdout;
            string stderr;
            ProcessUtil.RunCommand(EnvironmentUtils.PathToHadoopExe, string.Format("fs -ls \"{0}\"", folderName), out exitCode, out stdout, out stderr, false);

            string fRegex = "^-.*"                                          //avoid folders. eg -rw-r--r-- vs. drwxr-xr-x
                            + HdfsPath.Combine("(" + folderName, ".*)\r$"); // select filename including folder path. match \n and trim \r
            Regex freg = new Regex(fRegex, RegexOptions.Multiline);
            int fMatchCount = freg.Matches(stdout).Count;

            foreach (Match m in freg.Matches(stdout))
            {
                files.Add(m.Groups[1].Value);
            }

            return files.ToArray();
        }

        /// <summary>
        /// Enumerates the data in folder.
        /// </summary>
        /// <param name="folderName">Name of the folder.</param>
        /// <returns>Lines in all files, up to max of 20 lines.</returns>
        public static IEnumerable<string> EnumerateDataInFolder(string folderName)
        {
            return EnumerateDataInFolder(folderName, 20);
        }

        /// <summary>
        /// Enumerates the data in folder.
        /// </summary>
        /// <param name="folderName">Name of the folder.</param>
        /// <param name="maxLines">Maximum lines to include in the enumeration.</param>
        /// <param name="fileNamePredicate">File name predicate.</param>
        /// <returns>Lines in all files that match predicate, up to maxLines total.</returns>
        public static IEnumerable<string> EnumerateDataInFolder(string folderName, int maxLines, Func<string, bool> fileNamePredicate = null)
        {
            int linesRead = 0;
            foreach (string hdfsPath in EnumerateFilesInFolder(folderName))
            {
                string tempPath = System.IO.Path.GetTempFileName();
                File.Delete(tempPath); // creation of temp path actually creates a file.. need to delete it before export from HDFS.
                ProcessUtil.RunHadoopCommand_ThrowOnError(EnvironmentUtils.PathToHadoopExe, string.Format("fs -copyToLocal \"{0}\" \"{1}\"", hdfsPath, tempPath), false);
                using (StreamReader sr = File.OpenText(tempPath))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (linesRead >= maxLines)
                        {
                            yield return "<output truncated>";
                            break;
                        }
                        
                        linesRead++;
                        yield return line;
                    }
                }
            }
        }

    }
}
