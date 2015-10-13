using System;
using System.Collections;

namespace BizTalkComponents.PipelineComponents.CompressMessage
{
    public partial class CompressMessage
    {
        public string Name { get { return "Compress Message"; } }
        public string Version { get { return "1.0"; } }
        public string Description { get { return "Adds every message part to a zip archive."; } }

        public IEnumerator Validate(object projectSystem)
        {
            return null;
        }

        public IntPtr Icon { get { return IntPtr.Zero; } }
    }
}