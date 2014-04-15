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
namespace Microsoft.Hadoop.Avro.Schema
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
    using Newtonsoft.Json;

    /// <summary>
    ///     Base class for schema objects.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1724:TypeNamesShouldNotMatchNamespaces",
        Justification = "It is a different namespace.")]
    public abstract class Schema
    {
        private readonly Dictionary<string, string> attributes;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Schema" /> class.
        /// </summary>
        /// <param name="attributes">The attributes.</param>
        protected Schema(IDictionary<string, string> attributes)
        {
            if (attributes == null)
            {
                throw new ArgumentNullException("attributes");
            }

            this.attributes = new Dictionary<string, string>(attributes);
        }

        /// <summary>
        ///     Gets the attributes.
        /// </summary>
        public IDictionary<string, string> Attributes
        {
            get { return this.attributes; }
        }

        /// <summary>
        ///     Adds the attribute.
        /// </summary>
        /// <param name="attribute">The attribute.</param>
        /// <param name="value">The value.</param>
        public void AddAttribute(string attribute, string value)
        {
            if (attribute == null)
            {
                throw new ArgumentNullException("attribute");
            }
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            this.attributes.Add(attribute, value);
        }

        /// <summary>
        ///     Returns a <see cref="System.String" /> that represents this instance in JSON format.
        /// </summary>
        /// <returns>
        ///     A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            using (var result = new StringWriter(CultureInfo.InvariantCulture))
            {
                var writer = new JsonTextWriter(result);
                this.ToJson(writer, new HashSet<NamedSchema>());
                return result.ToString();
            }
        }

        /// <summary>
        ///     Converts current node to JSON according to the avro specification.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="seenSchemas">The seen schemas.</param>
        internal void ToJson(JsonTextWriter writer, HashSet<NamedSchema> seenSchemas)
        {
            this.ToJsonSafe(writer, seenSchemas);
        }

        /// <summary>
        ///     Converts current node to JSON according to the avro specification.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="seenSchemas">The seen schemas.</param>
        internal abstract void ToJsonSafe(JsonTextWriter writer, HashSet<NamedSchema> seenSchemas);
    }
}
