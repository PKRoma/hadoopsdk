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
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Microsoft.CSharp.RuntimeBinder;
    using Microsoft.Hadoop.Avro;
    using Microsoft.Hadoop.Avro.Schema;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public sealed class RecordSerializerTests
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity",
            Justification = "Need to test complex objects."), TestMethod]
        [TestCategory("CheckIn")]
        public void RecordSerializer_SerializeSimpleRecord()
        {
            const string StringSchema = @"{
                             ""type"":""record"",
                             ""name"":""Microsoft.Hadoop.Avro.Specifications.SimpleIntClass"",
                             ""fields"":
                                       [
                                           { ""name"":""IntField"", ""type"":""int"" },
                                           { ""name"":""MyGuid"", ""type"": {""type"":""fixed"", ""size"":4, ""name"": ""q"" } },
                                           { ""name"":""Arr"", ""type"": {""type"":""array"", ""items"":""int""}},
                                           { ""name"":""LongField"", ""type"": ""long""},
                                           {""name"":""LongMap"", ""type"": {""type"":""map"", ""values"":""long""}},
                                           { ""name"":""DoubleField"", ""type"": ""double""},
                                           { ""name"":""FloatField"", ""type"": ""float""},
                                           { ""name"":""BooleanField"", ""type"": ""boolean""},
                                           { ""name"":""BytesField"", ""type"": ""bytes""},
                                       ]
                          }";
            var serializer = AvroSerializer.CreateGeneric<AvroRecord>(StringSchema);

            using (var stream = new MemoryStream())
            {
                dynamic expected = new AvroRecord(serializer.WriterSchema);
                expected.IntField = 5;
                expected.MyGuid = new byte[] { 1, 2, 3, 4 };
                expected.Arr = new int[] { 1, 2, 5 };
                expected.LongField = (long)14;
                expected.LongMap = new Dictionary<string, long> { { "test", 1 } };
                expected.DoubleField = (double)3;
                expected.FloatField = (float)4;
                expected.BooleanField = true;
                expected.BytesField = new byte[3] { 4, 5, 6 };

                serializer.Serialize(stream, expected);
                stream.Position = 0;

                dynamic actual = serializer.Deserialize(stream);
                Assert.AreEqual(expected.IntField, actual.IntField);
                CollectionAssert.AreEqual(expected.MyGuid, actual.MyGuid);
                CollectionAssert.AreEqual(expected.Arr, actual.Arr);
                Assert.AreEqual(expected.LongField, actual.LongField);
                Utilities.DictionaryEquals(expected.LongMap as IDictionary<string, long>, actual.LongMap as IDictionary<string, long>);
                Assert.AreEqual(expected.DoubleField, actual.DoubleField);
                Assert.AreEqual(expected.FloatField, actual.FloatField);
                Assert.AreEqual(expected.BooleanField, actual.BooleanField);
                CollectionAssert.AreEqual(expected.BytesField, actual.BytesField);
            }
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        public void RecordSerializer_SerializeRecursiveRecord()
        {
            const string StringSchema = @"{
                      ""type"":""record"",
                      ""name"":""Microsoft.Hadoop.Avro.Specifications.Recursive"",
                      ""fields"":[
                                     {""name"":""IntField"",""type"":""int""},
                                     {""name"":""RecursiveField"",""type"":[
                                                                               ""null"",
                                                                               ""Microsoft.Hadoop.Avro.Specifications.Recursive""
                                                                           ]
                                     }
                                 ]}";

            var serializer = AvroSerializer.CreateGeneric<AvroRecord>(StringSchema);
            using (var stream = new MemoryStream())
            {
                dynamic expected = new AvroRecord(serializer.WriterSchema);
                expected.IntField = 5;
                expected.RecursiveField = new AvroRecord(((serializer.ReaderSchema as RecordSchema).GetField("RecursiveField").TypeSchema as UnionSchema).Schemas[1]);
                expected.RecursiveField.IntField = 3;
                expected.RecursiveField.RecursiveField = null;

                serializer.Serialize(stream, expected);
                stream.Position = 0;

                var actual = serializer.Deserialize(stream);
                Assert.IsTrue(ShallowlyEqual(expected, actual));
            }
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        public void RecordSerializer_SerializeNestedRecord()
        {
            const string StringSchema = @"{
                                        ""type"":""record"",
                                        ""name"":""Microsoft.Hadoop.Avro.Specifications.NestedClass"",
                                        ""fields"":[
                                            {
                                                ""name"":""Nested"",
                                                ""type"":[
                                                    ""null"",
                                                    {
                                                        ""type"":""record"",
                                                        ""name"":""Microsoft.Hadoop.Avro.Specifications.SimpleIntClass"",
                                                        ""fields"":[
                                                            {
                                                                 ""name"":""IntField"",
                                                                 ""type"":""int""
                                                            }
                                                        ]
                                                    }
                                                ]
                                            },
                                            {
                                                ""name"":""IntNestedField"",""type"":""int""
                                            }
                                        ]
                                    }";

            var serializer = AvroSerializer.CreateGeneric<AvroRecord>(StringSchema);
            var nested = serializer.ReaderSchema as RecordSchema;
            using (var stream = new MemoryStream())
            {
                dynamic expected = new AvroRecord(nested);
                expected.IntNestedField = 5;
                expected.Nested = new AvroRecord((nested.GetField("Nested").TypeSchema as UnionSchema).Schemas[1]);
                expected.Nested.IntField = 3;

                serializer.Serialize(stream, expected);
                stream.Position = 0;

                var actual = serializer.Deserialize(stream);
                Assert.AreEqual(expected.GetField<AvroRecord>("Nested").GetField<int>("IntField"),
                                actual.GetField<AvroRecord>("Nested").GetField<int>("IntField"));
            }
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        public void RecordSerializer_SerializeUsingDifferentReaderWriterType()
        {
            const string StringSchema = @"{
                             ""type"":""record"",
                             ""name"":""Microsoft.Hadoop.Avro.Specifications.SimpleIntClass"",
                             ""fields"":
                                       [
                                           {
                                               ""name"":""IntField"",
                                               ""type"":""int""
                                           }
                                       ]
                          }";
            var serializer = AvroSerializer.CreateGeneric<AvroRecord>(StringSchema);
            var deserializer = AvroSerializer.CreateGenericDeserializerOnly<AvroRecord>(StringSchema, StringSchema);

            using (var stream = new MemoryStream())
            {
                dynamic expected = new AvroRecord(serializer.WriterSchema);
                expected.IntField = 5;

                serializer.Serialize(stream, expected);
                stream.Position = 0;

                var actual = deserializer.Deserialize(stream);
                Assert.IsTrue(expected["IntField"].Equals(actual["IntField"]));
            }
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        public void RecordSerializer_SerializeRecordWithUnion()
        {
            const string StringSchema = @"{
                 ""name"":""Category"",
                 ""namespace"":""ApacheAvro.Types"",
                 ""type"":""record"",
                 ""fields"":
                           [
                                {""name"":""CategoryName"", ""type"":""string""},
                                {""name"":""Description"", ""type"":[""string"",""null""]},
                                {""name"":""Picture"", ""type"":[""bytes"", ""null""]},
                                {""name"":""Id"", ""type"":[""int"", ""null""]}
                           ]
             }";

            var serializer = AvroSerializer.CreateGeneric<AvroRecord>(StringSchema);
            using (var stream = new MemoryStream())
            {
                dynamic expected = new AvroRecord(serializer.WriterSchema);
                expected.CategoryName = "Test";
                expected.Description = "Test";
                expected.Picture = null;
                expected.Id = 1;

                serializer.Serialize(stream, expected);
                stream.Seek(0, SeekOrigin.Begin);

                var actual = serializer.Deserialize(stream);
                Assert.IsTrue(ShallowlyEqual(expected, actual));
            }
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        public void RecordSerializer_SerializeNestedRecordWithReferences()
        {
            const string StringSchema = 
            @"{
                  ""name"":""MultiOrderProperties"",
                  ""namespace"":""ApacheAvro.Types"",
                  ""type"":""record"",
                  ""fields"":
                  [
                      {
                          ""name"":""Orders1"",
                          ""type"":
                          {
                              ""name"":""Order"",
                              ""namespace"":""ApacheAvro.Types"",
                              ""type"":""record"",
                              ""fields"":
                              [
                                  {""name"":""CustomerId"", ""type"":""string""},
                                  {""name"":""EmployeeId"", ""type"":""int""},
                              ]
                          }
                      },
                      {""name"":""Orders2"", ""type"":""Order""},
                      {""name"":""Orders3"", ""type"":""Order""},
                      {""name"":""Orders4"", ""type"":""Order""},
                  ]
            }";

            var serializer = AvroSerializer.CreateGeneric<AvroRecord>(StringSchema);
            var recordSchema = serializer.ReaderSchema as RecordSchema;
            Assert.IsNotNull(recordSchema);
            using (var stream = new MemoryStream())
            {
                dynamic expected = new AvroRecord(serializer.ReaderSchema);
                expected.Orders1 = new AvroRecord(recordSchema.GetField("Orders1").TypeSchema);
                expected.Orders1.CustomerId = "1";
                expected.Orders1.EmployeeId = 1;
                expected.Orders2 = new AvroRecord(recordSchema.GetField("Orders2").TypeSchema);
                expected.Orders2.CustomerId = "1";
                expected.Orders2.EmployeeId = 2;
                expected.Orders3 = new AvroRecord(recordSchema.GetField("Orders3").TypeSchema);
                expected.Orders3.CustomerId = "1";
                expected.Orders3.EmployeeId = 3;
                expected.Orders4 = new AvroRecord(recordSchema.GetField("Orders4").TypeSchema);
                expected.Orders4.CustomerId = "1";
                expected.Orders4.EmployeeId = 4;

                serializer.Serialize(stream, expected);
                stream.Seek(0, SeekOrigin.Begin);

                var actual = serializer.Deserialize(stream);
                Assert.IsTrue(ShallowlyEqual(expected, actual));
            }
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        public void RecordSerializer_SerializeEvolvedRecordWithExtraFieldHavingDefaultValue()
        {
            const string WriterSchema = @"{
                             ""type"":""record"",
                             ""name"":""Microsoft.Hadoop.Avro.Specifications.SimpleIntClass"",
                             ""fields"":
                                       [
                                           {""name"":""IntField"", ""type"":""int""}
                                       ]
                          }";

            const string ReaderSchema = @"{
                            ""type"":""record"",
                            ""name"":""Microsoft.Hadoop.Avro.Specifications.SimpleIntClass"",
                            ""fields"":
                                    [
                                        {""name"":""IntField"",""type"":""int""},
                                        {""name"":""ExtraField"",""type"":""string"",""default"":""Default""}
                                    ]
                        }";

            var serializer = AvroSerializer.CreateGeneric<AvroRecord>(WriterSchema);
            var deserializer = AvroSerializer.CreateGenericDeserializerOnly<AvroRecord>(WriterSchema, ReaderSchema);

            using (var stream = new MemoryStream())
            {
                dynamic expected = new AvroRecord(serializer.ReaderSchema);
                expected.IntField = Utilities.GetRandom<int>(false);

                serializer.Serialize(stream, expected);
                stream.Seek(0, SeekOrigin.Begin);

                dynamic actual = deserializer.Deserialize(stream);
                Assert.AreEqual(expected.IntField, actual.IntField);
                Assert.AreEqual("Default", actual.ExtraField);
            }
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        public void RecordSerializer_SerializeEvolvedRecordWithPromotedField()
        {
            const string WriterSchema = @"{
                             ""type"":""record"",
                             ""name"":""Microsoft.Hadoop.Avro.Specifications.SimpleIntClass"",
                             ""fields"":
                                       [
                                           {""name"":""IntField"", ""type"":""int""}
                                       ]
                          }";

            const string ReaderSchema = @"{
                            ""type"":""record"",
                            ""name"":""Microsoft.Hadoop.Avro.Specifications.SimpleIntClass"",
                            ""fields"":
                                    [
                                        {""name"":""IntField"",""type"":""long""}
                                    ]
                        }";

            var serializer = AvroSerializer.CreateGeneric<AvroRecord>(WriterSchema);
            var deserializer = AvroSerializer.CreateGenericDeserializerOnly<AvroRecord>(WriterSchema, ReaderSchema);

            using (var stream = new MemoryStream())
            {
                dynamic expected = new AvroRecord(serializer.ReaderSchema);
                expected.IntField = 1;

                serializer.Serialize(stream, expected);
                stream.Seek(0, SeekOrigin.Begin);

                dynamic actual = deserializer.Deserialize(stream);
                Assert.AreEqual(expected.IntField, actual.IntField);
            }
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        public void RecordSerializer_SerializeEnum()
        {
            const string Schema = "{" +
                                  "\"type\":\"enum\"," +
                                  "\"name\":\"Microsoft.Hadoop.Avro.Specifications.TestEnum\"," +
                                  "\"symbols\":" +
                                  "[" +
                                  "\"EnumValue3\"," +
                                  "\"EnumValue2\"," +
                                  "\"EnumValue1\"" +
                                  "]}";
            var serializer = AvroSerializer.CreateGeneric<AvroEnum>(Schema);

            using (var stream = new MemoryStream())
            {
                dynamic expected = new AvroEnum(serializer.WriterSchema);
                expected.IntegerValue = 0;

                serializer.Serialize(stream, expected);
                stream.Seek(0, SeekOrigin.Begin);

                dynamic actual = serializer.Deserialize(stream);
                Assert.AreEqual(expected.IntegerValue, actual.IntegerValue);
                Assert.AreEqual(expected.Value, actual.Value);
            }
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        public void RecordSerializer_SerializeEnumEvolvedWithExtraSymbols()
        {
            const string WriterSchema = "{" +
                                        "\"type\":\"enum\"," +
                                        "\"name\":\"Microsoft.Hadoop.Avro.Specifications.TestEnum\"," +
                                        "\"symbols\":" +
                                        "[" +
                                        "\"EnumValue3\"," +
                                        "\"EnumValue2\"," +
                                        "\"EnumValue1\"" +
                                        "]}";

            const string ReaderSchema = "{" +
                                        "\"type\":\"enum\"," +
                                        "\"name\":\"Microsoft.Hadoop.Avro.Specifications.TestEnum\"," +
                                        "\"symbols\":" +
                                        "[" +
                                        "\"EnumValue3\"," +
                                        "\"EnumValue2\"," +
                                        "\"EnumValue1\"," +
                                        "\"ExtraEnumValue0\"" +
                                        "]}";

            var serializer = AvroSerializer.CreateGeneric<AvroEnum>(WriterSchema);
            var deserializer = AvroSerializer.CreateGenericDeserializerOnly<AvroEnum>(WriterSchema, ReaderSchema);

            using (var stream = new MemoryStream())
            {
                dynamic expected = new AvroEnum(serializer.WriterSchema);
                expected.IntegerValue = 0;

                serializer.Serialize(stream, expected);
                stream.Seek(0, SeekOrigin.Begin);

                dynamic actual = deserializer.Deserialize(stream);
                Assert.AreEqual(expected.IntegerValue, actual.IntegerValue);
                Assert.AreEqual(expected.Value, actual.Value);
            }
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [ExpectedException(typeof(RuntimeBinderException))]
        public void RecordSerializer_SerializeRecordWithUknownField()
        {
            const string StringSchema = @"{
                 ""name"":""Category"",
                 ""namespace"":""ApacheAvro.Types"",
                 ""type"":""record"",
                 ""fields"":
                           [
                                {""name"":""CategoryName"", ""type"":""string""},
                                {""name"":""Description"", ""type"":[""string"",""null""]},
                                {""name"":""Picture"", ""type"":[""bytes"", ""null""]},
                                {""name"":""Id"", ""type"":[""int"", ""null""]}
                           ]
             }";

            var serializer = AvroSerializer.CreateGeneric<AvroRecord>(StringSchema);
            dynamic expected = new AvroRecord(serializer.WriterSchema);
            expected.UknownField = 5;
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        public void RecordSerializer_SerializeUnionWithArrayAndNull()
        {
            const string StringSchema = @"{
                ""type"":""record"",
                ""name"":""Microsoft.Hadoop.Avro.Specifications.SimpleIntClass"",
                ""fields"":
                    [
                        { ""name"":""Arr"", ""type"": [""null"", {""type"":""array"", ""items"":""int""}]}
                    ]
            }";
            var serializer = AvroSerializer.CreateGeneric<AvroRecord>(StringSchema);

            using (var stream = new MemoryStream())
            {
                dynamic expected = new AvroRecord(serializer.WriterSchema);
                expected.Arr = new int[] { 1, 2, 5 };

                serializer.Serialize(stream, expected);
                stream.Position = 0;

                dynamic actual = serializer.Deserialize(stream);
                CollectionAssert.AreEqual(expected.Arr, actual.Arr);
            }
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        public void RecordSerializer_SerializeUnionWithMapAndNull()
        {
            const string StringSchema = @"{
                             ""type"":""record"",
                             ""name"":""Microsoft.Hadoop.Avro.Specifications.SimpleIntClass"",
                             ""fields"":
                                       [
                                           {""name"":""LongMap"", ""type"": [""null"", {""type"":""map"", ""values"":""long""}]},
                                       ]
                          }";
            var serializer = AvroSerializer.CreateGeneric<AvroRecord>(StringSchema);

            using (var stream = new MemoryStream())
            {
                dynamic expected = new AvroRecord(serializer.WriterSchema);
                expected.LongMap = new Dictionary<string, long> { { "test", 1 } };

                serializer.Serialize(stream, expected);
                stream.Position = 0;

                dynamic actual = serializer.Deserialize(stream);
                Utilities.DictionaryEquals<string, long>(expected.LongMap as IDictionary<string, long>, actual.LongMap as IDictionary<string, long>);
            }
        }

        internal static bool ShallowlyEqual(AvroRecord r1, AvroRecord r2)
        {
            if (r1 == r2)
            {
                return true;
            }

            if (r1 == null || r2 == null)
            {
                return false;
            }

            var keys1 = r1.Schema.Fields.Select(f => f.Name).ToList();
            var keys2 = r2.Schema.Fields.Select(f => f.Name).ToList();

            if (!keys1.SequenceEqual(keys2))
            {
                return false;
            }

            foreach (var key in keys1)
            {
                if (r1[key] == r2[key])
                {
                    continue;
                }

                if (r1[key] is AvroRecord)
                {
                    if (!ShallowlyEqual(r1[key] as AvroRecord, r2[key] as AvroRecord))
                    {
                        return false;
                    }
                }
                else if (!r1[key].Equals(r2[key]))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
