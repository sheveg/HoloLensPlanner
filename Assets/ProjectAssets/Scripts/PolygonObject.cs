using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using System;

namespace HoloLensPlanner
{
    /// <summary>
    /// Base class for all objects which define a polygon. Makes the object respond to the HoloLens gaze.
    /// </summary>
    public class PolygonObject : MonoBehaviour, IFocusable
    {
        /// <summary>
        /// Polygon to which the object corresponds to.
        /// </summary>
        protected Polygon m_RootPolygon { get; private set; }

        /// <summary>
        /// Returns whether the object is focused by the HoloLens gaze.
        /// </summary>
        protected bool m_IsFocused { get; private set; }

        /// <summary>
        /// Is the object in movement mode?
        /// </summary>
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


