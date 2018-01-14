using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity;

namespace HoloLensPlanner
{
    /// <summary>
    /// Tagalong which always follows the camera without any smoothing and just stays in front of the camera.
    /// </summary>
    public class HardTagalong : MonoBehaviour
    {
        public float Distance = 2f;


        private void Update()
        {
            transform.position = CameraCache.Main.transform.position + Camera.main.transform.forward * Distance;
        }

    }
}


