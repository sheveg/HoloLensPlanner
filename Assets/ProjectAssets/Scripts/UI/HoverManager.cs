using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;

namespace HoloLensPlanner
{
    /// <summary>
    /// Manager class to display a <see cref="HoverText"/>. 
    /// </summary>
    public class HoverManager : SingleInstance<HoverManager>
    {
        /// <summary>
        /// Text field to display a <see cref="HoverText"/>  above the gaze. 
        /// </summary>
        [SerializeField]
        private Text HoverText;

        /// <summary>
        /// Transform component to correctly position a <see cref="HoverText"/>. 
        /// </summary>
        [SerializeField]
        private RectTransform HoverTextPosition;

        /// <summary>
        /// Required focus time to activate a hover text.
        /// </summary>
        public float HoverTimeThreshold = 1f;

        /// <summary>
        /// Offset to show the <see cref="HoverText"/> above the gaze. 
        /// </summary>
        private const float VerticalOffset = 0.03f;

        /// <summary>
        /// Offset to show the <see cref="HoverText"/> infront of the gaze. 
        /// </summary>
        private const float ForwardOffset = 0.03f;

        /// <summary>
        /// Shows the HoverText with the given message.
        /// </summary>
        /// <param name="message"></param>
        public void ShowHoverText(string message)
        {
            positionHoverText();
            HoverText.text = message;
            HoverTextPosition.gameObject.SetActive(true);
        }

        /// <summary>
        /// Hides the current HoverText message.
        /// </summary>
        public void HideHoverText()
        {
            HoverTextPosition.gameObject.SetActive(false);
            HoverText.text = "";
        }

        private void positionHoverText()
        {
            HoverTextPosition.position = GazeManager.Instance.HitPosition + Vector3.up * VerticalOffset + GazeManager.Instance.HitNormal * ForwardOffset;
        }
    }
}
