using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;
using System;
using UnityEngine.UI;

namespace HoloLensPlanner
{
    /// <summary>
    /// Can be attached to a UI component to display a text after a certain threshold of focus time to explain the functionality.
    /// </summary>
    public class HoverText : MonoBehaviour, IFocusable
    {
        /// <summary>
        /// Which text should be displayed.
        /// </summary>
        public string Text;



        private float m_CurrentHoverTime = 0f;

        /// <summary>
        /// Indicates whether the hover text is already active or not.
        /// </summary>
        private bool m_IsActive;

        private bool m_IsFocused;

        public void OnFocusEnter()
        {
            m_IsFocused = true;
        }

        public void OnFocusExit()
        {
            m_IsFocused = false;
            m_IsActive = false;
            HoverManager.Instance.HideHoverText();
        }

        // Shows the hover text when the threshold is reached.
        private void Update()
        {
            if (!m_IsFocused || m_IsActive)
                return;

            if (m_CurrentHoverTime > HoverManager.Instance.HoverTimeThreshold)
            {
                m_CurrentHoverTime = 0f;
                HoverManager.Instance.ShowHoverText(Text);
                m_IsActive = true;
            }
            m_CurrentHoverTime += Time.unscaledDeltaTime;
            
        }
    }
}
