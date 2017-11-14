using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using System;

namespace HoloLensPlanner
{
    public class PolygonPoint : MonoBehaviour, IInputClickHandler
    {

        public GameObject Root { get; set; }
        public bool isStart { get; set; }
        public PolygonLine IngoingEdge { get; set; }
        public PolygonLine OutgoingEdge { get; set; }

        #region Interface implementations

        public void OnInputClicked(InputClickedEventData eventData)
        {
            if (GazeManager.Instance.HitObject == gameObject)
            {
                Debug.Log("Hit");
            }
        }

        #endregion // Interface implementations



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

