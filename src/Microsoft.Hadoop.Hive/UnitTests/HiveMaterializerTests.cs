using System;
using System.Data;
using System.Linq;
using Microsoft.Hadoop.Hive;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class HiveMaterializerTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void MaterializeNullTable()
        {
            HiveMaterializer.Materialize<string>(null);
        }

        [TestMethod]
        public void CreateObjectWithNullRow()
        {
            // var expectedResult = new SimpleObject();
            var obj = HiveMaterializer.CreateObject<SimpleObject>(null);

            Assert.IsNull(obj);
        }

        [TestMethod]
        [ExpectedException(typeof(MissingMethodException))]
        public void MaterializeObjectWithPrivateConstructor()
        {
            var table = new DataTable();
            table.Columns.Add("PropertyOne");
            var row = table.NewRow();
            row[0] = "TestValue";
            table.Rows.Add(row);

            var objects = HiveMaterializer.Materialize<PrivateConstructorObject>(table);

            Assert.IsNotNull(objects);
            var objList = objects.ToList();
            Assert.IsTrue(objList.Count == 1);
        }

        [TestMethod]
        [ExpectedException(typeof(MissingMethodException))]
        public void MaterializeObjectWithNoDefaultConstructor()
        {
            var table = new DataTable();
            table.Columns.Add("PropertyOne");
            var row = table.NewRow();
            row[0] = "TestValue";
            table.Rows.Add(row);

            HiveMaterializer.Materialize<NoDefaultConstructorObject>(table);
        }

        [TestMethod]
        public void MaterializeSimpleObject()
        {
            var table = new DataTable();
            table.Columns.Add("PropertyOne");
            var row = table.NewRow();
            row[0] = "TestValue";
            table.Rows.Add(row);

            var objects = HiveMaterializer.Materialize<SimpleObject>(table);

            Assert.IsNotNull(objects);
            var objList = objects.ToList();
            Assert.IsTrue(objList.Count == 1);
            Assert.IsTrue(objList[0].PropertyOne == "TestValue");
        }

        [TestMethod]
        public void MaterializeMultipleSimpleObjects()
        {
            var table = new DataTable();
            table.Columns.Add("PropertyOne");
            
            var row = table.NewRow();
            row[0] = "TestValue";
            table.Rows.Add(row);

            var row2 = table.NewRow();
            row2[0] = "OtherTestValue";
            table.Rows.Add(row2);

            var objects = HiveMaterializer.Materialize<SimpleObject>(table);

            Assert.IsNotNull(objects);
            var objList = objects.ToList();
            Assert.IsTrue(objList.Count == 2);
            Assert.IsTrue(objList[0].PropertyOne == "TestValue");
            Assert.IsTrue(objList[1].PropertyOne == "OtherTestValue");
        }

        [TestMethod]
        public void MaterializeComplexObjects()
        {
            var table = new DataTable();
            table.Columns.Add("PropertyOne");
            table.Columns.Add("PropertyTwo");

            var row = table.NewRow();
            row[0] = "TestValue";
            row[1] = "OtherTestValue";
            table.Rows.Add(row);

            var row2 = table.NewRow();
            row2[0] = "OtherTestValue";
            row2[1] = "TestValue";
            table.Rows.Add(row2);

            var objects = HiveMaterializer.Materialize<ComplexObject>(table);

            Assert.IsNotNull(objects);
            var objList = objects.ToList();
            Assert.IsTrue(objList.Count == 2);
            Assert.IsTrue(objList[0].PropertyOne == "TestValue");
            Assert.IsTrue(objList[0].PropertyTwo == "OtherTestValue");
            Assert.IsTrue(objList[1].PropertyTwo == "TestValue");
            Assert.IsTrue(objList[1].PropertyOne == "OtherTestValue");
        }

        [TestMethod]
        [ExpectedException(typeof(MissingMemberException))]
        public void MaterializeSimpleObjectWithMismatchedColumnNames()
        {
            var table = new DataTable();
            table.Columns.Add("PropertyTwo");
            var row = table.NewRow();
            row[0] = "TestValue";
            table.Rows.Add(row);

            var objects = HiveMaterializer.Materialize<SimpleObject>(table);

            Assert.IsNotNull(objects);
            var objList = objects.ToList();
            Assert.IsTrue(objList.Count == 1);
            Assert.IsTrue(objList[0].PropertyOne == "TestValue");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void MaterializeSimpleObjectWithMismatchedDataTypes()
        {
            var table = new DataTable();
            table.Columns.Add("PropertyOne");
            var row = table.NewRow();
            row[0] = "TestValue";
            table.Rows.Add(row);

            var objects = HiveMaterializer.Materialize<SimpleObjectWithNoStringProperties>(table);

            Assert.IsNotNull(objects);
            var objList = objects.ToList();
            Assert.IsTrue(objList.Count == 1);
        }

        [TestMethod]
        [ExpectedException(typeof(MissingMemberException))]
        public void MaterializeSimpleObjectWithMismatchedNumberOfColumns()
        {
            var table = new DataTable();
            table.Columns.Add("PropertyOne");
            table.Columns.Add("PropertyTwo");
            var row = table.NewRow();
            row[0] = "TestValue";
            row[1] = "OtherValue";
            table.Rows.Add(row);

            var objects = HiveMaterializer.Materialize<SimpleObject>(table);

            Assert.IsNotNull(objects);
            var objList = objects.ToList();
            Assert.IsTrue(objList.Count == 1);
            Assert.IsTrue(objList[0].PropertyOne == "TestValue");
        }



        [TestMethod]
        [ExpectedException(typeof(MissingMemberException))]
        public void MaterializeObjectWithNoProperties()
        {
            var table = new DataTable();
            table.Columns.Add("FieldOne");
            var row = table.NewRow();
            row[0] = "TestValue";
            table.Rows.Add(row);

            HiveMaterializer.Materialize<NoPropertyObject>(table);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void MaterializeObjectWithNoSetter()
        {
            var table = new DataTable();
            table.Columns.Add("PropertyOne");
            var row = table.NewRow();
            row[0] = "TestValue";
            table.Rows.Add(row);

            var objects = HiveMaterializer.Materialize<NoSetterObject>(table);

            Assert.IsNotNull(objects);
            var objList = objects.ToList();
            Assert.IsTrue(objList.Count == 1);
            Assert.IsTrue(objList[0].PropertyOne == "TestValue");
        }

        [TestMethod]
        public void MaterializeObjectWithPrivateSetter()
        {
            var table = new DataTable();
            table.Columns.Add("PropertyOne");
            var row = table.NewRow();
            row[0] = "TestValue";
            table.Rows.Add(row);

            var objects = HiveMaterializer.Materialize<PrivateSetterObject>(table);

            Assert.IsNotNull(objects);
            var objList = objects.ToList();
            Assert.IsTrue(objList.Count == 1);
        }


        internal class SimpleObject
        {
            public string PropertyOne { get; set; }
        }

        internal class SimpleObjectWithNoStringProperties
        {
            public int PropertyOne { get; set; }
        }

        internal class ComplexObject : SimpleObject
        {
            public new string PropertyOne { get; set; }
            public string PropertyTwo { get; set; }

            public string FieldOne = null;

            public void MethodOne()
            {
                return;
            }

            public void MethodOne(string arg)
            {
            }
        }

        internal class NoSetterObject
        {
            private readonly string propertyOne = string.Empty;
            public string PropertyOne { get { return propertyOne; } }
        }

        internal class PrivateSetterObject
        {
            public string PropertyOne { get; private set; }
        }

        internal class NoPropertyObject
        {
            public string FieldOne = null;
        }

        internal class PrivateConstructorObject
        {
            private PrivateConstructorObject()
            {
                return;
            }

            public string PropertyOne { get; set; }
        }

        internal class NoDefaultConstructorObject
        {
            private NoDefaultConstructorObject(string test)
            {
                return;
            }

            public string PropertyOne { get; set; }
        }
    }
}
