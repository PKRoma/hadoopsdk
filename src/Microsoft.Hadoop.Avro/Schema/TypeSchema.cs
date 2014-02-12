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
    using System.Diagnostics.Contracts;
    using Microsoft.Hadoop.Avro.Serializers;

    /// <summary>
    ///     Base class for all type schemas.
    ///     For more details please see <a href="http://avro.apache.org/docs/current/spec.html">the specification</a>.
    /// </summary>
    public abstract class TypeSchema : Schema
    {
        private Type runtimeType;

        /// <summary>
        ///     Initializes a new instance of the <see cref="TypeSchema" /> class.
        /// </summary>
        /// <param name="runtimeType">Type of the runtime.</param>
        /// <param name="attributes">The attributes.</param>
        protected TypeSchema(Type runtimeType, IDictionary<string, string> attributes)
            : base(attributes)
        {
            if (runtimeType == null)
            {
                throw new ArgumentNullException("runtimeType");
            }
            Contract.EndContractBlock();

            this.runtimeType = runtimeType;
        }

        /// <summary>
        /// Gets the runtime type.
        /// </summary>
        public Type RuntimeType
        {
            get { return this.runtimeType; }
            //internal set { this.runtimeType = value; }
        }

        /// <summary>
        /// Gets the type of the schema as string.
        /// </summary>
        internal abstract string Type { get; }

        /// <summary>
        ///     Gets or sets the builder.
        /// </summary>
        internal IObjectSerializer Serializer { get; set; }
    }
}
