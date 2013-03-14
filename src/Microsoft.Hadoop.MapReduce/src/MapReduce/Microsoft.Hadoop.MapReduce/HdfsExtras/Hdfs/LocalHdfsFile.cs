namespace Microsoft.Hadoop.MapReduce.HdfsExtras.Hdfs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Globalization;
    using System.IO;
    using Microsoft.Hadoop.MapReduce.Execution.Hadoop;

    public class LocalHdfsFile : HdfsFileBase
    {
        internal delegate IProcessExecutor ExecutionConstructor();

        internal ExecutionConstructor MakeExecutor;
        private ITempPathGenerator pathGenerator;

        internal LocalHdfsFile(ExecutionConstructor constructor, ITempPathGenerator generator)
        {
            this.MakeExecutor = () =>
            {
                var executor = constructor();
                executor.Command = EnvironmentUtils.PathToHadoopExe;
                executor.Arguments.Add("fs");
                return executor;
            };
            this.pathGenerator = generator;
        }

        public static IHdfsFile Create()
        {
            return new LocalHdfsFile(() => new ProcessExecutor(), new TempPathGenerator());
        }

        public override bool Exists(string hdfsPath)
        {
            hdfsPath = this.GetAbsolutePath(hdfsPath);
            var executor = this.MakeExecutor();
            executor.Arguments.Add(FsCommands.Ls);
            executor.Arguments.Add(hdfsPath);
            int retval = ProcessUtil.RunHadoopCommand(executor);
            var stdErr = string.Join(Environment.NewLine, executor.StandardError);
            if (retval == 0 && stdErr.IndexOf("Cannot access", StringComparison.Ordinal) == -1)
            {
                return true;
            }
            return false;
        }



        public override void MakeDirectory(string hdfsPath)
        {
            hdfsPath = this.GetAbsolutePath(hdfsPath);
            var executor = this.MakeExecutor();
            executor.Arguments.Add(FsCommands.MkDir);
            executor.Arguments.Add(hdfsPath);
            ProcessUtil.RunHadoopCommand_ThrowOnError(executor);
            string stdErr = string.Join(Environment.NewLine, executor.StandardError);
            if (stdErr.IndexOf("cannot create", StringComparison.Ordinal) != -1)
            {
                throw new StreamingException(stdErr);
            }
        }

        public override void CopyToLocal(string hdfsPath, string localPath)
        {
            hdfsPath = this.GetAbsolutePath(hdfsPath);
            var executor = this.MakeExecutor();
            if (File.Exists(localPath))
            {
                File.Delete(localPath);
            }
            executor.Arguments.Add(FsCommands.CopyToLocal);
            executor.Arguments.Add(hdfsPath);
            executor.Arguments.Add(localPath);
            ProcessUtil.RunHadoopCommand_ThrowOnError(executor);
        }

        public override void CopyFromLocal(string localPath, string hdfsPath)
        {
            hdfsPath = this.GetAbsolutePath(hdfsPath);
            var executor = this.MakeExecutor();
            executor.Arguments.Add(FsCommands.CopyFromLocal);
            executor.Arguments.Add(localPath);
            executor.Arguments.Add(hdfsPath);
            ProcessUtil.RunHadoopCommand_ThrowOnError(executor);
        }

        public override void Delete(string hdfsPath)
        {
            hdfsPath = this.GetAbsolutePath(hdfsPath);
            var executor = this.MakeExecutor();
            executor.Arguments.Add(FsCommands.Rmr);
            executor.Arguments.Add(hdfsPath);
            ProcessUtil.RunHadoopCommand_ThrowOnError(executor);
        }

        public override string[] LsFiles(string hdfsPath)
        {
            hdfsPath = this.GetAbsolutePath(hdfsPath);
            List<string> result = new List<string>();
            var executor = this.MakeExecutor();
            executor.Arguments.Add(FsCommands.Ls);
            executor.Arguments.Add(hdfsPath);
            ProcessUtil.RunHadoopCommand_ThrowOnError(executor);
            if (string.Join("\r\n", executor.StandardError.ToArray()).IndexOf("No such file or directory", StringComparison.Ordinal) >= 0)
            {
                string msg = string.Format(CultureInfo.InvariantCulture, "No such file or directory '{0}'", hdfsPath);
                throw new IOException(msg);
            }
            // We can skip the first line of output as it is the count of items found.
            IEnumerable<string> output = executor.StandardOut.Skip(1);
            foreach (var line in output)
            {
                int part = 0;
                StringBuilder[] parts = new StringBuilder[8];
                parts[0] = new StringBuilder();
                bool inWhiteSpace = false;
                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }
                // parse line.   
                foreach (var character in line)
                {
                    // The last part of the line is the file path (whitespaces do not separate there)
                    if (char.IsWhiteSpace(character) && part != 7)
                    {
                        if (!inWhiteSpace)
                        {
                            inWhiteSpace = true;
                            part++;
                            parts[part] = new StringBuilder();
                        }
                    }
                    else
                    {
                        inWhiteSpace = false;
                        parts[part].Append(character);
                    }
                }
                if (parts[0][0] == '-')
                {
                    result.Add(parts[7].ToString());
                }
            }
            return result.ToArray();
        }

        public override string GetAbsolutePath(string hdfsPath)
        {
            if (string.IsNullOrEmpty(hdfsPath))
            {
                return "/";
            }
            else if (hdfsPath[0] == '/')
            {
                return hdfsPath;
            }
            else if (hdfsPath.Contains(":"))
            {
                Uri uri = new Uri(hdfsPath);
                return uri.AbsolutePath;
            }
            else
            {
                return "/user/" + System.Environment.GetEnvironmentVariable("USERNAME") + "/" + hdfsPath;
            }
        }

        public override string GetFullyQualifiedPath(string hdfsPath)
        {
            return "hdfs://" + this.GetAbsolutePath(hdfsPath);
        }

        public override void WriteAllLines(string hdfsPath, IEnumerable<string> lines)
        {
            hdfsPath = this.GetAbsolutePath(hdfsPath);
            string tempPath = this.pathGenerator.GetTempPath();
            File.WriteAllLines(tempPath, lines);
            this.PerformMoveFromLocal(tempPath, hdfsPath);
        }

        private void PerformMoveFromLocal(string local, string hdfsPath)
        {
            hdfsPath = this.GetAbsolutePath(hdfsPath);
            var executor = this.MakeExecutor();
            executor.Arguments.Add(FsCommands.MoveFromLocal);
            executor.Arguments.Add(local);
            executor.Arguments.Add(hdfsPath);
            try
            {
                ProcessUtil.RunHadoopCommand_ThrowOnError(executor);
            }
            finally
            {
                if (File.Exists(local))
                {
                    File.Delete(local);
                }
            }
        }

        private string PerformCopyToLocalLocal(string hdfsPath)
        {
            hdfsPath = this.GetAbsolutePath(hdfsPath);
            var executor = this.MakeExecutor();
            string local = this.pathGenerator.GetTempPath();
            if (File.Exists(local))
            {
                File.Delete(local);
            }
            executor.Arguments.Add(FsCommands.CopyToLocal);
            executor.Arguments.Add(hdfsPath);
            executor.Arguments.Add(local);
            ProcessUtil.RunHadoopCommand_ThrowOnError(executor);
            return local;
        }

        public override void WriteAllText(string hdfsPath, string text)
        {
            hdfsPath = this.GetAbsolutePath(hdfsPath);
            string tempPath = this.pathGenerator.GetTempPath();
            File.WriteAllText(tempPath, text);
            this.PerformMoveFromLocal(tempPath, hdfsPath);
        }

        public override void WriteAllBytes(string hdfsPath, byte[] bytes)
        {
            hdfsPath = this.GetAbsolutePath(hdfsPath);
            string tempPath = this.pathGenerator.GetTempPath();
            File.WriteAllBytes(tempPath, bytes);
            this.PerformMoveFromLocal(tempPath, hdfsPath);
        }

        private byte[] ReadFileBytes(string file)
        {
            byte[] retval = new byte[0];
            if (File.Exists(file))
            {
                retval = File.ReadAllBytes(file);
            }
            return retval;
        }

        private string[] ReadFileLines(string file)
        {
            string[] retval = new string[0];
            if (File.Exists(file))
            {
                retval = File.ReadAllLines(file);
            }
            return retval;
        }

        private string ReadFileText(string file)
        {
            string retval = string.Empty;
            if (File.Exists(file))
            {
                retval = File.ReadAllText(file);
            }
            return retval;
        }

        public override string ReadAllText(string hdfsPath)
        {
            hdfsPath = this.GetAbsolutePath(hdfsPath);
            string tempPath = this.PerformCopyToLocalLocal(hdfsPath);
            string result = this.ReadFileText(tempPath);
            File.Delete(tempPath);
            return result;
        }

        /// <summary>
        /// Reads all the lines in the given HDFS file path.
        /// </summary>
        /// <param name="hdfsPath">The HDFS path to the file to read.</param>
        /// <returns>The lines read.</returns>
        public override string[] ReadAllLines(string hdfsPath)
        {
            hdfsPath = this.GetAbsolutePath(hdfsPath);
            string tempPath = this.PerformCopyToLocalLocal(hdfsPath);
            string[] lines = this.ReadFileLines(tempPath);
            File.Delete(tempPath);
            return lines;
        }

        /// <summary>
        /// Reads all the lines in the given HDFS file path.
        /// </summary>
        /// <param name="hdfsPath">The HDFS path to the file to read.</param>
        /// <returns>The lines read.</returns>
        public override byte[] ReadAllBytes(string hdfsPath)
        {
            hdfsPath = this.GetAbsolutePath(hdfsPath);
            string tempPath = this.PerformCopyToLocalLocal(hdfsPath);
            byte[] bytes = this.ReadFileBytes(tempPath);
            File.Delete(tempPath);
            return bytes;
        }

    }
}
