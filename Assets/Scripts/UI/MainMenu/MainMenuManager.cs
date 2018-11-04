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
        private Button ShowFloorMenuButton;

        [SerializeField]
        private Button ShowTileMenuButton;

        [SerializeField]
        private Button ShowObjectMenuButton;

        [SerializeField]
        private Button AssistantButton;

        private const float m_HideAlpha = 0.2f;

        private bool assistantOn = false;

        private void Start()
        {
            ShowFloorMenuButton.onClick.AddListener(showFloorMenu);
            ShowTileMenuButton.onClick.AddListener(showTileMenu);
            ShowObjectMenuButton.onClick.AddListener(showObjectMenu);
            AssistantButton.onClick.AddListener(startAssistant);
        }

        private void showFloorMenu()
        {
            MenuHub.Instance.ShowMenu(FloorMenuManager.Instance.gameObject);
        }

        private void showObjectMenu()
        {
            Debug.Log("Not implemented!");
        }

        private void showTileMenu()
        {
            MenuHub.Instance.ShowMenu(TileMenuManager.Instance.gameObject);
        }

        private void startAssistant()
        {
            if (!assistantOn)
            {
                MenuHub.Instance.ShowMenu(AssistantManager.Instance.gameObject);
                AssistantManager.Instance.activate();
                assistantOn = !assistantOn;
            }
            else
            {
                AssistantManager.Instance.deactivate();
                assistantOn = !assistantOn;
            }
        }
    }
}


