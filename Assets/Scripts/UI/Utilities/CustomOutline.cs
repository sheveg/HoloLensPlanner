using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HoloLensPlanner
{
    /// <summary>
    /// Simple script which to show a rectangle outline on a image. Relies on a outline sprite to work. Very basic.
    /// </summary>
    public class CustomOutline : MonoBehaviour
    {

        [SerializeField]
        private Image OutlineImage;

        [SerializeField]
        private Color OutlineColor;

        private void Start()
        {
            if (!OutlineImage)
                return;

            OutlineImage.color = OutlineColor;
        }

        private void OnEnable()
        {
            if (!OutlineImage)
                return;

            OutlineImage.gameObject.SetActive(true);
        }

        private void OnDisable()
        {
            if (!OutlineImage)
                return;

            OutlineImage.gameObject.SetActive(false);
        }
    }
}


