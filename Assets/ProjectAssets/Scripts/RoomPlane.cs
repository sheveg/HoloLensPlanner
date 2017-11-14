using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;

namespace HoloLensPlanner
{
    public class RoomPlane : MonoBehaviour
    {
        public Polygon MeshPolygon { get; private set; }
        
        public PlaneType Type { get; private set; }

        public void Setup(Polygon polygon, PlaneType planeType)
        {
            MeshPolygon = polygon;
            Type = planeType;
            polygon.transform.SetParent(transform, true);
            // level 
            projectPointsOnPlane(polygon);

        }

        
        private void projectPointsOnPlane(Polygon polygon)
        {
            // The projection of a point q onto a plane given by a point p and a normal n  is : q_proj = q - dot(q - p, n) * n
            foreach (var point in polygon.Points)
            {
                point.transform.position -= Vector3.Dot(point.transform.position - transform.position, transform.up) * transform.up;
            }
            // correct the lines to the new projected points
            for (int i = 1; i <= polygon.Points.Count; i++)
            {
                Vector3 from = polygon.Points[i - 1].transform.position;
                Vector3 to = polygon.Points[i % polygon.Points.Count].transform.position;
                polygon.Points[i - 1].OutgoingEdge.SetPosition(from, to);
            }
        }
    }
}


