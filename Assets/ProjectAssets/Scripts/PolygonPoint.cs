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
    public class PolygonPoint : PolygonObject, IInputClickHandler, IHoldHandler
    {
        public PolygonLine IngoingEdge { get; set; }
        public PolygonLine OutgoingEdge { get; set; }

        private IInputSource m_CurrentInputSource;
        private uint m_CurrentInputSourceId;

        private Vector3 m_HandOffset;

        void Update()
        {
            if (m_IsMoving)
            {
                updateMoving();
            }
        }

        #region Interface implementations

        public void OnInputClicked(InputClickedEventData eventData)
        {
            if (GazeManager.Instance.HitObject == gameObject)
            {
                Debug.Log("Hit");
            }
        }

        public void OnHoldCanceled(HoldEventData eventData)
        {
            throw new NotImplementedException();
        }

        public void OnHoldCompleted(HoldEventData eventData)
        {
            throw new NotImplementedException();
        }

        public void OnHoldStarted(HoldEventData eventData)
        {
            if (m_IsFocused)
            {
                eventData.Use();
                m_CurrentInputSource = eventData.InputSource;
                m_CurrentInputSourceId = eventData.SourceId;
                startMoving();


            }
                
        }

        #endregion // Interface implementations

        private void startMoving()
        {
            if (m_IsMoving)
                return;

            // mark that we are moving the point
            m_IsMoving = true;

            // Add self as a modal input handler, to get all inputs during the manipulation
            InputManager.Instance.PushModalInputHandler(gameObject);

            // get the hand position
            Vector3 handPosition;
            m_CurrentInputSource.TryGetPointerPosition(m_CurrentInputSourceId, out handPosition);

            // if we already have a mesh, this means we have a plane on which we have to project to the point to
            if (m_RootPolygon.MeshPlane)
            {
                //currentI

            }

            m_HandOffset = transform.position - handPosition;
        }

        private void updateMoving()
        {
            if (!m_IsMoving)
                return;

            Vector3 handPosition;
            m_CurrentInputSource.TryGetPointerPosition(m_CurrentInputSourceId, out handPosition);

            transform.position = handPosition + m_HandOffset;

        }

        private void deletePoint()
        {
            if (!PolygonManager.Instance.CurrentPolygon.IsFinished)
            {
                if (IngoingEdge)
                    Destroy(IngoingEdge.gameObject);
                if (OutgoingEdge)
                    Destroy(OutgoingEdge.gameObject);
            }
        }


    }

}

