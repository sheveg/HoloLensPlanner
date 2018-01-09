using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit;

namespace HoloLensPlanner.GazeResponse
{
    /// <summary>
    /// GazeUIObjectChild applies the OnFocus method to all its immediate children but itself.
    /// </summary>
    public class GazeUIObjectChild : GazeObjectChild
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


