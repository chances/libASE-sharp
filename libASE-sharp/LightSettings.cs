using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libASEsharp
{
    /// <summary>
    /// Light settings
    /// </summary>
    public struct LightSettings
    {
        public int timeValue;
        public ColorRGB color;
        public float intensity;
        public float aspect;
        public float tdist;
        public float mapBias;
        public float mapRange;
        public int mapSize;
        public float rayBias;
    }
}