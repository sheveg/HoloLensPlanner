﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

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
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }
            if (GUILayout.Button("Reset"))
            {
                m_UIOverlay.Reset();
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }
        }
    }
}


