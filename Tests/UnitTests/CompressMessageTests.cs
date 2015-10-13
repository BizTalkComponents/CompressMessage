using System;
using System.IO;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
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
            msg.AddPart("invoice2",msgPart2,false);
            msg.AddPart("invoice3", msgPart3, false);
            msg.BodyPart.PartProperties.Write("ReceivedFileName","http://schemas.microsoft.com/BizTalk/2003/file-properties","invoice1.xml");
            pipeline.AddComponent(component,PipelineStage.Encode);

            var result = pipeline.Execute(msg);

            ZipInputStream zipInputStream = new ZipInputStream(result.BodyPart.GetOriginalDataStream());
            ZipEntry zipEntry = zipInputStream.GetNextEntry();
            int i = 1;
            while (zipEntry != null)
            {
                Assert.AreEqual(string.Format("invoice{0}.xml",i),zipEntry.Name);
                zipEntry = zipInputStream.GetNextEntry();
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

            ZipInputStream zipInputStream = new ZipInputStream(result.BodyPart.GetOriginalDataStream());
            ZipEntry zipEntry = zipInputStream.GetNextEntry();
            while (zipEntry != null)
            {
                Guid g;
                Assert.IsTrue(Guid.TryParse(Path.GetFileNameWithoutExtension(zipEntry.Name), out g));
                Assert.AreEqual(Path.GetExtension(zipEntry.Name), ".xml");
                zipEntry = zipInputStream.GetNextEntry();
            }
        }
    }
}
