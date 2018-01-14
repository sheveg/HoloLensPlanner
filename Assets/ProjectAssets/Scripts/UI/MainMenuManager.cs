using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity;
using UnityEngine.UI;
using HoloLensPlanner.TEST;

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

            TileMenuManager.Instance.Hide();
        }

        public void StartFollow()
        {
            m_CanvasGroup.alpha = 1f;
            m_TagAlong.enabled = true;
        }

        public void StopFollow()
        {
            m_CanvasGroup.alpha = m_HideAlpha;
            m_TagAlong.enabled = false;
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void createFloor()
        {
            RoomManager.Instance.CreateFloorPlane();
            StopFollow();
        }

        private void showObjectMenu()
        {
            Debug.Log("Not implemented!");
        }

        private void showTileMenu()
        {
            TileMenuManager.Instance.Show();
            Hide();
        }


    }
}


