using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libASEsharp
{
    public struct MaterialMap
    {
        public string name;
        public string mapclass;
        public int subno;
        public float amount;
        public string imagePath;
        public string type;
        public float uOffset;
        public float vOffset;
        public float uTiling;
        public float vTiling;
        public float angle;
        public float blur;
        public float blurOffset;
        public float noiseAmt;
        public float noiseSize;
        public int noiseLevel;
        public float noisePhase;
        public string bitmapFilter;
    }
}