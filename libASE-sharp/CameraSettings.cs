using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libASEsharp
{
    /// <summary>
    /// Camera settings.
    /// </summary>
    public struct CameraSettings
    {
        public int timeValue;
        public float nearVal;
        public float farVal;
        public float fov;
        public float tdist;
    }
}