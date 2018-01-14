using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity;
using UnityEngine.UI;

namespace HoloLensPlanner
{
    public class MainMenuManager : SingleInstance<MainMenuManager>
    {
        [SerializeField]
        private Button CreateFloorButton;

        [SerializeField]
        private Button ShowTileMenuButton;

        [SerializeField]
        private Button ShowObjectMenuButton;

        /// <summary>
        /// Component for blending. Canvasgroup has an alpha value which blends the whole UI.
        /// </summary>
        private CanvasGroup m_CanvasGroup;

        /// <summary>
        /// When we show/hide the menu we simply enable/disable following the camera and blend it out/in.
        /// </summary>
        private SimpleTagalong m_TagAlong;

        private const float m_HideAlpha = 0.2f;

        private void Start()
        {
            CreateFloorButton.onClick.AddListener(createFloor);
            ShowTileMenuButton.onClick.AddListener(showTileMenu);
            ShowObjectMenuButton.onClick.AddListener(showObjectMenu);

            m_CanvasGroup = GetComponent<CanvasGroup>();
            m_TagAlong = GetComponent<SimpleTagalong>();
        }

        public void Show()
        {
            m_CanvasGroup.alpha = 1f;
            m_TagAlong.enabled = true;
        }

        public void Hide()
        {
            m_CanvasGroup.alpha = m_HideAlpha;
            m_TagAlong.enabled = false;
        }

        private void createFloor()
        {
            RoomManager.Instance.CreateFloorPlane();
            Hide();
        }

        private void showObjectMenu()
        {
            Debug.Log("Not implemented!");
        }

        private void showTileMenu()
        {
            Debug.Log("Not implemented!");
        }


    }
}


