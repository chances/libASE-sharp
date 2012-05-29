using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libASEsharp
{
    /// <summary>
    /// A 3D geometric object
    /// </summary>
    public struct GeomObject
    {
        public string name;
        public Transform transform;
        public Mesh mesh;
        public bool motionBlur;
        public bool castShadow;
        public bool recieveShadow;
        /// <summary>
        /// GeomObject's material ID (?)
        /// </summary>
        public int material;
    }
}