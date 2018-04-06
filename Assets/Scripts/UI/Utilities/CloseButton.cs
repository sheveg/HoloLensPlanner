using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HoloLensPlanner
{
    /// <summary>
    /// Utility class to enable the same behavior for every close button independent of the current view. Shows the default menu of the <see cref="MenuHub"/> (First menu in the list).
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class CloseButton : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {
            GetComponent<Button>().onClick.AddListener(OnClose);
        }

        private void OnClose()
        {
            MenuHub.Instance.ShowDefaultMenu();
        }
    }
}


