using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using System;

namespace HoloLensPlanner.Tutorials
{
    public class OnClickGameObject : MonoBehaviour, IInputClickHandler
    {
        [SerializeField]
        private GameObject m_ActivateGameObject;

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
            if (m_ActivateGameObject)
            {
                gameObject.SetActive(false);
                m_ActivateGameObject.gameObject.SetActive(true);
            }
        }
    }
}


