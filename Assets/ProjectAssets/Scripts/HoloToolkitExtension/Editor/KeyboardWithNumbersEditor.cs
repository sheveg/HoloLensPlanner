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

            if (GUILayout.Button("Doshit")) {
                Transform[] allChilds = m_Keyboard.GetComponentsInChildren<Transform>();
                foreach (var child in allChilds)
                {
                    KeyboardValueKey valueKey;
                    if ((valueKey = child.GetComponent<KeyboardValueKey>()) != null)
                    {
                        var valueKeyNumbers = child.gameObject.AddComponent<KeyboardWithNumbersValueKey>();
                        valueKeyNumbers.Value = valueKey.Value;
                        valueKeyNumbers.ShiftValue = valueKey.ShiftValue;
                        DestroyImmediate(valueKey);
                    }
                    KeyboardKeyFunc keyFunc;
                    if ((keyFunc = child.GetComponent<KeyboardKeyFunc>()) != null)
                    {
                        var keyFuncNumbers = child.gameObject.AddComponent<KeyboardWithNumbersKeyFunc>();
                        keyFuncNumbers.m_ButtonFunction = (KeyboardWithNumbersKeyFunc.Function)keyFunc.m_ButtonFunction;
                        DestroyImmediate(keyFunc);
                    }
                    var childKeyFunc = child.GetComponents<KeyboardWithNumbersKeyFunc>();
                    for (int i = 1; i < childKeyFunc.Length; i++)
                    {
                        DestroyImmediate(childKeyFunc[i]);
                    }
                }
                var allKeyFunc = m_Keyboard.GetComponentsInChildren<KeyboardKeyFunc>();
                foreach (var a in allKeyFunc)
                    DestroyImmediate(a);
            }

            


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