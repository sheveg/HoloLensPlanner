using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HoloToolkit.Unity;

namespace HoloLensPlanner
{
    /// <summary>
    /// A hub which can be used to hold different menus as children. Can either follow the gaze or can be locked in space.
    /// </summary>
    public class MenuHub : SingleInstance<MenuHub>
    {
        /// <summary>
        /// Menus which can be shown under the hub.
        /// </summary>
        [SerializeField]
        private List<GameObject> Menus = new List<GameObject>();

        /// <summary>
        /// Button to toggle pin status.
        /// </summary>
        [SerializeField]
        private Button PinButton;

        /// <summary>
        /// Image background when hub is pinned for additional visual cue.
        /// </summary>
        [SerializeField]
        private Image PinnedBackground;

        /// <summary>
        /// Reference to the current active menu from the menu list.
        /// </summary>
        public GameObject CurrentMenu { get; private set; }

        /// <summary>
        /// Is the hub currently locked in space?
        /// </summary>
        private bool m_Pinned;

        /// <summary>
        /// Tagalong component which controls the gaze follow behavior.
        /// </summary>
        private SimpleTagalong m_Tagalong;

        /// <summary>
        /// Rotation of pin button in idle state.
        /// </summary>
        private Vector3 m_PinIdleRotation;

        /// <summary>
        /// Rotation of pin button in locked state.
        /// </summary>
        private Vector3 m_PinLockedRotation;

        private void Start()
        {
            m_Tagalong = GetComponent<SimpleTagalong>();
            PinButton.onClick.AddListener(TogglePinStatus);
            m_PinIdleRotation = Vector3.zero;
            m_PinLockedRotation = new Vector3(0, 0, 90f);
            for (int i = 1; i < Menus.Count; i++)
            {
                Menus[i].gameObject.SetActive(false);
            }
            CurrentMenu = Menus[0];
            CurrentMenu.gameObject.SetActive(true);
        }

        /// <summary>
        /// Toggles the pin status to either free or locked state.
        /// </summary>
        public void TogglePinStatus()
        {
            if (m_Pinned)
            {
                Unpin();
            }
            else
            {
                Pin();
            }
        }

        /// <summary>
        /// Locks the hub in space.
        /// </summary>
        public void Pin()
        {
            m_Pinned = true;
            m_Tagalong.enabled = false;
            PinButton.transform.localEulerAngles = m_PinLockedRotation;
            PinnedBackground.gameObject.SetActive(true);
        }

        /// <summary>
        /// Enables the gaze following behavior of the hub.
        /// </summary>
        public void Unpin()
        {
            m_Pinned = false;
            m_Tagalong.enabled = true;
            PinButton.transform.localEulerAngles = m_PinIdleRotation;
            PinnedBackground.gameObject.SetActive(false);
        }

        /// <summary>
        /// Shows the given menu if not already the case.
        /// </summary>
        /// <param name="menu"></param>
        public void ShowMenu(GameObject menu)
        {
            if (Menus.Contains(menu) && CurrentMenu != menu)
            {
                CurrentMenu.SetActive(false);
                menu.gameObject.SetActive(true);
                CurrentMenu = menu;
            }
        }

        /// <summary>
        /// Hides the given menu if it is active and shows the default menu, which is the first entry in the menu list.
        /// </summary>
        /// <param name="menu"></param>
        public void HideMenu(GameObject menu)
        {
            if (Menus.Contains(menu) && CurrentMenu == menu)
            {
                menu.gameObject.SetActive(false);
                CurrentMenu = Menus[0];
                CurrentMenu.SetActive(true);
            }
        }

        /// <summary>
        /// Hides the current menu and shows the default menu, which is the first entry in the menu list.
        /// </summary>
        public void ShowDefaultMenu()
        {
            if (CurrentMenu == Menus[0])
                return;

            CurrentMenu.SetActive(false);
            CurrentMenu = Menus[0];
            CurrentMenu.SetActive(true);
        }
    }
}


