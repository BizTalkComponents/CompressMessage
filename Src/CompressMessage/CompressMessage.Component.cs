using System;
using System.Collections;
using System.Linq;
using BizTalkComponents.Utils;

namespace BizTalkComponents.PipelineComponents.CompressMessage
{
    public partial class CompressMessage
    {
        public string Name { get { return "Compress Message"; } }
        public string Version { get { return "1.0"; } }
        public string Description { get { return "Adds every message part to a zip archive."; } }

        public IEnumerator Validate(object projectSystem)
        {
            return ValidationHelper.Validate(this, false).ToArray().GetEnumerator();
        }

        public bool Validate(out string errorMessage)
        {
            var errors = ValidationHelper.Validate(this, true).ToArray();

            if (errors.Any())
            {
                errorMessage = string.Join(",", errors);

                return false;
            }

            errorMessage = string.Empty;

            return true;
        }

        public IntPtr Icon { get { return IntPtr.Zero; } }

        public void GetClassID(out Guid classID)
        {
            classID = Guid.Parse("");
        }

        public void InitNew()
        {
            
        }
    }
}