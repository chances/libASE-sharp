using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libASEsharp
{
    /// <summary>
    /// A 3D camera.
    /// </summary>
    public struct Camera
    {
        public string name;
        public string type;
        public Transform camera;
        public Transform target;
        public CameraSettings settings;
    }
}