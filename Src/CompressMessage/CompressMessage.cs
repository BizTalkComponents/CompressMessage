using System;
using System.Runtime.InteropServices;
using BizTalkComponents.PipelineComponents.CompressMessage.Utils;
using BizTalkComponents.Utils;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;

namespace BizTalkComponents.PipelineComponents.CompressMessage
{
    [ComponentCategory(CategoryTypes.CATID_PipelineComponent)]
    [Guid("3427E840-C3AE-4E20-9116-A327B8D6D9C8")]
    [ComponentCategory(CategoryTypes.CATID_Encoder)]
    public partial class CompressMessage : IComponent, IBaseComponent,
                                         IComponentUI
    {
        public IBaseMessage Execute(IPipelineContext pContext, IBaseMessage pInMsg)
        {
            var cu = new CompressionUtil();
            
            for (var i = 0; i < pInMsg.PartCount; i++)
            {
                string partName;
                var part = pInMsg.GetPartByIndex(i, out partName);

                
                var fileName = GetFileName(part);

                cu.AddMessage(part.GetOriginalDataStream(), fileName);
            }

            var outMsg = pContext.GetMessageFactory().CreateMessage();
            outMsg.Context = pInMsg.Context;
            var bodyPart = pContext.GetMessageFactory().CreateMessagePart();
            bodyPart.Data = cu.GetZip();
            bodyPart.Charset = "utf-8";
            bodyPart.ContentType = "application/zip";

            outMsg.AddPart("Body",bodyPart,true);

            return outMsg;
        }

        private string GetFileName(IBaseMessagePart part)
        {
            var receivedFileNameProperty = new ContextProperty(FileProperties.ReceivedFileName);
            var receivedFileName = part.PartProperties.Read(receivedFileNameProperty.PropertyName, receivedFileNameProperty.PropertyNamespace) as string;
            string fileName;
            string extension = string.Empty;
            if (!string.IsNullOrEmpty(receivedFileName))
            {
                fileName = receivedFileName;
            }
            else
            {
                if (!string.IsNullOrEmpty(part.ContentType))
                {
                    extension = MimeUtils.GetFileExtensionForMimeType(part.ContentType);    
                }
                
                fileName = Guid.NewGuid() + extension;    
            }

            return fileName;
        }
    }
}
