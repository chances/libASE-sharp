using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libASEsharp
{
    /// <summary>
    /// A 3D transformation.
    /// </summary>
    public struct Transform
    {
        /// <summary>
        /// Transformation name
        /// </summary>
        public string name;
        /// <summary>
        /// Inherited position (?)
        /// </summary>
        public int[] inheritPos;
        /// <summary>
        /// Inherited rotation (?)
        /// </summary>
        public int[] inheritRot;
        /// <summary>
        /// Inherited scale (?)
        /// </summary>
        public int[] inheritScl;
        /// <summary>
        /// Transformation matrix (?)
        /// </summary>
        public float[][] matrix;
        /// <summary>
        /// Transformation position.
        /// </summary>
        public Vector3D pos;
        /// <summary>
        /// Axis of rotation
        /// </summary>
        public Vector3D rotAxis;
        /// <summary>
        /// Rotation angle
        /// </summary>
        public float rotAngle;
        /// <summary>
        /// Transformation scale
        /// </summary>
        public Vector3D scale;
        /// <summary>
        /// Scalar axis
        /// </summary>
        public Vector3D scaleAxis;
        /// <summary>
        /// Scalar angle
        /// </summary>
        public float scaleAngle;
    }
}