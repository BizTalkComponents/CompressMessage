using System;
using System.Text;
using System.IO;
using System.IO.Compression;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Winterdom.BizTalk.PipelineTesting;

namespace BizTalkComponents.PipelineComponents.CompressMessage.Tests.UnitTests
{
    [TestClass]
    public class CompressMessageTests
    {
        [TestMethod]
        public void TestCompressWithReceivedFilename()
        {
            var pipeline = PipelineFactory.CreateEmptySendPipeline();

            var component = new CompressMessage();
            var msgPart2 = MessageHelper.CreatePartFromString("<testmessage2></testmessage2>");
            var msgPart3 = MessageHelper.CreatePartFromString("<testmessage3></testmessage3>");
            msgPart2.PartProperties.Write("ReceivedFileName", "http://schemas.microsoft.com/BizTalk/2003/file-properties", "invoice2.xml");
            msgPart3.PartProperties.Write("ReceivedFileName", "http://schemas.microsoft.com/BizTalk/2003/file-properties", "invoice3.xml");
            var msg = MessageHelper.CreateFromString("<testmessage1></testmessage1>");
            msg.AddPart("invoice2", msgPart2, false);
            msg.AddPart("invoice3", msgPart3, false);
            msg.BodyPart.PartProperties.Write("ReceivedFileName", "http://schemas.microsoft.com/BizTalk/2003/file-properties", "invoice1.xml");
            pipeline.AddComponent(component, PipelineStage.Encode);

            var result = pipeline.Execute(msg);

            int i = 1;
            ZipArchive zipArchive = new ZipArchive(result.BodyPart.GetOriginalDataStream());
            foreach (var zipEntry in zipArchive.Entries)
            {
                Assert.AreEqual(string.Format("invoice{0}.xml", i), zipEntry.Name);
                using (var reader = new StreamReader(zipEntry.Open(), Encoding.Unicode))
                {
                    Assert.AreEqual(string.Format("<testmessage{0}></testmessage{0}>", i), reader.ReadToEnd());
                }
                i++;
            }
        }

        [TestMethod]
        public void TestCompressWithContentType()
        {
            var pipeline = PipelineFactory.CreateEmptySendPipeline();

            var component = new CompressMessage();
            var msgPart2 = MessageHelper.CreatePartFromString("<testmessage2></testmessage2>");
            var msgPart3 = MessageHelper.CreatePartFromString("<testmessage3></testmessage3>");
            msgPart2.ContentType = "application/xml";
            msgPart3.ContentType = "application/xml";
            var msg = MessageHelper.CreateFromString("<testmessage1></testmessage1>");
            msg.AddPart("invoice2", msgPart2, false);
            msg.AddPart("invoice3", msgPart3, false);
            msg.BodyPart.ContentType = "application/xml";
            pipeline.AddComponent(component, PipelineStage.Encode);

            var result = pipeline.Execute(msg);

            int i = 1;
            ZipArchive zipArchive = new ZipArchive(result.BodyPart.GetOriginalDataStream());
            foreach (var zipEntry in zipArchive.Entries)
            {
                Guid g;
                Assert.IsTrue(Guid.TryParse(Path.GetFileNameWithoutExtension(zipEntry.Name), out g));
                Assert.AreEqual(".xml", Path.GetExtension(zipEntry.Name));
                using (var reader = new StreamReader(zipEntry.Open(), Encoding.Unicode))
                {
                    Assert.AreEqual(string.Format("<testmessage{0}></testmessage{0}>", i), reader.ReadToEnd());
                }
                i++;
            }
        }

        [TestMethod]
        public void TestCompressSingleMessagePart()
        {
            var pipeline = PipelineFactory.CreateEmptySendPipeline();

            var component = new CompressMessage { DefaultZipEntryFileExtension = "xml" };

            string messageContent = "<testmessage1></testmessage1>";
            var msg = MessageHelper.CreateFromString(messageContent);

            pipeline.AddComponent(component, PipelineStage.Encode);

            var result = pipeline.Execute(msg);

            ZipArchive zipArchive = new ZipArchive(result.BodyPart.GetOriginalDataStream());
            Assert.AreEqual(1, zipArchive.Entries.Count);
            ZipArchiveEntry zipEntry = zipArchive.Entries[0];
            Guid g;
            Assert.IsTrue(Guid.TryParse(Path.GetFileNameWithoutExtension(zipEntry.Name), out g));
            Assert.AreEqual(".xml", Path.GetExtension(zipEntry.Name));
            using (var reader = new StreamReader(zipEntry.Open(), Encoding.Unicode))
            {
                Assert.AreEqual(messageContent, reader.ReadToEnd());
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestCompressInvalidExtension()
        {
            var pipeline = PipelineFactory.CreateEmptySendPipeline();

            var component = new CompressMessage { DefaultZipEntryFileExtension = ".xml" };

            var msg = MessageHelper.CreateFromString("<testmessage1></testmessage1>");

            pipeline.AddComponent(component, PipelineStage.Encode);

            var result = pipeline.Execute(msg);
        }
    }
}
