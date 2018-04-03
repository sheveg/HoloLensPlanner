using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using HoloLensPlanner;
using HoloToolkit.UI.Keyboard;

namespace HoloLensPlanner
{
    [CustomEditor(typeof(KeyboardWithNumbers))]
    public class KeyboardWithNumbersEditor : Editor
    {
        KeyboardWithNumbers m_Keyboard;

        bool m_DebugMode = false;

        private void Awake()
        {
            m_Keyboard = (KeyboardWithNumbers)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            m_DebugMode = EditorGUILayout.Toggle("DebugMode", m_DebugMode);

            if (!m_DebugMode)
                return;

            if (GUILayout.Button("Alpha"))
            {
                m_Keyboard.PresentKeyboard(KeyboardWithNumbers.LayoutType.Alpha);
            }
            if (GUILayout.Button("Symbol"))
            {
                m_Keyboard.PresentKeyboard(KeyboardWithNumbers.LayoutType.Symbol);
            }
            if (GUILayout.Button("Email"))
            {
                m_Keyboard.PresentKeyboard(KeyboardWithNumbers.LayoutType.Email);
            }
            if (GUILayout.Button("URL"))
            {
                m_Keyboard.PresentKeyboard(KeyboardWithNumbers.LayoutType.URL);
            }
            if (GUILayout.Button("Number"))
            {
                m_Keyboard.PresentKeyboard(KeyboardWithNumbers.LayoutType.Number);
            }
        }
    }
}