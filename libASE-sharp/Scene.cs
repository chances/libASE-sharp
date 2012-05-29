using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libASEsharp
{
    /// <summary>
    /// A 3D scene.
    /// </summary>
    public struct Scene
    {
        public int firstFrame;
        public int lastFrame;
        public int frameSpeed;
        public int ticksPerFrame;
        public ColorRGB backgroundLight;
        public ColorRGB ambientLight;

        public List<GeomObject> objs;
        public List<Light> lights;
        public List<Material> materials;
        public List<Camera> cameras;
    }
}