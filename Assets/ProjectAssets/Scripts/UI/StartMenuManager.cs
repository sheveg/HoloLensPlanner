using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace HoloLensPlanner
{
    public class StartMenuManager : Singleton<StartMenuManager>
    {
        /// <summary>
        /// Loads the main scene?
        /// </summary>
        [SerializeField]
        private Button StartButton;

        /// <summary>
        /// Opens the tutorial menu.
        /// </summary>
        [SerializeField]
        private Button TutorialButton;

        [SerializeField]
        private string MainSceneName;

        private void Start()
        {
            StartButton.onClick.AddListener(loadMainScene);
        }

        private void loadMainScene()
        {
            SceneManager.LoadScene(MainSceneName);
        }
    }
}


