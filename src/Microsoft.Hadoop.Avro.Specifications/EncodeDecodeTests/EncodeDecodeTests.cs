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
namespace Microsoft.Hadoop.Avro.Specifications
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Runtime.Serialization;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable",
        Justification = "This should be fixed when we can bring in disposable object. [tgs]")]
    [TestClass]
    public class EncodeDecodeTests
    {
        private MemoryStream stream;
        private IEncoder encoder;
        private IDecoder decoder;
        private Random random;

        [TestInitialize]
        public void TestSetup()
        {
            const int Seed = 13;
            this.stream = new MemoryStream();
            this.encoder = this.CreateEncoder(this.stream);
            this.decoder = this.CreateDecoder(this.stream);
            this.random = new Random(Seed);
        }

        [TestCleanup]
        public void TestTeardown()
        {
            this.stream.Dispose();
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        public void EncodeDecode_ZeroInt()
        {
            const int Expected = 0;
            this.encoder.Encode(Expected);
            this.encoder.Flush();

            this.stream.Seek(0, SeekOrigin.Begin);
            var actual = this.decoder.DecodeInt();

            Assert.AreEqual(Expected, actual);
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        public void EncodeDecode_PositiveInt()
        {
            const int Expected = 105;
            this.encoder.Encode(Expected);
            this.encoder.Flush();

            this.stream.Seek(0, SeekOrigin.Begin);
            var actual = this.decoder.DecodeInt();

            Assert.AreEqual(Expected, actual);
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        public void EncodeDecode_NegativeInt()
        {
            const int Expected = -106;
            this.encoder.Encode(Expected);
            this.encoder.Flush();

            this.stream.Seek(0, SeekOrigin.Begin);
            var actual = this.decoder.DecodeInt();

            Assert.AreEqual(Expected, actual);
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        public void EncodeDecode_MaxInt()
        {
            const int Expected = int.MaxValue;
            this.encoder.Encode(Expected);
            this.encoder.Flush();

            this.stream.Seek(0, SeekOrigin.Begin);
            var actual = this.decoder.DecodeInt();

            Assert.AreEqual(Expected, actual);
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        public void EncodeDecode_MinInt()
        {
            const int Expected = int.MinValue;
            this.encoder.Encode(Expected);
            this.encoder.Flush();

            this.stream.Seek(0, SeekOrigin.Begin);
            var actual = this.decoder.DecodeInt();

            Assert.AreEqual(Expected, actual);
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [ExpectedException(typeof(SerializationException))]
        public void Decode_InvalidInt()
        {
            this.stream.WriteByte(0xFF);
            this.stream.WriteByte(0xFF);
            this.stream.WriteByte(0xFF);
            this.stream.WriteByte(0xFF);
            //causes corruption
            this.stream.WriteByte(0xFF);
            this.stream.WriteByte(0x1);
            this.stream.Flush();
            this.stream.Seek(0, SeekOrigin.Begin);
            var result = this.decoder.DecodeInt();
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        public void EncodeDecode_ZeroLong()
        {
            const long Expected = 0;
            this.encoder.Encode(Expected);
            this.encoder.Flush();

            this.stream.Seek(0, SeekOrigin.Begin);
            var actual = this.decoder.DecodeLong();

            Assert.AreEqual(Expected, actual);
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        public void EncodeDecode_PositiveLong()
        {
            const long Expected = 105;
            this.encoder.Encode(Expected);
            this.encoder.Flush();

            this.stream.Seek(0, SeekOrigin.Begin);
            var actual = this.decoder.DecodeLong();

            Assert.AreEqual(Expected, actual);
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        public void EncodeDecode_NegativeLong()
        {
            const long Expected = -106;
            this.encoder.Encode(Expected);
            this.encoder.Flush();

            this.stream.Seek(0, SeekOrigin.Begin);
            var actual = this.decoder.DecodeLong();

            Assert.AreEqual(Expected, actual);
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        public void EncodeDecode_MaxLong()
        {
            const long Expected = long.MaxValue;
            this.encoder.Encode(Expected);
            this.encoder.Flush();

            this.stream.Seek(0, SeekOrigin.Begin);
            var actual = this.decoder.DecodeLong();

            Assert.AreEqual(Expected, actual);
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        public void EncodeDecode_MinLong()
        {
            const long Expected = long.MinValue;
            this.encoder.Encode(Expected);
            this.encoder.Flush();

            this.stream.Seek(0, SeekOrigin.Begin);
            var actual = this.decoder.DecodeLong();

            Assert.AreEqual(Expected, actual);
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [ExpectedException(typeof(SerializationException))]
        public void Decode_InvalidLong()
        {
            this.stream.WriteByte(0xFF);
            this.stream.WriteByte(0xFF);
            this.stream.WriteByte(0xFF);
            this.stream.WriteByte(0xFF);
            this.stream.WriteByte(0xFF);
            this.stream.WriteByte(0xFF);
            this.stream.WriteByte(0xFF);
            this.stream.WriteByte(0xFF);
            this.stream.WriteByte(0xFF);
            //causes corruption
            this.stream.WriteByte(0xFF);
            this.stream.WriteByte(0x1);
            this.stream.Flush();
            this.stream.Seek(0, SeekOrigin.Begin);
            var result = this.decoder.DecodeLong();
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        public void EncodeDecode_BooleanTrue()
        {
            const bool Expected = true;
            this.encoder.Encode(Expected);
            this.encoder.Flush();

            this.stream.Seek(0, SeekOrigin.Begin);
            var actual = this.decoder.DecodeBool();

            Assert.AreEqual(Expected, actual);
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        public void EncodeDecode_BooleanFalse()
        {
            const bool Expected = false;
            this.encoder.Encode(Expected);
            this.encoder.Flush();

            this.stream.Seek(0, SeekOrigin.Begin);
            var actual = this.decoder.DecodeBool();

            Assert.AreEqual(Expected, actual);
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        public void EncodeDecode_EmptyByteArray()
        {
            var expected = new byte[] { };
            this.encoder.Encode(expected);
            this.encoder.Flush();

            this.stream.Seek(0, SeekOrigin.Begin);
            var actual = this.decoder.DecodeByteArray();

            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        public void EncodeDecode_NotEmptyByteArray()
        {
            var expected = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            this.encoder.Encode(expected);
            this.encoder.Flush();

            this.stream.Seek(0, SeekOrigin.Begin);
            var actual = this.decoder.DecodeByteArray();

            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        public void EncodeDecode_FloatMax()
        {
            const float Expected = float.MaxValue;
            this.encoder.Encode(Expected);
            this.encoder.Flush();

            this.stream.Seek(0, SeekOrigin.Begin);
            var actual = this.decoder.DecodeFloat();

            Assert.AreEqual(Expected, actual);
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        public void EncodeDecode_FloatMin()
        {
            const float Expected = float.MinValue;
            this.encoder.Encode(Expected);
            this.encoder.Flush();

            this.stream.Seek(0, SeekOrigin.Begin);
            var actual = this.decoder.DecodeFloat();

            Assert.AreEqual(Expected, actual);
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        public void EncodeDecode_DoubleMax()
        {
            const double Expected = double.MaxValue;
            this.encoder.Encode(Expected);
            this.encoder.Flush();

            this.stream.Seek(0, SeekOrigin.Begin);
            var actual = this.decoder.DecodeDouble();

            Assert.AreEqual(Expected, actual);
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        public void EncodeDecode_DoubleMin()
        {
            const double Expected = double.MinValue;
            this.encoder.Encode(Expected);
            this.encoder.Flush();

            this.stream.Seek(0, SeekOrigin.Begin);
            var actual = this.decoder.DecodeDouble();

            Assert.AreEqual(Expected, actual);
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        public void EncodeDecode_EmptyString()
        {
            const string Expected = "";
            this.encoder.Encode(Expected);
            this.encoder.Flush();

            this.stream.Seek(0, SeekOrigin.Begin);
            var actual = this.decoder.DecodeString();

            Assert.AreEqual(Expected, actual);
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        public void EncodeDecode_NotEmptyString()
        {
            const string Expected = "Test string";
            this.encoder.Encode(Expected);
            this.encoder.Flush();

            this.stream.Seek(0, SeekOrigin.Begin);
            var actual = this.decoder.DecodeString();

            Assert.AreEqual(Expected, actual);
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        public void EncodeDecode_HundredThousandRandomInts()
        {
            const int NumberOfTests = 100000;
            const int Seed = 13;
            var random = new Random(Seed);

            for (var i = 0; i < NumberOfTests; ++i)
            {
                this.stream.SetLength(0);

                var expected = random.Next(int.MinValue, int.MaxValue);
                this.encoder.Encode(expected);
                this.encoder.Flush();

                this.stream.Seek(0, SeekOrigin.Begin);
                var actual = this.decoder.DecodeInt();

                Assert.AreEqual(expected, actual);
            }
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        public void EncodeDecode_HundredThousandRandomLongs()
        {
            const int NumberOfTests = 100000;
            const int Seed = 13;
            var random = new Random(Seed);

            var buffer = new byte[8];
            for (var i = 0; i < NumberOfTests; ++i)
            {
                random.NextBytes(buffer);
                var expected = BitConverter.ToInt64(buffer, 0);

                this.stream.SetLength(0);
                this.encoder.Encode(expected);
                this.encoder.Flush();

                this.stream.Seek(0, SeekOrigin.Begin);
                var actual = this.decoder.DecodeLong();

                Assert.AreEqual(expected, actual);
            }
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        public void EncodeDecode_DifferentInts()
        {
            var values = new[]
                         {
                             new byte[] { 0, 0, 0, 1 },
                             new byte[] { 0, 0, 1, 0 },
                             new byte[] { 0, 1, 0, 0 },
                             new byte[] { 1, 0, 0, 0 }
                         };

            foreach (var value in values)
            {
                this.stream.SetLength(0);

                var expected = BitConverter.ToInt32(value, 0);
                this.encoder.Encode(expected);
                this.encoder.Flush();

                this.stream.Seek(0, SeekOrigin.Begin);

                var actual = this.decoder.DecodeInt();
                Assert.AreEqual(expected, actual);
            }
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        public void EncodeDecode_DifferentLongs()
        {
            var values = new[]
                         {
                             new byte[] { 0, 0, 0, 0, 0, 0, 0, 1 },
                             new byte[] { 0, 0, 0, 0, 0, 0, 1, 0 },
                             new byte[] { 0, 0, 0, 0, 0, 1, 0, 0 },
                             new byte[] { 0, 0, 0, 0, 1, 0, 0, 0 },
                             new byte[] { 0, 0, 0, 1, 0, 0, 0, 0 },
                             new byte[] { 0, 0, 1, 0, 0, 0, 0, 0 },
                             new byte[] { 0, 1, 0, 0, 0, 0, 0, 0 },
                             new byte[] { 1, 0, 0, 0, 0, 0, 0, 0 }
                         };

            foreach (var value in values)
            {
                this.stream.SetLength(0);

                var expected = BitConverter.ToInt64(value, 0);
                this.encoder.Encode(expected);
                this.encoder.Flush();

                this.stream.Seek(0, SeekOrigin.Begin);

                var actual = this.decoder.DecodeLong();
                Assert.AreEqual(expected, actual);
            }
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        public void Skip_Double()
        {
            var valueToSkip = this.random.NextDouble();
            this.CheckSkipping(
                e => e.Encode(valueToSkip),
                d => d.SkipDouble());
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        public void Skip_Float()
        {
            var valueToSkip = (float)this.random.NextDouble();
            this.CheckSkipping(
                e => e.Encode(valueToSkip),
                d => d.SkipFloat());
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        public void Skip_Bool()
        {
            var valueToSkip = this.random.Next(0, 100) % 2 == 1;
            this.CheckSkipping(
                e => e.Encode(valueToSkip),
                d => d.SkipBool());
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        public void Skip_Int()
        {
            var valueToSkip = this.random.Next();
            this.CheckSkipping(
                e => e.Encode(valueToSkip),
                d => d.SkipInt());
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        public void Skip_Long()
        {
            long valueToSkip = this.random.Next();
            this.CheckSkipping(
                e => e.Encode(valueToSkip),
                d => d.SkipLong());
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        public void Skip_ByteArray()
        {
            var valueToSkip = new byte[128];
            this.random.NextBytes(valueToSkip);

            this.CheckSkipping(
                e => e.Encode(valueToSkip),
                d => d.SkipByteArray());
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        public void Skip_String()
        {
            this.CheckSkipping(
                e => e.Encode("test string" + this.random.Next(0, 100)),
                d => d.SkipString());
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Encoder_NullByteArray()
        {
            this.encoder.Encode((byte[])null);
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Encode_NullStream()
        {
            this.encoder.Encode((Stream)null);
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Encode_NullFixed()
        {
            this.encoder.EncodeFixed(null);
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Decode_FixedWithNegativeSize()
        {
            this.decoder.DecodeFixed(-1);
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Encode_NullString()
        {
            this.encoder.Encode((string)null);
        }

        #region Helper methods
        internal virtual IEncoder CreateEncoder(Stream stream)
        {
            return new BinaryEncoder(stream);
        }

        internal virtual IDecoder CreateDecoder(Stream stream)
        {
            return new BinaryDecoder(stream);
        }

        private void CheckSkipping(Action<IEncoder> encode, Action<IDecoder> skip)
        {
            var startGuard = this.random.Next();
            var endGuard = this.random.Next();

            this.encoder.Encode(startGuard);
            encode(this.encoder);
            this.encoder.Encode(endGuard);
            this.encoder.Flush();
            this.stream.Seek(0, SeekOrigin.Begin);

            Assert.AreEqual(startGuard, this.decoder.DecodeInt());
            skip(this.decoder);
            Assert.AreEqual(endGuard, this.decoder.DecodeInt());
        }
        #endregion
    }
}
