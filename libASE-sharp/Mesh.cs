using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libASEsharp
{
    /// <summary>
    /// A 3D mesh.
    /// </summary>
    public struct Mesh
    {
        /// <summary>
        /// Time (?)
        /// </summary>
        public int timeValue;

        public int vertexCount;
        public int textureCoordinateCount;
        public int faceCount;

        public List<Vector3D> verticies;
        public List<Vector3D> vertexNormals;
        public List<Vector3D> textureCoordinates;
        public List<Face> faces;
    }
}