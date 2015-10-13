using System;
using System.IO;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.BizTalk.Message.Interop;

namespace BizTalkComponents.PipelineComponents.CompressMessage.Utils
{
    public class CompressionUtil
    {
        private readonly ZipOutputStream _zipStream;
        private readonly MemoryStream _outputMemStream;

        public CompressionUtil()
        {
            _outputMemStream = new MemoryStream();
            _zipStream = new ZipOutputStream(_outputMemStream);
            _zipStream.SetLevel(3);
        }

        public void AddMessage(Stream msg, string filename)
        {
            var entry = new ZipEntry(filename) {DateTime = DateTime.Now};

            _zipStream.PutNextEntry(entry);
            StreamUtils.Copy(msg, _zipStream, new byte[4096]);
            _zipStream.CloseEntry();
        }

        public MemoryStream GetZip()
        {
            _zipStream.IsStreamOwner = false;
            _zipStream.Close();
            _outputMemStream.Position = 0;

            return _outputMemStream;
        }
    }
}