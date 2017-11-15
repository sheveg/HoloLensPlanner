using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloLensPlanner
{
    public class Polygon : MonoBehaviour
    {
        public List<PolygonPoint> Points { get; set; }

        public bool IsFinished { get; set; }

        public RoomPlane MeshPlane { get; set; }

        public Vector3 Center { get { return calculateCenter(); } }

        private void Awake()
        {
            Points = new List<PolygonPoint>();
            IsFinished = false;
        }

        private Vector3 calculateCenter()
        {
            Vector3 center = Vector3.zero;
            foreach (var point in Points)
            {
                center += point.transform.position;
            }
            return center / Points.Count;
        }
    }
}


