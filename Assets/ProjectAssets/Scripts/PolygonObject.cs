using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using System;

namespace HoloLensPlanner
{
    public class PolygonObject : MonoBehaviour, IFocusable
    {
        protected Polygon m_RootPolygon { get; private set; }

        protected bool m_IsFocused { get; private set; }

        protected bool m_IsMoving { get; set; }

        public void OnFocusEnter()
        {
            if (m_RootPolygon.MeshPlane == null)
                return;

            m_IsFocused = true;
        }

        public void OnFocusExit()
        {
            if (m_RootPolygon.MeshPlane == null)
                return;

            m_IsFocused = false;
        }

        public void SetRootPolygon(Polygon polygon)
        {
            transform.parent = polygon.transform;
            m_RootPolygon = polygon;
        }
    }
}


