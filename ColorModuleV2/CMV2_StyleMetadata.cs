using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PendulumClient.ColorModuleV2
{
    class CMV2_StyleMetadata
    {
        public string Name = UnnamedName;

        public string Description = "";

        public string Author = "";

        public string VrcBuildNumber;

        public bool IsMixin;

        public int MixinPriority = 1;

        public bool DisabledByDefault;

        public List<string> SpritesToGrayscale = new List<string>();

        public const string UnnamedName = "<unnamed>";
    }
}
