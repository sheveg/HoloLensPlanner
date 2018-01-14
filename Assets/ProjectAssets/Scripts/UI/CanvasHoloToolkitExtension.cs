using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;

namespace HoloLensPlanner
{
    /// <summary>
    /// Wrapper class for a Canvas. Because with the Toolkit version 2017.2+ all world canvas need to assign the UIRayCastCamera of the FocusManager as EventCamera to recognize UI objects.
    /// </summary>
    [RequireComponent(typeof(Canvas))]
    public class CanvasHoloToolkitExtension : MonoBehaviour
    {
        private Canvas m_Canvas;

        private void Start()
        {
            FocusManager.AssertIsInitialized();
            m_Canvas = GetComponent<Canvas>();
            if (m_Canvas.renderMode == RenderMode.WorldSpace)
            {
                m_Canvas.worldCamera = FocusManager.Instance.UIRaycastCamera;
            }
        }

    }
}


