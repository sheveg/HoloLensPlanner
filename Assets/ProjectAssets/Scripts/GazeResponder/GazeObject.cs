using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloLensPlanner.GazeResponse
{
    /// <summary>
    /// GazeObject transforms a standard gameObject into an object which responds to the HoloLens gaze. Depending on the <see cref="GazeResponder.OnFocus"/> variable
    /// the gameObject reacts with the corresponding event, e.g. it scales up when <see cref="FocusEvent.Scale"/> is selected.
    /// </summary>
    //[RequireComponent(typeof(Renderer))]
    public class GazeObject : GazeResponder
    {
        protected override void OnColor()
        {
            
        }

        protected override void OnOutline()
        {
            
        }

        protected override void OnScale()
        {
            base.OnScale();
        }
    }
}

