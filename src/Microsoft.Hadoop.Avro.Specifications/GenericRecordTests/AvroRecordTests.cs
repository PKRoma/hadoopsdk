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
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public sealed class AvroRecordTests
    {
        [SuppressMessage(
            "Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", 
            MessageId = "Microsoft.Hadoop.Avro.AvroRecord",
            Justification = "Constructor is called only to see if it will throw")]
        [TestMethod]
        [TestCategory("CheckIn")]
        [ExpectedException(typeof(ArgumentException))]
        public void AvroRecord_CreateWithWrongSchema()
        {
            const string StringSchema = @"""float""";
            var serializer = AvroSerializer.CreateGeneric<AvroRecord>(StringSchema);
            var avroRecord = new AvroRecord(serializer.WriterSchema);
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AvroRecord_GetFieldWithNullName()
        {
            const string StringSchema = @"{
                             ""type"":""record"",
                             ""name"":""Microsoft.Hadoop.Avro.Specifications.SimpleIntClass"",
                             ""fields"":
                                       [
                                           { ""name"":""IntField"", ""type"":""int"" },
                                       ]
                          }";
            var serializer = AvroSerializer.CreateGeneric<AvroRecord>(StringSchema);
            dynamic avroRecord = new AvroRecord(serializer.WriterSchema);
            avroRecord[null] = 1;
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void AvroRecord_GetFieldWithInvalidName()
        {
            const string StringSchema = @"{
                             ""type"":""record"",
                             ""name"":""Microsoft.Hadoop.Avro.Specifications.SimpleIntClass"",
                             ""fields"":
                                       [
                                           { ""name"":""IntField"", ""type"":""int"" },
                                       ]
                          }";
            var serializer = AvroSerializer.CreateGeneric<AvroRecord>(StringSchema);
            dynamic avroRecord = new AvroRecord(serializer.WriterSchema);
            avroRecord["InvalidField"] = 1;
        }
    }
}
