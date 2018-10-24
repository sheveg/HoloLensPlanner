using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloLensPlanner.Utilities;

namespace HoloLensPlanner
{
    /// <summary>
    /// A polygon represents the outer boundaries of a <see cref="RoomPlane"/>. It is used in the creation process and when editing. 
    /// </summary>
    public class Polygon : MonoBehaviour
    {
        /// <summary>
        /// Points representing the polygon.
        /// </summary>
        public List<PolygonPoint> Points { get; set; }

        /// <summary>
        /// Helper variable to check whether the polygon is done.
        /// </summary>
        public bool IsFinished
        {
            get { return m_IsFinished; }
            set {
                m_IsFinished = value;
                if (m_IsFinished)
                {
                    foreach (var point in Points)
                    {
                        point.enabled = false;
                    }
                }
            }
        }

        private bool m_IsFinished;

        /// <summary>
        /// Cross reference to a <see cref="RoomPlane"/> when the polygon is finished. 
        /// </summary>
        public RoomPlane MeshPlane { get; set; }

        /// <summary>
        /// Average center of the polygon.
        /// </summary>
        public Vector3 Center {
            get
            {
                if (!m_Center.HasValue)
                    m_Center = calculateCenter();
                return m_Center.Value;
            }
        }

        /// <summary>
        /// Cached center reference.
        /// </summary>
        private Vector3? m_Center;

        private void Awake()
        {
            Points = new List<PolygonPoint>();
            IsFinished = false;
        }

        /// <summary>
        /// Calculates the center by the average of all points.
        /// </summary>
        /// <returns></returns>
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


