namespace Microsoft.Hadoop.MapReduce.HdfsExtras.Hdfs
{
    using System;
    using System.Collections.Generic;

    public interface IHdfsFile
    {
        void WriteAllLines(string hdfsPath, IEnumerable<string> lines);
        void WriteAllText(string hdfsPath, string text);
        void WriteAllBytes(string hdfsPath, byte[] bytes);
        string[] ReadAllLines(string hdfsPath);
        string ReadAllText(string hdfsPath);
        byte[] ReadAllBytes(string hdfsPath);
        bool Exists(string hdfsPath);
        void MakeDirectory(string hdfsPath);
        void CopyToLocal(string hdfsPath, string localPath);
        void CopyFromLocal(string localPath, string hdfsPath);
        void Delete(string hdfsPath);
        string[] LsFiles(string hdfsPath);
        string GetAbsolutePath(string hdfsPath);
        string GetFullyQualifiedPath(string hdfsPath);
        IEnumerable<string> EnumerateDataInFolder(string folderName);
        IEnumerable<string> EnumerateDataInFolder(string folderName, int maxLines, Func<string, bool> fileNamePredicate = null);
    }
}
