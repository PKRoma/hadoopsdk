using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Hadoop.WebHDFS.Adapters;
using Microsoft.Hadoop.WebHDFS;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WebClientTests.Common
{
    internal class FileUploaderDownloader
    {
        private readonly TestContext testContext;

        private readonly BlobStorageAdapter adapter;
        readonly WebHDFSClient client;

        public FileUploaderDownloader(TestContext testContext, string accountName, string accountKey, string containerName)
        {
            this.testContext = testContext;

            adapter = new BlobStorageAdapter(accountName, accountKey, containerName, true);
            client = new WebHDFSClient("hadoop", adapter);
        }

        public void Delete(string src)
        {
            var deleted2 = client.DeleteDirectory(src);
            deleted2.Wait();
        }

        public void DeleteFolder(string src)
        {
            var deleted2 = client.DeleteDirectory(src, true);
            deleted2.Wait();
        }

        public void Download(string src, string localdst)
        {
            var fileRead = client.OpenFile(src);
            fileRead.Wait();
            var content = fileRead.Result.Content.ReadAsByteArrayAsync();
            content.Wait();

            System.IO.File.WriteAllBytes(localdst, content.Result);
        }

        public void Upload(string localSrc, string dst)
        {
            testContext.WriteLine("Uploading to " + dst);
            var createdFile = client.CreateFile(localSrc, dst);
            createdFile.Wait();

        }

        public void CreateDirectory(string src)
        {
            var directory = client.CreateDirectory(src);
            directory.Wait();
        }

        public string[] ReadFile(string fileFullPath)
        {
            var fileRead = client.OpenFile(fileFullPath);
            fileRead.Wait();
            var content = fileRead.Result.Content.ReadAsStringAsync();
            content.Wait();
            var results = content.Result;

            return results.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        }

        public void UploadDirectory(DirectoryInfo src, string dest)
        {
            foreach (var filePath in GetRelativePaths(src))
            {
                string source = Path.Combine(src.FullName, filePath);
                string target = string.Format("{0}/{1}", dest, filePath.Replace('\\', '/'));
                testContext.WriteLine("Uploading '{0}' to '{1}'", source, target);
                Upload(source, target);
            }
        }

        static IEnumerable<string> GetRelativePaths(DirectoryInfo source)
        {
            var files = source.GetFiles("*", SearchOption.AllDirectories);
            return files.Select(file => file.FullName.Replace(source.FullName, "").Substring(1));
        }

    }
}