using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloLensPlanner
{
    [RequireComponent(typeof(Renderer))]
    public class GazeObject : GazeResponder
    {
        protected override void OnColor()
        {
            base.OnColor();
        }

        protected override void OnOutline()
        {
            base.OnOutline();
        }

        protected override void OnScale()
        {
            base.OnScale();
        }
    }
}

