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
namespace Microsoft.Hadoop.Avro.Container
{
    using System;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.IO.Compression;

    /// <summary>
    /// Wrapper around a stream supporting compression on write and read of compressed data after flush.
    /// </summary>
    internal sealed class CompressionStream : Stream
    {
        private readonly Stream buffer;
        private DeflateStream compressionStream;

        public CompressionStream(Stream buffer)
        {
            Contract.Assert(buffer != null);

            this.compressionStream = new DeflateStream(buffer, CompressionMode.Compress, true);
            this.buffer = buffer;
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return true; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override void Flush()
        {
            this.compressionStream.Close();
        }

        public override long Length
        {
            get { return this.buffer.Length; }
        }

        public override long Position
        {
            get
            {
                return this.buffer.Position;
            }

            set
            {
                this.buffer.Position = value;
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return this.buffer.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return this.buffer.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            this.compressionStream.Write(buffer, offset, count);
        }

        protected override void Dispose(bool disposed)
        {
            base.Dispose(disposed);

            if (disposed)
            {
                this.compressionStream.Dispose();
                this.compressionStream = null;
            }
        }
    }
}
