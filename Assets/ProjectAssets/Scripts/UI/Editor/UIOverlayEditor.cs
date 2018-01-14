using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace HoloLensPlanner
{
    [CustomEditor(typeof(UIOverlay))]
    public class UIOverlayEditor : Editor
    {
        private UIOverlay m_UIOverlay;

        private void Awake()
        {
            m_UIOverlay = (UIOverlay)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();

            if (GUILayout.Button("Apply Material"))
            {
                m_UIOverlay.ApplyMaterial();
            }
        }
    }
}


