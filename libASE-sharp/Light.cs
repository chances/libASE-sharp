using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libASEsharp
{
    /// <summary>
    /// A 3D light
    /// </summary>
    public struct Light
    {
        public string name;
        public string type;
        public Transform transform;
        public string shadows;
        public bool useLight;
        public string spotShape;
        public bool useGlobal;
        public int absMapBias;
        public int overshoot;
        public LightSettings settings;
    }
}
