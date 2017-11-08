using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace HoloLensPlanner
{
    [CustomEditor(typeof(GazeResponder), true)]
    public class GazeResponderEditor : Editor
    {
        private GazeResponder m_GazeResponder;

        protected virtual void Awake()
        {
            m_GazeResponder = (GazeResponder)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();
            
            // show only the variable needed for the chosen focus event
            switch (m_GazeResponder.OnFocus)
            {
                case FocusEvent.Color:
                    m_GazeResponder.EventColor = EditorGUILayout.ColorField("Color", m_GazeResponder.EventColor);
                    break;
                case FocusEvent.Outline:
                    m_GazeResponder.EventOutlineColor = EditorGUILayout.ColorField("Outline color", m_GazeResponder.EventOutlineColor);
                    break;
                case FocusEvent.Scale:
                    m_GazeResponder.EventScale = EditorGUILayout.FloatField("Scale", m_GazeResponder.EventScale);
                    break;
                case FocusEvent.None:
                default:
                    break;
            }
        }
    }
}


