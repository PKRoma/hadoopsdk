using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace Microsoft.Hadoop.MapReduce.Test
{
    [TestClass]
    public class HdfsTests
    {
        private string hdfsFolderPath = "input/unittest/HdfsFileTests";
        private string hdfsFilePath = "input/unittest/HdfsFileTests/file0.txt";
        private void EnsureTestFileExists()
        {
            if (!HdfsFile.Exists(hdfsFilePath))
            {
                HdfsFile.WriteAllLines(hdfsFilePath, new[] { "hello", "world" });
            }
        }

        [TestMethod]
        public void Delete()
        {
            EnsureTestFileExists();
            HdfsFile.Delete(hdfsFilePath);
            EnsureTestFileExists();
            Assert.IsTrue(HdfsFile.Exists(hdfsFilePath));
        }

        [TestMethod]
        public void Exists()
        {
            EnsureTestFileExists();
            Assert.IsTrue(HdfsFile.Exists(hdfsFilePath));
        }

        [TestMethod]
        public void EnumerateDataInFolder()
        {
            EnsureTestFileExists();
            foreach (string line in HdfsFile.EnumerateDataInFolder(hdfsFolderPath))
            {
                Console.WriteLine(line);
            }
        }

        [TestMethod]
        public void EnumerateFiles()
        {
            EnsureTestFileExists();
            foreach (string filePath in HdfsFile.EnumerateFilesInFolder(hdfsFolderPath))
            {
                Console.WriteLine(filePath);
            }
        }

        [TestMethod]
        public void CopyToLocal()
        {
            string tempPath = Path.GetTempFileName();
            HdfsFile.CopyToLocal("/user/mikelid/input/Textsort/input0.bin", tempPath);
            Assert.IsTrue(File.Exists(tempPath));
        }
    }
}
