using System;
using System.Collections.Generic;

namespace BizTalkComponents.PipelineComponents.CompressMessage.Utils
{
    public static class MimeUtils
    {

        private static readonly Dictionary<string, string> FileExtensions = new Dictionary<string, string>()
                {
                    {"application/pdf", ".pdf"},
                    {"application/xml", ".xml"},
                    {"text/xml", ".xml"},
                    {"text/plain", ".txt"}
                };


        internal static string GetFileExtensionForMimeType(string mimeType)
        {
            string extension;

            if (!FileExtensions.TryGetValue(mimeType, out extension))
            {
                throw new InvalidOperationException(string.Format("The specified mime type {0} does not have a associated file extension.", mimeType));
            }

            return extension;
        }
    }
}
