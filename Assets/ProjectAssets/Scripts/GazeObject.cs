using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloLensPlanner
{
    /// <summary>
    /// GazeObject transforms a standard gameObject into an object which responds to the HoloLens gaze. Depending on the <see cref="GazeResponder.OnFocus"/> variable
    /// the gameObject reacts with the corresponding event, e.g. it scales up when <see cref="FocusEvent.Scale"/> is selected.
    /// </summary>
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

