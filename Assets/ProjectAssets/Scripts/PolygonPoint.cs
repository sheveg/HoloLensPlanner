using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using System;

namespace HoloLensPlanner
{
    public class PolygonPoint : PolygonObject, IInputClickHandler, IHoldHandler
    {
        public PolygonLine IngoingEdge { get; set; }
        public PolygonLine OutgoingEdge { get; set; }

        private IInputSource currentInputSource;
        private uint currentInputSourceId;

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
                currentInputSource = eventData.InputSource;
                currentInputSourceId = eventData.SourceId;
                startMoving();


            }
                
        }

        #endregion // Interface implementations

        private void startMoving()
        {
            if (m_IsMoving)
                return;

            m_IsMoving = true;
            Vector3 handPosition;
            currentInputSource.TryGetPointerPosition(currentInputSourceId, out handPosition);

            if (m_RootPolygon.MeshPlane)
            {
                //currentI
            }
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

