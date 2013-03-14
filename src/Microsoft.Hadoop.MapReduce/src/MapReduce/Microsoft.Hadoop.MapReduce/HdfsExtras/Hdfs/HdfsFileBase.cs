namespace Microsoft.Hadoop.MapReduce.HdfsExtras.Hdfs
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    public abstract class HdfsFileBase : IHdfsFile
    {
        /// <inheritdoc />
        public abstract void WriteAllLines(string hdfsPath, IEnumerable<string> lines);
        
        /// <inheritdoc />
        public abstract void WriteAllText(string hdfsPath, string text);
        
        /// <inheritdoc />
        public abstract void WriteAllBytes(string hdfsPath, byte[] bytes);
        
        /// <inheritdoc />
        public abstract string[] ReadAllLines(string hdfsPath);
        
        /// <inheritdoc />
        public abstract string ReadAllText(string hdfsPath);
        
        /// <inheritdoc />
        public abstract byte[] ReadAllBytes(string hdfsPath);
        
        /// <inheritdoc />
        public abstract bool Exists(string hdfsPath);
        
        /// <inheritdoc />
        public abstract void MakeDirectory(string hdfsPath);
        
        /// <inheritdoc />
        public abstract void CopyToLocal(string hdfsPath, string localPath);
        
        /// <inheritdoc />
        public abstract void CopyFromLocal(string localPath, string hdfsPath);
        
        /// <inheritdoc />
        public abstract void Delete(string hdfsPath);

        /// <inheritdoc />
        public abstract string[] LsFiles(string hdfsPath);

        /// <inheritdoc />
        public abstract string GetAbsolutePath(string hdfsPath);

        /// <inheritdoc />
        public abstract string GetFullyQualifiedPath(string hdfsPath);

        /// <summary>
        /// Enumerates the data in folder.
        /// </summary>
        /// <param name="folderName">Name of the folder.</param>
        /// <returns>Lines in all files, up to max of 20 lines.</returns>
        public IEnumerable<string> EnumerateDataInFolder(string folderName)
        {
            return this.EnumerateDataInFolder(folderName, 20);
        }

        /// <summary>
        /// Enumerates the data in folder.
        /// </summary>
        /// <param name="folderName">Name of the folder.</param>
        /// <param name="maxLines">Maximum lines to include in the enumeration.</param>
        /// <param name="fileNamePredicate">File name predicate.</param>
        /// <returns>Lines in all files that match predicate, up to maxLines total.</returns>
        public IEnumerable<string> EnumerateDataInFolder(string folderName, int maxLines, Func<string, bool> fileNamePredicate = null)
        {
            int linesRead = 0;
            foreach (string hdfsPath in this.LsFiles(folderName))
            {
                string tempPath = System.IO.Path.GetTempFileName();
                File.Delete(tempPath); // creation of temp path actually creates a file.. need to delete it before export from HDFS.
                this.CopyToLocal(hdfsPath, tempPath);
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
