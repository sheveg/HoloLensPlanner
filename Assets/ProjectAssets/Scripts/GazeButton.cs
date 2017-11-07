using UnityEngine;
using UnityEngine.UI;
using HoloToolkit.Unity.InputModule;
using System;
using System.Collections;

namespace HoloLensPlanner
{
    [RequireComponent(typeof(Graphic))]
    public class GazeButton : GazeResponder
    {
        
        private void Start()
        {

        }

        public override void SetDefaults()
        {
            base.SetDefaults();
        }

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