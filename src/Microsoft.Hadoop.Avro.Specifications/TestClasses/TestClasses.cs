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
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Runtime.Serialization;
    using ProtoBuf;

    [DataContract]
    public class ClassOfInt : IEquatable<ClassOfInt>
    {
        [DataMember]
        public int PrimitiveInt;

        public static ClassOfInt Create(bool nullablesAreNulls)
        {
            return new ClassOfInt { PrimitiveInt = Utilities.GetRandom<int>(nullablesAreNulls), };
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as ClassOfInt);
        }

        public bool Equals(ClassOfInt other)
        {
            if (other == null)
            {
                return false;
            }
            return this.PrimitiveInt == other.PrimitiveInt;
        }

        public override int GetHashCode()
        {
            return this.PrimitiveInt.GetHashCode();
        }

    }

    [DataContract]
    public class NestedClass : IEquatable<NestedClass>
    {
        [DataMember]
        public ClassOfInt ClassOfIntReference;

        [DataMember]
        public int PrimitiveInt;

        public static NestedClass Create(bool nullablesAreNulls)
        {
            return new NestedClass
            {
                ClassOfIntReference = Utilities.GetRandom<ClassOfInt>(nullablesAreNulls),
                PrimitiveInt = Utilities.GetRandom<int>(nullablesAreNulls),
            };
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as NestedClass);
        }

        public bool Equals(NestedClass other)
        {
            if (other == null)
            {
                return false;
            }
            return ((this.ClassOfIntReference == null && other.ClassOfIntReference == null) ||
                    (this.ClassOfIntReference != null && this.ClassOfIntReference.Equals(other.ClassOfIntReference))) &&
                   this.PrimitiveInt == other.PrimitiveInt;
        }

        public override int GetHashCode()
        {
            return this.ClassOfIntReference.GetHashCode() ^ this.PrimitiveInt.GetHashCode();
        }

    }

    [DataContract]
    public class ClassOfGuid : IEquatable<ClassOfGuid>
    {
        [DataMember]
        public Guid PrimitiveGuid;

        public static ClassOfGuid Create(bool nullablesAreNulls)
        {
            return new ClassOfGuid { PrimitiveGuid = Utilities.GetRandom<Guid>(nullablesAreNulls), };
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as ClassOfGuid);
        }

        public bool Equals(ClassOfGuid other)
        {
            if (other == null)
            {
                return false;
            }
            return this.PrimitiveGuid == other.PrimitiveGuid;
        }

        public override int GetHashCode()
        {
            return this.PrimitiveGuid.GetHashCode();
        }

    }

    [ProtoContract]
    internal class SimpleFlatClass : IEquatable<SimpleFlatClass>
    {
        public static Random R = new Random(typeof(SimpleFlatClass).GetHashCode());

        public static SimpleFlatClass Create()
        {
            var bytes = new byte[13];
            R.NextBytes(bytes);
            return new SimpleFlatClass
            {
                MaxCharField = char.MaxValue,
                MinCharField = char.MinValue,
                MaxByteField = byte.MaxValue,
                MinByteField = byte.MinValue,
                MaxSByteField = sbyte.MaxValue,
                MinSByteField = sbyte.MinValue,
                MaxShortField = short.MaxValue,
                MinShortField = short.MinValue,
                MaxUShortField = ushort.MaxValue,
                MinUShortField = ushort.MinValue,
                MaxIntField = int.MaxValue,
                MinIntField = int.MinValue,
                MaxUIntField = uint.MaxValue,
                MinUIntField = uint.MinValue,
                MaxLongField = long.MaxValue,
                MinLongField = long.MinValue,
                MaxULongField = ulong.MaxValue,
                MinULongField = ulong.MinValue,
                MaxFloatField = float.MaxValue,
                MinFloatField = float.MinValue,
                MaxDoubleField = double.MaxValue,
                MinDoubleField = double.MinValue,
                StringField = Utilities.GetRandom<string>(false),
                NullStringField = null,
                EmptyStringField = string.Empty,
                ByteArrayField = bytes,
                NullByteArrayField = null,
                ZeroByteArrayField = new byte[0],
                BoolFalseField = false,
                BoolTrueField = true,
                MaxDecimalField = decimal.MaxValue,
                MinDecimalField = decimal.MinValue
            };
        }

        [ProtoMember(30)]
        public bool BoolFalseField { get; set; }

        [ProtoMember(29)]
        public bool BoolTrueField { get; set; }

        [ProtoMember(26)]
        public byte[] ByteArrayField { get; set; }

        [ProtoMember(24)]
        public string EmptyStringField { get; set; }

        [ProtoMember(3)]
        public byte MaxByteField { get; set; }

        [ProtoMember(1)]
        public char MaxCharField { get; set; }

        [ProtoMember(31)]
        public decimal MaxDecimalField { get; set; }

        [ProtoMember(21)]
        public double MaxDoubleField { get; set; }

        [ProtoMember(19)]
        public float MaxFloatField { get; set; }

        [ProtoMember(11)]
        public int MaxIntField { get; set; }

        [ProtoMember(15)]
        public long MaxLongField { get; set; }

        [ProtoMember(5)]
        public sbyte MaxSByteField { get; set; }

        [ProtoMember(7)]
        public short MaxShortField { get; set; }

        [ProtoMember(13)]
        public uint MaxUIntField { get; set; }

        [ProtoMember(17)]
        public ulong MaxULongField { get; set; }

        [ProtoMember(9)]
        public ushort MaxUShortField { get; set; }

        [ProtoMember(4)]
        public byte MinByteField { get; set; }

        [ProtoMember(2)]
        public char MinCharField { get; set; }

        [ProtoMember(32)]
        public decimal MinDecimalField { get; set; }

        [ProtoMember(22)]
        public double MinDoubleField { get; set; }

        [ProtoMember(20)]
        public float MinFloatField { get; set; }

        [ProtoMember(12)]
        public int MinIntField { get; set; }

        [ProtoMember(16)]
        public long MinLongField { get; set; }

        [ProtoMember(6)]
        public sbyte MinSByteField { get; set; }

        [ProtoMember(8)]
        public short MinShortField { get; set; }

        [ProtoMember(14)]
        public uint MinUIntField { get; set; }

        [ProtoMember(18)]
        public ulong MinULongField { get; set; }

        [ProtoMember(10)]
        public ushort MinUShortField { get; set; }

        [ProtoMember(27)]
        public byte[] NullByteArrayField { get; set; }

        [ProtoMember(25)]
        public string NullStringField { get; set; }

        [ProtoMember(23)]
        public string StringField { get; set; }

        [ProtoMember(28)]
        public byte[] ZeroByteArrayField { get; set; }

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity",
            Justification = "This could probably be simplified but is okay for now. [tgs]")]
        public bool Equals(SimpleFlatClass other)
        {
            if (other == null)
            {
                return false;
            }

            if (this.MaxCharField != other.MaxCharField)
            {
                return false;
            }

            if (this.MinCharField != other.MinCharField)
            {
                return false;
            }

            if (this.MaxByteField != other.MaxByteField)
            {
                return false;
            }

            if (this.MinByteField != other.MinByteField)
            {
                return false;
            }

            if (this.MaxSByteField != other.MaxSByteField)
            {
                return false;
            }

            if (this.MinSByteField != other.MinSByteField)
            {
                return false;
            }

            if (this.MaxShortField != other.MaxShortField)
            {
                return false;
            }

            if (this.MinShortField != other.MinShortField)
            {
                return false;
            }

            if (this.MaxUShortField != other.MaxUShortField)
            {
                return false;
            }

            if (this.MinUShortField != other.MinUShortField)
            {
                return false;
            }

            if (this.MaxIntField != other.MaxIntField)
            {
                return false;
            }

            if (this.MinIntField != other.MinIntField)
            {
                return false;
            }

            if (this.MaxUIntField != other.MaxUIntField)
            {
                return false;
            }

            if (this.MinUIntField != other.MinUIntField)
            {
                return false;
            }

            if (this.MaxLongField != other.MaxLongField)
            {
                return false;
            }

            if (this.MinLongField != other.MinLongField)
            {
                return false;
            }

            if (this.MaxULongField != other.MaxULongField)
            {
                return false;
            }

            if (this.MinULongField != other.MinULongField)
            {
                return false;
            }

            if (this.MaxFloatField != other.MaxFloatField)
            {
                return false;
            }

            if (this.MinFloatField != other.MinFloatField)
            {
                return false;
            }

            if (this.MaxDoubleField != other.MaxDoubleField)
            {
                return false;
            }

            if (this.MinDoubleField != other.MinDoubleField)
            {
                return false;
            }

            if (this.StringField != other.StringField)
            {
                return false;
            }

            if (this.NullStringField != other.NullStringField)
            {
                return false;
            }

            if (this.EmptyStringField != other.EmptyStringField)
            {
                return false;
            }

            if (this.BoolFalseField != other.BoolFalseField)
            {
                return false;
            }

            if (this.BoolTrueField != other.BoolTrueField)
            {
                return false;
            }

            if (this.MaxDecimalField != other.MaxDecimalField)
            {
                return false;
            }

            if (this.MinDecimalField != other.MinDecimalField)
            {
                return false;
            }

            if (!this.ByteArrayField.OrderBy(a => a).SequenceEqual(other.ByteArrayField.OrderBy(a => a)))
            {
                return false;
            }

            if (!this.ZeroByteArrayField.OrderBy(a => a).SequenceEqual(other.ZeroByteArrayField.OrderBy(a => a)))
            {
                return false;
            }

            if (this.NullByteArrayField != null || other.NullByteArrayField != null)
            {
                return false;
            }

            return true;
        }

        public override bool Equals(object other)
        {
            return this.Equals(other as SimpleFlatClass);
        }

        public override int GetHashCode()
        {
            return this.MaxCharField.GetHashCode() ^ this.MinCharField.GetHashCode() ^ this.MaxByteField.GetHashCode() ^
                   this.MinByteField.GetHashCode() ^ this.MaxSByteField.GetHashCode() ^ this.MinSByteField.GetHashCode() ^
                   this.MaxShortField.GetHashCode() ^ this.MinShortField.GetHashCode() ^ this.MaxUShortField.GetHashCode() ^
                   this.MinUShortField.GetHashCode() ^ this.MaxIntField.GetHashCode() ^ this.MinIntField.GetHashCode() ^
                   this.MaxUIntField.GetHashCode() ^ this.MinUIntField.GetHashCode() ^ this.MaxLongField.GetHashCode() ^
                   this.MinLongField.GetHashCode() ^ this.MaxULongField.GetHashCode() ^ this.MinULongField.GetHashCode() ^
                   this.MaxFloatField.GetHashCode() ^ this.MinFloatField.GetHashCode() ^ this.MaxDoubleField.GetHashCode() ^
                   this.MinDoubleField.GetHashCode() ^ (this.StringField == null ? string.Empty.GetHashCode() : this.StringField.GetHashCode()) ^
                   (this.EmptyStringField == null ? string.Empty.GetHashCode() : this.EmptyStringField.GetHashCode()) ^
                   (this.NullStringField == null ? string.Empty.GetHashCode() : this.NullStringField.GetHashCode()) ^
                   (this.ByteArrayField == null ? string.Empty.GetHashCode() : this.ByteArrayField.GetHashCode()) ^
                   (this.NullByteArrayField == null ? string.Empty.GetHashCode() : this.NullByteArrayField.GetHashCode()) ^
                   (this.ZeroByteArrayField == null ? string.Empty.GetHashCode() : this.ZeroByteArrayField.GetHashCode()) ^
                   this.BoolTrueField.GetHashCode() ^ this.BoolFalseField.GetHashCode() ^ this.MaxDecimalField.GetHashCode() ^
                   this.MinDecimalField.GetHashCode();
        }
    }

    [ProtoContract]
    [DataContract]
    internal class Recursive : IEquatable<Recursive>
    {
        public static Recursive Create()
        {
            return new Recursive
            {
                IntField = Utilities.GetRandom<int>(false),
                RecursiveField = new Recursive { IntField = Utilities.GetRandom<int>(false), RecursiveField = new Recursive { IntField = Utilities.GetRandom<int>(false) } }
            };
        }

        [ProtoMember(1)]
        [DataMember]
        public int IntField { get; set; }

        [ProtoMember(2)]
        [DataMember]
        public Recursive RecursiveField { get; set; }

        public bool Equals(Recursive other)
        {
            if (other == null)
            {
                return false;
            }

            if (this.IntField != other.IntField)
            {
                return false;
            }

            if (this.RecursiveField == other.RecursiveField)
            {
                return true;
            }

            return this.RecursiveField.Equals(other.RecursiveField);
        }

        public override bool Equals(object other)
        {
            return this.Equals(other as Recursive);
        }

        public override int GetHashCode()
        {
            return this.IntField.GetHashCode() ^ (this.RecursiveField == null ? string.Empty.GetHashCode() : this.RecursiveField.GetHashCode());
        }
    }

    [ProtoContract]
    internal class ComplexNestedClass : IEquatable<ComplexNestedClass>
    {
        [ProtoMember(2)]
        public int IntNestedField { get; set; }

        [ProtoMember(1)]
        public SimpleFlatClass NestedField { get; set; }

        [ProtoMember(3)]
        public Recursive RecursiveField { get; set; }

        public static Random R = new Random(typeof(ComplexNestedClass).GetHashCode());

        public static ComplexNestedClass Create()
        {
            return new ComplexNestedClass
            {
                NestedField = SimpleFlatClass.Create(),
                IntNestedField = R.Next(),
                RecursiveField = Recursive.Create()
            };
        }

        public bool Equals(ComplexNestedClass other)
        {
            if (other == null)
            {
                return false;
            }

            if (this.IntNestedField != other.IntNestedField)
            {
                return false;
            }

            if (!this.NestedField.Equals(other.NestedField))
            {
                return false;
            }

            return this.RecursiveField.Equals(other.RecursiveField);
        }

        public override bool Equals(object other)
        {
            return this.Equals(other as ComplexNestedClass);
        }

        public override int GetHashCode()
        {
            return this.IntNestedField.GetHashCode() ^ (this.NestedField == null ? string.Empty.GetHashCode() : this.NestedField.GetHashCode()) ^
                   (this.RecursiveField == null ? string.Empty.GetHashCode() : this.RecursiveField.GetHashCode());
        }
    }

    [Flags]
    internal enum TestFlags
    {
        Value1 = 0x1,

        Value2 = 0x2,

        Value3 = 0x4
    }

    internal class FlagsEnumClass : IEquatable<FlagsEnumClass>
    {
        public TestFlags TestFlagsField { get; set; }

        public static FlagsEnumClass Create()
        {
            return new FlagsEnumClass
            {
                TestFlagsField = TestFlags.Value1 | TestFlags.Value2 | TestFlags.Value3
            };
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as FlagsEnumClass);
        }

        public bool Equals(FlagsEnumClass other)
        {
            if (other == null)
            {
                return false;
            }

            return this.TestFlagsField == other.TestFlagsField;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    [ProtoContract]
    internal class TwoFieldsOfTheSameTypeClass : IEquatable<TwoFieldsOfTheSameTypeClass>
    {
        public static TwoFieldsOfTheSameTypeClass Create()
        {
            return new TwoFieldsOfTheSameTypeClass { Field1 = ClassOfInt.Create(true), Field2 = ClassOfInt.Create(true) };
        }

        [ProtoMember(1)]
        public ClassOfInt Field1 { get; set; }

        [ProtoMember(2)]
        public ClassOfInt Field2 { get; set; }

        public bool Equals(TwoFieldsOfTheSameTypeClass other)
        {
            if (other == null)
            {
                return false;
            }

            if (!this.Field1.Equals(other.Field1))
            {
                return false;
            }

            if (!this.Field2.Equals(other.Field2))
            {
                return false;
            }

            return true;
        }

        public override bool Equals(object other)
        {
            return this.Equals(other as TwoFieldsOfTheSameTypeClass);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

    }

    [ProtoContract]
    internal class ClassWithFields : IEquatable<ClassWithFields>
    {
        [ProtoMember(1)]
        [NullableSchema]
        public ClassOfInt Field1;

        [ProtoMember(2)]
        public ClassOfInt Field2;

        public static ClassWithFields Create()
        {
            return new ClassWithFields { Field1 = ClassOfInt.Create(true), Field2 = ClassOfInt.Create(true) };
        }

        public bool Equals(ClassWithFields other)
        {
            if (other == null)
            {
                return false;
            }

            if (!this.Field1.Equals(other.Field1))
            {
                return false;
            }

            if (!this.Field2.Equals(other.Field2))
            {
                return false;
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as ClassWithFields);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

    }

    [DataContract]
    public class DataContractClassWithFields : IEquatable<DataContractClassWithFields>
    {
        [DataMember]
        public ClassOfInt Field1;

        [DataMember]
        private ClassOfInt Field2;

        public static DataContractClassWithFields Create()
        {
            return new DataContractClassWithFields { Field1 = ClassOfInt.Create(true), Field2 = ClassOfInt.Create(true) };
        }

        public int NotSerializedValue { get; set; }

        public bool Equals(DataContractClassWithFields other)
        {
            if (other == null)
            {
                return false;
            }

            if (!this.Field1.Equals(other.Field1))
            {
                return false;
            }

            if (!this.Field2.Equals(other.Field2))
            {
                return false;
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as DataContractClassWithFields);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    [DataContract]
    internal class InheritedSimpleInt : ClassOfInt, IEquatable<InheritedSimpleInt>
    {
        public static InheritedSimpleInt Create()
        {
            return new InheritedSimpleInt
            {
                PrimitiveInt = Utilities.GetRandom<int>(true),
                PublicIntProperty = Utilities.GetRandom<int>(true),
                DataMemberIntProperty = Utilities.GetRandom<int>(true)
            };
        }

        public int PublicIntProperty { get; set; }

        [DataMember]
        internal int DataMemberIntProperty { get; set; }

        public bool Equals(InheritedSimpleInt other)
        {
            return this.Equals(other as ClassOfInt);
        }

        public override bool Equals(object other)
        {
            return this.Equals(other as InheritedSimpleInt);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    internal class ContainingUrlClass : IEquatable<ContainingUrlClass>
    {
        public Uri Field;

        public Uri NullField;

        public static ContainingUrlClass Create()
        {
            return new ContainingUrlClass { Property = Utilities.GetRandom<Uri>(false), Field = Utilities.GetRandom<Uri>(false), NullField = null };
        }

        public Uri Property { get; set; }

        public bool Equals(ContainingUrlClass other)
        {
            if (other == null)
            {
                return false;
            }

            return this.Property == other.Property && this.Field == other.Field && this.NullField == other.NullField;
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as ContainingUrlClass);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Type is used by serializer.")]
    internal class UnicodeClassNameŠŽŒ
    {
    }

    internal class DateTimeOffsetContainingClass : IEquatable<DateTimeOffsetContainingClass>
    {
        public DateTimeOffset UtcTime;

        public static DateTimeOffsetContainingClass Create()
        {
            return new DateTimeOffsetContainingClass { LocalTime = DateTimeOffset.Now, UtcTime = DateTimeOffset.UtcNow };
        }

        public DateTimeOffset LocalTime { get; set; }

        public bool Equals(DateTimeOffsetContainingClass other)
        {
            if (other == null)
            {
                return false;
            }

            return this.LocalTime == other.LocalTime && this.UtcTime == other.UtcTime;
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as DateTimeOffsetContainingClass);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

    }

    [DataContract]
    internal struct NullableStruct : IEquatable<NullableStruct>
    {
        [DataMember]
        public int? IntNullProperty;

        [DataMember]
        public int? IntProperty;

        public static NullableStruct Create()
        {
            return new NullableStruct { IntProperty = 13, IntNullProperty = null };
        }

        public bool Equals(NullableStruct other)
        {
            return this.IntProperty == other.IntProperty && this.IntNullProperty == other.IntNullProperty;
        }
    }

    [DataContract]
    internal class NullableFieldsClass : IEquatable<NullableFieldsClass>
    {
        public static Random R = new Random(typeof(NullableFieldsClass).GetHashCode());

        [DataMember]
        private DateTime? dataTimeNotNullField;

        [DataMember]
        private DateTime? dataTimeNullField;

        [DataMember]
        private Guid? guidNotNullField;

        [DataMember]
        private Guid? guidNullField;

        [DataMember]
        private int? intNotNullField;

        [DataMember]
        private int? intNullField;

        [DataMember]
        private NullableStruct? notNullStruct;

        [DataMember]
        private NullableStruct? nullStruct;

        public static NullableFieldsClass Create()
        {
            return new NullableFieldsClass
            {
                dataTimeNotNullField = DateTime.Now,
                dataTimeNullField = null,
                guidNotNullField = Guid.NewGuid(),
                guidNullField = null,
                intNotNullField = R.Next(),
                intNullField = null,
                notNullStruct = NullableStruct.Create(),
                nullStruct = null
            };
        }

        public bool Equals(NullableFieldsClass other)
        {
            if (other == null)
            {
                return false;
            }

            return this.intNotNullField == other.intNotNullField && this.intNullField == other.intNullField &&
                   this.dataTimeNotNullField == other.dataTimeNotNullField && this.dataTimeNullField == other.dataTimeNullField &&
                   this.guidNotNullField == other.guidNotNullField && this.guidNullField == other.guidNullField &&
                   this.notNullStruct.Equals(other.notNullStruct) && this.nullStruct.Equals(other.nullStruct);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as NullableFieldsClass);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

    }

    internal struct EmptyStruct
    {
    }

    [DataContract]
    internal class ClassWithSchemaNullableField : IEquatable<ClassWithSchemaNullableField>
    {
        [DataMember]
        public int NotNullableValueNotNullableSchema;

        [DataMember]
        [NullableSchema]
        public int NotNullableValueNullableSchema;

        [DataMember]
        public int? NullableValueNotNullableSchema;

        [DataMember]
        [NullableSchema]
        public int? NullableValueNullableSchema;

        [DataMember]
        public ClassOfInt ReferenceFieldNotNullableSchema;

        [DataMember]
        [NullableSchema]
        public ClassOfInt ReferenceFieldNullableSchema;

        public static ClassWithSchemaNullableField Create()
        {
            return new ClassWithSchemaNullableField
            {
                NullableValueNullableSchema = Utilities.GetRandom<int>(false),
                NullableValueNotNullableSchema = Utilities.GetRandom<int>(false),
                NotNullableValueNullableSchema = Utilities.GetRandom<int>(false),
                NotNullableValueNotNullableSchema = Utilities.GetRandom<int>(false),
                ReferenceFieldNullableSchema = ClassOfInt.Create(true),
                ReferenceFieldNotNullableSchema = ClassOfInt.Create(true)
            };
        }

        public bool Equals(ClassWithSchemaNullableField other)
        {
            if (other == null)
            {
                return false;
            }

            return this.NullableValueNullableSchema == other.NullableValueNullableSchema &&
                   this.NullableValueNotNullableSchema == other.NullableValueNotNullableSchema &&
                   this.NotNullableValueNullableSchema == other.NotNullableValueNullableSchema &&
                   this.NotNullableValueNotNullableSchema == other.NotNullableValueNotNullableSchema &&
                   this.ReferenceFieldNullableSchema.Equals(other.ReferenceFieldNullableSchema) &&
                   this.ReferenceFieldNotNullableSchema.Equals(other.ReferenceFieldNotNullableSchema);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as ClassWithSchemaNullableField);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    [DataContract]
    internal class ClassOfEnum : IEquatable<ClassOfEnum>
    {
        [DataMember]
        public Utilities.RandomEnumeration PrimitiveEnum;

        public override bool Equals(object obj)
        {
            return this.Equals(obj as ClassOfEnum);
        }

        public bool Equals(ClassOfEnum other)
        {
            if (other == null)
            {
                return false;
            }
            return
            this.PrimitiveEnum == other.PrimitiveEnum;
        }

        public override int GetHashCode()
        {
            return
            this.PrimitiveEnum.GetHashCode();
        }

        public static ClassOfEnum Create(bool nullablesAreNulls)
        {
            return new ClassOfEnum
            {
                PrimitiveEnum = Utilities.GetRandom<Utilities.RandomEnumeration>(nullablesAreNulls),
            };
        }
    }

    [DataContract]
    public class ClassOfListOfGuid : IEquatable<ClassOfListOfGuid>
    {
        [DataMember]
        internal List<Guid> ListOfGuid;

        public override bool Equals(object obj)
        {
            return this.Equals(obj as ClassOfListOfGuid);
        }

        public bool Equals(ClassOfListOfGuid other)
        {
            if (other == null)
            {
                return false;
            }

            return (this.ListOfGuid == null && other.ListOfGuid == null) ||
                (this.ListOfGuid != null && this.ListOfGuid.SequenceEqual(other.ListOfGuid));
        }

        public override int GetHashCode()
        {
            return this.ListOfGuid.GetHashCode();
        }

        public static ClassOfListOfGuid Create(bool nullablesAreNulls)
        {
            return new ClassOfListOfGuid
            {
                ListOfGuid = Utilities.GetRandom<List<Guid>>(nullablesAreNulls),
            };
        }
    }

    internal class ClassWithNullableIntField
    {
        public int? NullableIntField;
    }

    public class NonDataContractClassWithFields : IEquatable<NonDataContractClassWithFields>
    {
        public ClassOfInt Field1;

        public static NonDataContractClassWithFields Create()
        {
            return new NonDataContractClassWithFields { Field1 = ClassOfInt.Create(true) };
        }

        public bool Equals(NonDataContractClassWithFields other)
        {
            if (other == null)
            {
                return false;
            }

            if (!this.Field1.Equals(other.Field1))
            {
                return false;
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as NonDataContractClassWithFields);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
