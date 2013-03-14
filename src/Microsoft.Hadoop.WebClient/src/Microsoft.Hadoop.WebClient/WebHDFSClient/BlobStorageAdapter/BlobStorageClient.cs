using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.RetryPolicies;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Shared.Protocol;

namespace Microsoft.Hadoop.WebHDFS.Adapters
{
    internal enum BlobType
    {
        File,
        Directory
    }

    internal class BlobDescriptor
    {
        internal string Name;
        internal BlobType Type;
        internal long Length;
    }

    internal class BlobStorageClient
    {
        private string storageAccountConnectionString = @"BlobEndpoint={2};AccountName={0};AccountKey={1}";

        private CloudStorageAccount storageAccount;

        private CloudBlobClient blobStorage;

        private CloudBlobContainer blobContainer;

        private const string IsFolderProp = "asv_isfolder";

        internal BlobStorageClient(string baseUri, string account, string key)
        {
            storageAccount = CloudStorageAccount.Parse(string.Format(storageAccountConnectionString, account, key, baseUri));
            blobStorage = storageAccount.CreateCloudBlobClient();
            //blobStorage.RetryPolicy = new ExponentialRetry(TimeSpan.FromMilliseconds(100),20);
        }

        internal void SetContainer(string containerName, bool createContainerIfDoesNotExist)
        {
            this.blobContainer = blobStorage.GetContainerReference(containerName);
                this.blobContainer.CreateIfNotExists();
        }

        internal void DeleteContainer()
        {
            if (this.blobContainer != null)
            {
                blobContainer.DeleteIfExists();
                blobContainer = null;
            }
        }

        internal void CreateFile(string path, Stream contents, bool overwrite)
        {
            // before we create the file, need to make sure directories exist
            if (String.IsNullOrEmpty(path))
                throw new Exception("Invalid Path");

            this.CreateParentPathIfDoesNotExist(path);

            var blob = this.blobContainer.GetBlockBlobReference(path);
            blob.UploadFromStream(contents);
        }

        internal void OpenFile(string path, Stream contents, out string contentType, out long length)
        {
            var blob = this.blobContainer.GetBlockBlobReference(path);
            blob.FetchAttributes();
            length = blob.Properties.Length;
            contentType = blob.Properties.ContentType;
            blob.DownloadToStream(contents);
        }

        internal void DeleteFile(string path)
        {
            // create parent directory if does not exist  ASV semantics
            this.CreateParentPathIfDoesNotExist(path);

            var desc = GetBlobDescriptor(path);
            if (desc != null && desc.Type == BlobType.File)
            {
                var blob = blobContainer.GetBlockBlobReference(path);
                blob.DeleteIfExists();
            }
            else
                throw new Exception("File does not exist.");
        }

        internal void CreateDirectory(string path)
        {
            this.CreateParentPathIfDoesNotExist(path);
          
            // check to see if blob with same name exists
            var desc = GetBlobDescriptor(path);
            if (desc != null)
            {
                if (desc.Type == BlobType.Directory)
                    return;
                else
                     throw new Exception("Can not create folder where file of same name already exists.");
            }

            var blob = this.blobContainer.GetBlockBlobReference(path);
            blob.Metadata.Add(IsFolderProp, "true");

            blob.UploadFromStream(new MemoryStream(new byte[] {}));
        }

        internal void DeleteDirectory(string path, bool recursive)
        {
            var desc = GetBlobDescriptor(path);

            if (desc == null)
                return;

            var children = ListDirectory(path +"/");

            if (!recursive)
            {
                // file 
                if (desc.Type == BlobType.File)
                    return;
                if (children.Any())
                    throw new Exception("Can not delete directory that is not empty.");
            }
            else
            {
                foreach (var child in children)
                {
                    if (child.Type == BlobType.Directory)
                    {
                        DeleteDirectory(child.Name, true);
                    }
                    else if (child.Type == BlobType.File)
                    {
                        DeleteFile(child.Name);
                    }
                }
            }

            Debug.Assert(!ListDirectory(path + "/").Any());
            var blob = this.blobContainer.GetBlockBlobReference(path);
            blob.DeleteIfExists();
        }

        internal BlobType? GetTypeIfExists(string path)
        {
            var bd = GetBlobDescriptor(path);
            return bd != null ? bd.Type : (BlobType?) null;
        }

        internal IEnumerable<BlobDescriptor> ListDirectory(string path)
        {
            var blobs = blobContainer.ListBlobs(path, false, BlobListingDetails.Metadata);

            var descs = new List<BlobDescriptor>();
            foreach (ICloudBlob blob in blobs.Where(b => !(b is CloudBlobDirectory)))
            {
                descs.Add(CreateDescriptorFromProperities(blob));
            }

            return descs;
        }

        internal BlobDescriptor GetBlobDescriptor(string path)
        {
            BlobDescriptor desc = null;
            if (string.IsNullOrEmpty(path)) // root - no blob for it
            {
                return new BlobDescriptor() { Name = path, Type = BlobType.Directory, Length = 0 };
            }
            else
            {
                var blob = blobContainer.GetBlockBlobReference(path);
                if (CheckIfBlobExist(blob))
                {
                    desc = blob.Properties != null ? CreateDescriptorFromProperities(blob) : null;
                }
            }
            return desc;
        }

        private bool CheckIfBlobExist(CloudBlockBlob blob)
        {
            try
            {
                blob.FetchAttributes();
                return true;
            }
            catch (StorageException e)
            {
               if (e.RequestInformation.HttpStatusCode != 404)
               {
                   throw;
               }
            }
            return false;
        }

        private BlobDescriptor CreateDescriptorFromProperities(ICloudBlob bp)
        {
            Debug.Assert(bp != null);
            return new BlobDescriptor() { Name = bp.Name, 
                                          Type = bp.Metadata.Keys.Any(k => k == IsFolderProp) ? BlobType.Directory : BlobType.File,
                                          Length = bp.Properties.Length};
        }

        private string[] GetSegmentsForPath(string path)
        {
            return path.Split(Path.AltDirectorySeparatorChar);
        }

        private string GetParentPath(string path)
        {
            var segments = GetSegmentsForPath(path);

            var count = segments.Count();

            if (count <= 1)
                return String.Empty;
            
            var directoires = segments.Take(count - 1).ToArray();
            return string.Join(Path.AltDirectorySeparatorChar.ToString(), directoires);
        }

        private bool IsInCurrentDirectory(string directoryPath, string path)
        {
            return !String.Equals(directoryPath, path, StringComparison.OrdinalIgnoreCase) && 
                    String.Equals(directoryPath, this.GetParentPath(path), StringComparison.OrdinalIgnoreCase);
        }

        private void CreateParentPathIfDoesNotExist(string path)
        {
            string parentPath = this.GetParentPath(path);

            if (!String.IsNullOrEmpty(parentPath))
                this.CreateDirectory(parentPath);
        }
    }

}
