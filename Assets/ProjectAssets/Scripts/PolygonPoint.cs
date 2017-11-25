using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using System;

namespace HoloLensPlanner
{
    /// <summary>
    /// Boundary vertex of a polygon. Can be moved/focused by the HoloLens.
    /// </summary>
    public class PolygonPoint : PolygonObject, IInputClickHandler
    {
        public PolygonLine IngoingEdge { get; set; }
        public PolygonLine OutgoingEdge { get; set; }

        public void OnInputClicked(InputClickedEventData eventData)
        {
            if (GazeManager.Instance.HitObject == gameObject)
            {
                // use the click so no other events are triggered
                eventData.Use();
                deletePoint();
            }
        }

        private void deletePoint()
        {
            if (PolygonManager.Instance.CurrentPolygon == m_RootPolygon && !m_RootPolygon.IsFinished)
            {
                // first clear the references in the polygon
                PolygonPoint previousPoint;
                if (IngoingEdge.From == this)
                    previousPoint = IngoingEdge.To;
                else
                    previousPoint = IngoingEdge.From;
                previousPoint.OutgoingEdge = null;
                m_RootPolygon.Points.Remove(this);

                // then destroy the actual gameObjects
                if (IngoingEdge)
                    Destroy(IngoingEdge.gameObject);
                if (OutgoingEdge)
                    Destroy(OutgoingEdge.gameObject);
                Destroy(gameObject);
            }
        }
    }
}

