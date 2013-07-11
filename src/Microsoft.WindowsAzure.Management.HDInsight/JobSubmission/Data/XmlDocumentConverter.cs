namespace Microsoft.WindowsAzure.Management.HDInsight.JobSubmission.Data
{
    using System.IO;
    using System.Xml;

    internal class XmlDocumentConverter
    {
        internal XmlDocument GetXmlDocument(string payLoad)
        {
            XmlDocument doc = new XmlDocument();
            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream))
            {
                writer.Write(payLoad);
                writer.Flush();
                stream.Position = 0;
                using (var xmlReader = XmlReader.Create(stream))
                {
                    doc.Load(xmlReader);
                    return doc;
                }
            }
        }
    }
}