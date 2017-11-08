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
            polygon.Root.transform.SetParent(transform, true);
            polygon.Root.name = "Polygon";
        }
    }
}


