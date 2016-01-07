using Microsoft.BizTalk.Streaming;
using System;
using System.IO;
using System.IO.Compression;

namespace BizTalkComponents.PipelineComponents.CompressMessage.Utils
{
    public class CompressionUtil : IDisposable
    {
        private readonly ZipArchive _zipArchive;
        private readonly Stream _outputStream;

        public CompressionUtil()
        {
            _outputStream = new VirtualStream();
            _zipArchive = new ZipArchive(_outputStream, ZipArchiveMode.Create, true);
        }

        public void AddMessage(Stream msg, string filename, CompressionLevel compressionLevel = CompressionLevel.Optimal)
        {
            var entry = _zipArchive.CreateEntry(filename, compressionLevel);

            using (var entryStream = entry.Open())
            {
                msg.CopyTo(entryStream);
            }

        }

        public Stream GetZip()
        {
            Dispose();
            _outputStream.Position = 0;

            return _outputStream;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _zipArchive.Dispose();
                }

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion
    }
}