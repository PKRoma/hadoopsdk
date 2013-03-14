namespace Microsoft.Hadoop.MapReduce.HdfsExtras.Hdfs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.IO;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Microsoft.Hadoop.WebHDFS;

    public class WebHdfsFile : HdfsFileBase, IHdfsFile
    {
        private WebHDFSClient client;
        private ITempPathGenerator pathGenerator;
        private string user;

        internal WebHdfsFile(string user, WebHDFSClient client, ITempPathGenerator pathGenerator)
        {
            this.client = client;
            this.pathGenerator = pathGenerator;
            this.user = user;
        }

        public static IHdfsFile Create(string user, WebHDFSClient hdfsClient)
        {
            return new WebHdfsFile(user, hdfsClient, new TempPathGenerator());
        }

        public override void WriteAllLines(string hdfsPath, IEnumerable<string> lines)
        {
            hdfsPath = this.GetAbsolutePath(hdfsPath);
            string tempPath = this.pathGenerator.GetTempPath();
            File.WriteAllLines(tempPath, lines);
            this.CopyFromLocal(tempPath, hdfsPath);
            File.Delete(tempPath);
        }

        public override void WriteAllText(string hdfsPath, string text)
        {
            hdfsPath = this.GetAbsolutePath(hdfsPath);
            string tempPath = this.pathGenerator.GetTempPath();
            File.WriteAllText(tempPath, text);
            this.CopyFromLocal(tempPath, hdfsPath);
            File.Delete(tempPath);
        }

        public override void WriteAllBytes(string hdfsPath, byte[] bytes)
        {
            hdfsPath = this.GetAbsolutePath(hdfsPath);
            string tempPath = this.pathGenerator.GetTempPath();
            File.WriteAllBytes(tempPath, bytes);
            this.CopyFromLocal(tempPath, hdfsPath);
            File.Delete(tempPath);
        }

        public override string[] ReadAllLines(string hdfsPath)
        {
            hdfsPath = this.GetAbsolutePath(hdfsPath);
            string tempPath = this.pathGenerator.GetTempPath();
            this.CopyToLocal(hdfsPath, tempPath);
            var retval = File.ReadAllLines(tempPath);
            File.Delete(tempPath);
            return retval;
        }

        public override string ReadAllText(string hdfsPath)
        {
            hdfsPath = this.GetAbsolutePath(hdfsPath);
            string tempPath = this.pathGenerator.GetTempPath();
            this.CopyToLocal(hdfsPath, tempPath);
            string retval = File.ReadAllText(tempPath);
            File.Delete(tempPath);
            return retval;
        }

        public override byte[] ReadAllBytes(string hdfsPath)
        {
            hdfsPath = this.GetAbsolutePath(hdfsPath);
            string tempPath = this.pathGenerator.GetTempPath();
            this.CopyToLocal(hdfsPath, tempPath);
            var retval = File.ReadAllBytes(tempPath);
            File.Delete(tempPath);
            return retval;
        }

        public override bool Exists(string hdfsPath)
        {
            hdfsPath = this.GetAbsolutePath(hdfsPath);
            // TODO: Rework so exception catch is not necessary. [tgs]
            try
            {
                var statusTask = this.client.GetFileStatus(hdfsPath);
                statusTask.Wait();
            }
            catch (AggregateException ex)
            {
                if (ex.InnerException.GetType() == typeof(HttpRequestException) &&
                                    ex.InnerException.Message.Contains("404 (Not Found)"))
                {
                    return false;
                }
                throw; 
            }
            return true;
        }

        public override void MakeDirectory(string hdfsPath)
        {
            hdfsPath = this.GetAbsolutePath(hdfsPath);
            this.client.CreateDirectory(hdfsPath).Wait();
        }

        public override void CopyToLocal(string hdfsPath, string localPath)
        {
            hdfsPath = this.GetAbsolutePath(hdfsPath);
            var result = this.client.OpenFile(hdfsPath);
            result.Wait();
            var dataRead = result.Result.Content.ReadAsStreamAsync();
            dataRead.Wait();
            using (var output = new FileStream(localPath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                dataRead.Result.CopyTo(output);
            }
        }

        public override void CopyFromLocal(string localPath, string hdfsPath)
        {
            hdfsPath = this.GetAbsolutePath(hdfsPath);
            this.client.CreateFile(localPath, hdfsPath).Wait();
        }

        public override void Delete(string hdfsPath)
        {
            hdfsPath = this.GetAbsolutePath(hdfsPath);
            this.client.DeleteDirectory(hdfsPath, true).Wait();
        }

        public override string[] LsFiles(string hdfsPath)
        {
            hdfsPath = this.GetAbsolutePath(hdfsPath);
            DirectoryListing results = this.client.GetDirectoryStatus(hdfsPath).WaitForResult();
            return results.Entries.Where(e => e.Type == "FILE").Select(e => hdfsPath + "/" + e.PathSuffix).ToArray();
        }

        public override string GetAbsolutePath(string hdfsPath)
        {
            return this.client.GetAbsolutePath(hdfsPath);
        }

        public override string GetFullyQualifiedPath(string hdfsPath)
        {
            return this.client.GetFullyQualifiedPath(hdfsPath);
        }
    }
}
