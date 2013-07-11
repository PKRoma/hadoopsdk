namespace Microsoft.WindowsAzure.Management.HDInsight.TestUtilities
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.Management.Framework;

    public static class Assert2
    {
        private static bool AssertForNull([ValidatedNotNull] object expected, [ValidatedNotNull] object actual)
        {
            if (expected.IsNotNull() && actual.IsNotNull())
            {
                return true;
            }
            if (expected.IsNull() && actual.IsNull())
            {
                return false;
            }
            if (expected.IsNull())
            {
                Assert.Fail("The expected object was null but the actual object wasn't");
            }
            Assert.Fail("The actual object was null but the expected object wasn't");
            // This line is only needed because the complier is unaware that the above line will always throw an exception.
            return false;
        }

        private static XmlAttribute FindInEnum(IEnumerable<XmlAttribute> list, XmlAttribute toFind)
        {
            XmlAttribute retval = null;
            foreach (var xmlAttribute in list)
            {
                if (xmlAttribute.Name == toFind.Name && xmlAttribute.NamespaceURI == toFind.NamespaceURI)
                {
                    retval = xmlAttribute;
                    break;
                }
            }
            return retval;
        }

        [SuppressMessage("Microsoft.Design", "CA1059:MembersShouldNotExposeCertainConcreteTypes", MessageId = "System.Xml.XmlNode",
            Justification = "Suppressing for now until we can replace with an IXPathNavigable implementation. [tgs]")]
        public static void AreSame(XmlAttribute expected, XmlAttribute actual)
        {
            if (AssertForNull(expected, actual))
            {
                Assert.AreEqual(expected.Name, actual.Name);
                Assert.AreEqual(expected.NamespaceURI, actual.NamespaceURI);
                Assert.AreEqual(expected.Value, actual.Value);
            }
        }

        public static void AreEquivalent(IEnumerable<XmlAttribute> expected, IEnumerable<XmlAttribute> actual)
        {
            if (AssertForNull(expected, actual))
            {
                Assert.AreEqual(expected.Count(), actual.Count());
                foreach (var xmlAttribute in expected)
                {
                    var foundActual = FindInEnum(actual, xmlAttribute);
                    AreSame(xmlAttribute, foundActual);
                }
            }
        }

        public static void AreSame(IEnumerable<XmlElement> expected, IEnumerable<XmlElement> actual)
        {
            if (AssertForNull(expected, actual))
            {
                var expectedArray = expected.ToArray();
                var actualArray = actual.ToArray();
                Assert.AreEqual(expectedArray.Length, actualArray.Length);
                for (int i = 0; i < expectedArray.Length; i++)
                {
                    Assert.AreSame(expectedArray[i], actualArray[i]);
                }
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1059:MembersShouldNotExposeCertainConcreteTypes", MessageId = "System.Xml.XmlNode",
            Justification = "Suppressing for now until we can replace with an IXPathNavigable implementation. [tgs]")]
        public static void AreSame(XmlElement expected, XmlElement actual)
        {
            if (AssertForNull(expected, actual))
            {
                Assert.AreEqual(expected.Name, actual.Name);
                Assert.AreEqual(expected.NamespaceURI, actual.NamespaceURI);
                List<XmlElement> expectedChildren = new List<XmlElement>();
                List<XmlElement> actualChildren = new List<XmlElement>();
                List<XmlAttribute> expectedAttributes = new List<XmlAttribute>();
                List<XmlAttribute> actualAttributes = new List<XmlAttribute>();
                foreach (var childNode in expected.ChildNodes)
                {
                    var asElement = childNode as XmlElement;
                    if (asElement.IsNotNull())
                    {
                        expectedChildren.Add(asElement);
                        continue;
                    }
                    var asAttribute = childNode as XmlAttribute;
                    if (asAttribute.IsNotNull())
                    {
                        expectedAttributes.Add(asAttribute);
                    }
                }
                foreach (var childNode in actual.ChildNodes)
                {
                    var asElement = childNode as XmlElement;
                    if (asElement.IsNotNull())
                    {
                        actualChildren.Add(asElement);
                    }
                    var asAttribute = childNode as XmlAttribute;
                    if (asAttribute.IsNotNull())
                    {
                        actualAttributes.Add(asAttribute);
                    }
                }
                AreEquivalent(expectedAttributes, actualAttributes);
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1059:MembersShouldNotExposeCertainConcreteTypes", MessageId = "System.Xml.XmlNode",
            Justification = "Suppressing for now until we can replace with an IXPathNavigable implementation. [tgs]")]
        public static void AreSame(XmlDocument expected, XmlDocument actual)
        {
            if (AssertForNull(expected, actual))
            {
                AreSame(expected.DocumentElement, actual.DocumentElement);
            }
        }
    }
}
