using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloLensPlanner
{
    public class Polygon : MonoBehaviour
    {
        public List<PolygonPoint> Points { get; set; }

        public bool IsFinished { get; set; }

        private void Awake()
        {
            Points = new List<PolygonPoint>();
            IsFinished = false;
        }

    }
}


