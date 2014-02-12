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
namespace Microsoft.Hadoop.Avro.Serializers
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq.Expressions;
    using System.Runtime.Serialization;
    using Microsoft.Hadoop.Avro.Schema;

    /// <summary>
    ///     Only possible fixed value is Guid. It is handled in the <see cref="GuidSerializer"/>. Here only skipper is implemented.
    /// </summary>
    internal sealed class FixedSerializer : ObjectSerializerBase<FixedSchema>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FixedSerializer" /> class.
        /// </summary>
        /// <param name="schema">The schema.</param>
        public FixedSerializer(FixedSchema schema) : base(schema)
        {
        }

        protected override Expression BuildSkipperSafe(Expression decoder)
        {
            return Expression.Call(decoder, this.Skip("Fixed"), new Expression[] { Expression.Constant(this.Schema.Size) });
        }

        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Internal + done in base class.")]
        protected override void SerializeSafe(IEncoder encoder, object @object)
        {
            var @fixed = (byte[])@object;
            if (@fixed.Length != this.Schema.Size)
            {
                throw new SerializationException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Invalid fixed size. Expected: '{0}', actual: '{1}'.",
                        this.Schema.Size,
                        @fixed.Length));
            }
            encoder.EncodeFixed(@fixed);
        }

        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Internal + done in base class.")]
        protected override object DeserializeSafe(IDecoder decoder)
        {
            return decoder.DecodeFixed(this.Schema.Size);
        }

        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Internal + done in base class.")]
        protected override void SkipSafe(IDecoder decoder)
        {
            decoder.SkipFixed(this.Schema.Size);
        }
    }
}
