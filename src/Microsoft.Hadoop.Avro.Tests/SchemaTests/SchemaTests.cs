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
namespace Microsoft.Hadoop.Avro.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using Microsoft.Hadoop.Avro.Schema;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public sealed class SchemaTests
    {
        [TestMethod]
        [TestCategory("CheckIn")]
        public void SchemaTests_SchemaNameEquality()
        {
            var schemaName = new SchemaName("namespace.name");
            var secondSchemaName = new SchemaName("namespace.name");
            var differentSchemaName = new SchemaName("different.name");
            Utilities.VerifyEquality(schemaName, secondSchemaName, differentSchemaName);
        }

        [SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", Justification = "Ctor should throw.")]
        [TestMethod]
        [TestCategory("CheckIn")]
        public void SchemaTests_SchemaNameTryArguments()
        {
            Utilities.ShouldThrow<ArgumentException>(() => new SchemaName(null, "namespace"));
            Utilities.ShouldThrow<ArgumentException>(() => new SchemaName(string.Empty, "namespace"));
            Utilities.ShouldThrow<SerializationException>(() => new SchemaName("name.with.namespace.ending.with.dot.", string.Empty));
            Utilities.ShouldThrow<SerializationException>(() => new SchemaName("invalidnameŠŽŒ", "valid.namespace"));
            Utilities.ShouldThrow<SerializationException>(() => new SchemaName("validName", "invalid.namespaceŠŽŒ"));
        }
    }
}
