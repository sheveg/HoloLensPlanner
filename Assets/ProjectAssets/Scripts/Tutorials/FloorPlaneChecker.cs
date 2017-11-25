using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using System;

namespace HoloLensPlanner.Tutorials
{
    public class FloorPlaneChecker : MonoBehaviour, IInputClickHandler
    {
        [SerializeField]
        private GameObject m_NextInstruction;

        private void OnEnable()
        {
            InputManager.Instance.PushModalInputHandler(gameObject);
        }

        private void OnDisable()
        {
            InputManager.Instance.PopModalInputHandler();
        }

        public void OnInputClicked(InputClickedEventData eventData)
        {
            StartCoroutine(checkCoroutine());
        }

        private IEnumerator checkCoroutine()
        {
            int tries = 0;
            // wait until currentpolygon is created
            while (PolygonManager.Instance.CurrentPolygon == null)
            {
                if (tries > 1000)
                    yield break;
                tries++;
                yield return null;
            }
            if (RoomManager.Instance.CurrentPlaneType.HasValue &&
                RoomManager.Instance.CurrentPlaneType == PlaneType.Floor
                && PolygonManager.Instance.CurrentPolygon.Points.Count == 1)
            {
                gameObject.SetActive(false);
                m_NextInstruction.SetActive(true);
            }
        }
    }
}
