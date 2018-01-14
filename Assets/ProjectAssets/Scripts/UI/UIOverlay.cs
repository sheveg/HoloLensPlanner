﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HoloLensPlanner
{
    public class UIOverlay : MonoBehaviour
    {
        public Material OverlayMaterialImage;
        public Material OverlayMaterialText;

        public void ApplyMaterial()
        {
            if (OverlayMaterialImage == null)
            {
                Debug.Log("Cannot apply material! OverlayMaterial is not set!");
                return;
            }
            var images = GetComponentsInChildren<Image>(true);
            foreach (var img in images)
            {
                img.material = OverlayMaterialImage;
            }
            var texts = GetComponentsInChildren<Text>(true);
            foreach (var text in texts)
            {
                text.material = OverlayMaterialText;
            }
        }
    }
}

