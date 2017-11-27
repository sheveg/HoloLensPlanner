using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using System;

namespace HoloLensPlanner.Tutorials
{
    public class ShowObjectMenuInstruction : MonoBehaviour, IInputHandler
    {
        [SerializeField]
        private GameObject m_ObjectMenuInstruction;

        private void OnEnable()
        {
            InputManager.Instance.PushModalInputHandler(gameObject);
        }

        private void OnDisable()
        {
            InputManager.Instance.PopModalInputHandler();
        }

        public void OnInputDown(InputEventData eventData)
        {

        }

        public void OnInputUp(InputEventData eventData)
        {
            StartCoroutine(objectMenuCoroutine());
        }

        private IEnumerator objectMenuCoroutine()
        {
            while (!PolygonManager.Instance.CurrentPolygon.IsFinished)
            {
                yield return null;
            }

            yield return new WaitForSeconds(1f);
            gameObject.SetActive(false);
            m_ObjectMenuInstruction.SetActive(true);
        }


    }
}


