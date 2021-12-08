using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PendulumClient.ColorModuleV2
{
    class CMV2_OverrideObject
    {
        public string Selector { get; }
        public string Body { get; }

        internal CMV2_OverrideObject(string selector, string body)
        {
            Selector = selector;
            Body = body;
        }
    }
}
