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

        private const float m_HideAlpha = 0.2f;

        private void Start()
        {
            CreateFloorButton.onClick.AddListener(createFloor);
            ShowTileMenuButton.onClick.AddListener(showTileMenu);
            ShowObjectMenuButton.onClick.AddListener(showObjectMenu);
        }

        private void createFloor()
        {
            MenuHub.Instance.Pin();
            RoomManager.Instance.CreateFloorPlane();
        }

        private void showObjectMenu()
        {
            Debug.Log("Not implemented!");
        }

        private void showTileMenu()
        {
            MenuHub.Instance.ShowMenu(TileMenuManager.Instance.gameObject);
        }
    }
}


