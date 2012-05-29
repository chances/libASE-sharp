using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libASEsharp
{
    /// <summary>
    /// A 3D face.
    /// </summary>
    public struct Face
    {
        /// <summary>
        /// Face vertex
        /// </summary>
        public int[] vertex;
        /// <summary>
        /// Texture coordinated
        /// </summary>
        public int[] textureCoordinates;
        /// <summary>
        /// Face smoothing (bool?)
        /// </summary>
        public int smoothing;
        /// <summary>
        /// Material ID (?)
        /// </summary>
        public int mtlid;
        /// <summary>
        /// Product of the length of sides A & B (?)
        /// </summary>
        public int ab;
        /// <summary>
        /// Product of the length of sides B & C (?)
        /// </summary>
        public int bc;
        /// <summary>
        /// Product of the length of sides C & A (?)
        /// </summary>
        public int ca;
        /// <summary>
        /// Face normal
        /// </summary>
        public Vector3D faceNormal;
    }
}