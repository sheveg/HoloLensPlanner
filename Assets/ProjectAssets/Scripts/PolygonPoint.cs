using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using System;
using HoloLensPlanner.GazeResponse;

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
                Debug.Log("HitPoint");
                // use the click so no other events are triggered
                eventData.Use();
                deletePoint();
            }
        }

        private void deletePoint()
        {
            Debug.Log("DeletePoint");
            // if the polygon is in edit mode and this point is the last point created
            if (PolygonManager.Instance.CurrentPolygon == m_RootPolygon && !m_RootPolygon.IsFinished && m_RootPolygon.Points[m_RootPolygon.Points.Count - 1] == this)
            {
                if (m_RootPolygon.Points.Count == 1)
                {
                    Destroy(gameObject);
                    return;
                }
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

                // enable gazeObject component on previous point again, as this is the new last point
                GazeObject pointGazeObject = previousPoint.GetComponent<GazeObject>();
                if (pointGazeObject)
                {
                    pointGazeObject.enabled = true;
                }
                Destroy(gameObject);
            }
        }
    }
}

