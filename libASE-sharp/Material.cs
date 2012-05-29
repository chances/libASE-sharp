using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libASEsharp
{
    /// <summary>
    /// A material applied to a GeomObject.
    /// </summary>
    public struct Material
    {
        public string name;
        public string matclass;
        public ColorRGB ambient;
        public ColorRGB diffuse;
        public ColorRGB specular;
        public float shine;
        public float shineStrength;
        public float transparency;
        public float wiresize;
        public string shading;
        public float xpFalloff;
        public float selfIllum;
        public string falloff;
        public string xpType;
        public MaterialMap diffuseMaterialMap;
        public List<Material> subMaterials;
    }
}