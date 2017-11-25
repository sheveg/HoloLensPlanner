using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using System;
using UnityEngine.Events;

namespace HoloLensPlanner.Tutorials
{
    public class OnClickEvent : MonoBehaviour, IInputClickHandler
    {
        [SerializeField]
        private UnityEvent m_ActivateEvent;

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
            if (m_ActivateEvent != null)
            {
                m_ActivateEvent.Invoke();
            }
        }
    }
}


