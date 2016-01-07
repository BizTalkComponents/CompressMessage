using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using BizTalkComponents.PipelineComponents.CompressMessage.Utils;
using BizTalkComponents.Utils;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using IComponent = Microsoft.BizTalk.Component.Interop.IComponent;

namespace BizTalkComponents.PipelineComponents.CompressMessage
{
    [ComponentCategory(CategoryTypes.CATID_PipelineComponent)]
    [Guid("3427E840-C3AE-4E20-9116-A327B8D6D9C8")]
    [ComponentCategory(CategoryTypes.CATID_Encoder)]
    public partial class CompressMessage : IComponent, IBaseComponent,
                                         IComponentUI, IPersistPropertyBag
    {
        private const string DefaultZipEntryFileExtensionPropertyName = "DefaultZipEntryFileExtension";

        [DisplayName("Default Zip Entry file extension")]
        [Description("Specifies the fallback file extension to be used for all zip entry files if no other extension is specified.")]
        [RegularExpression(@"[a-zA-Z0-9]{3}",
        ErrorMessage = "File extension should be 3 characters, i.e. xml")]
        public string DefaultZipEntryFileExtension { get; set; }

        public IBaseMessage Execute(IPipelineContext pContext, IBaseMessage pInMsg)
        {
            string errorMessage;

            if (!Validate(out errorMessage))
            {
                throw new ArgumentException(errorMessage);
            }

            var outMsg = pContext.GetMessageFactory().CreateMessage();
            outMsg.Context = pInMsg.Context;
            var bodyPart = pContext.GetMessageFactory().CreateMessagePart();

            using (var compressionUtil = new CompressionUtil())
            {
                for (var i = 0; i < pInMsg.PartCount; i++)
                {
                    string partName;
                    var part = pInMsg.GetPartByIndex(i, out partName);


                    var fileName = GetFileName(part);

                    compressionUtil.AddMessage(part.GetOriginalDataStream(), fileName);
                }

                bodyPart.Data = compressionUtil.GetZip();
                pContext.ResourceTracker.AddResource(bodyPart.Data);
                bodyPart.Charset = "utf-8";
                bodyPart.ContentType = "application/zip";
            }

            outMsg.AddPart("Body", bodyPart, true);

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
                else if (!string.IsNullOrEmpty(DefaultZipEntryFileExtension))
                {
                    extension = string.Concat(".", DefaultZipEntryFileExtension);
                }


                fileName = Guid.NewGuid() + extension;
            }

            return fileName;
        }

        public void Load(IPropertyBag propertyBag, int errorLog)
        {
            DefaultZipEntryFileExtension = PropertyBagHelper.ToStringOrDefault(PropertyBagHelper.ReadPropertyBag<string>(propertyBag, DefaultZipEntryFileExtensionPropertyName, DefaultZipEntryFileExtension), string.Empty);
        }

        public void Save(IPropertyBag propertyBag, bool clearDirty, bool saveAllProperties)
        {
            PropertyBagHelper.WritePropertyBag(propertyBag, DefaultZipEntryFileExtensionPropertyName, DefaultZipEntryFileExtension);
        }
    }
}
