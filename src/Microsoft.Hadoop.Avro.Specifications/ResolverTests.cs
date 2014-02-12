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
    using System.Linq;
    using System.Runtime.Serialization;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public sealed class ResolverTests
    {
        [TestMethod]
        [TestCategory("CheckIn")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Resolver_GetSerializationInfoUsingDataContactResolverWithNullFieldInfo()
        {
            var resolver = new AvroDataContractResolver();
            resolver.ResolveMembers(null);
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [ExpectedException(typeof(SerializationException))]
        public void Resolver_GetSerializationInfoDataContractResolverWithUnsupportedTypes()
        {
            var resolver = new AvroDataContractResolver();
            resolver.ResolveType(typeof(ClassWithoutParameterlessConstructor));
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Resolver_GetSerializationInfoUsingPublicMembersResolverWithNullFieldInfo()
        {
            var resolver = new AvroPublicMemberContractResolver();
            resolver.ResolveMembers(null);
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [ExpectedException(typeof(SerializationException))]
        public void Resolver_GetSerializationInfoPublicMembersResolverWithUnsupportedTypes()
        {
            var resolver = new AvroPublicMemberContractResolver();
            resolver.ResolveType(typeof(ClassWithoutParameterlessConstructor));
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        public void Resolver_GetKnownTypesUsingDataContactResolverForAbstractClassAndInvalidTypes()
        {
            var resolver = new AvroDataContractResolver();
            var knownTypes = resolver.GetKnownTypes(typeof(AbstractClassWithInvalidKnownTypes));
            Assert.IsTrue(knownTypes.SequenceEqual(new[] { typeof(Rectangle) }));
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Resolver_GetKnownTypesUsingDataContractResolverWithNull()
        {
            var resolver = new AvroDataContractResolver();
            resolver.GetKnownTypes(null);
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        public void Resolver_GetKnownTypesUsingDataContactResolverForAbstractClassAndValidTypes()
        {
            var resolver = new AvroDataContractResolver();
            var knownTypes = resolver.GetKnownTypes(typeof(AbstractShape)).ToList();

            Assert.AreEqual(2, knownTypes.Count);
            Assert.IsTrue(
                (knownTypes[0] == typeof(Square) && knownTypes[1] == typeof(Rectangle)) ||
                (knownTypes[1] == typeof(Square) && knownTypes[0] == typeof(Rectangle)));
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Resolver_GetSerializationInfoUsingDataContactResolverWithNullType()
        {
            var resolver = new AvroDataContractResolver();
            resolver.ResolveType(null);
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Resolver_GetSerializationInfoUsingPublicMembersResolverWithNullType()
        {
            var resolver = new AvroPublicMemberContractResolver();
            resolver.ResolveType(null);
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        public void Resolver_DataContractResolverEqualityWithNull()
        {
            var resolver = new AvroDataContractResolver();
            var result = resolver.Equals(null);
            Assert.AreEqual(false, result);
        }

        [TestMethod]
        [TestCategory("CheckIn")]
        public void Resolver_DataContractResolverEqualityWithAnotherDataContractResolver()
        {
            var resolver = new AvroDataContractResolver();
            var anotherResolver = new AvroDataContractResolver();
            var result = resolver.Equals(anotherResolver);
            Assert.AreEqual(true, result);
        }

    }
}
