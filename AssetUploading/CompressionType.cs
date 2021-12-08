using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PendulumClient.AssetUploading
{
    public enum CompressionType
    {
        None = 0,
        LZMA,
        LZ4,
        LZ4HC,
        LZHAM
    }
}
