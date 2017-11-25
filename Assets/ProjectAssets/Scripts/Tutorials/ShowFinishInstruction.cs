﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using System;

namespace HoloLensPlanner.Tutorials
{
    public class ShowFinishInstruction : MonoBehaviour, IInputClickHandler
    {

        [SerializeField]
        private GameObject m_FinishInstruction;

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
            StartCoroutine(finishInstructionCoroutine());
        }

        private IEnumerator finishInstructionCoroutine()
        {
            // wait until user has at least two points
            while (RoomManager.Instance.CurrentPlaneType.HasValue &&
               RoomManager.Instance.CurrentPlaneType == PlaneType.Floor
               && PolygonManager.Instance.CurrentPolygon.Points.Count < 4)
            {
                yield return null;
            }
            gameObject.SetActive(false);
            m_FinishInstruction.SetActive(true);
        }
    }
}
