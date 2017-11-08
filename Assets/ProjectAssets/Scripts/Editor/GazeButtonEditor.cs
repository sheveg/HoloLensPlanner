using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace HoloLensPlanner
{
    [CustomEditor(typeof(GazeButton))]
    public class GazeButtonEditor : GazeResponderEditor
    {
        GazeButton m_GazeButton;
        protected override void Awake()
        {
            base.Awake();
            m_GazeButton = (GazeButton)target;
        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();

            m_GazeButton.SetOutline();
            switch (m_GazeButton.OnFocus)
            {
                case FocusEvent.Outline:
                    m_GazeButton.Outline.hideFlags = HideFlags.HideInInspector;
                    m_GazeButton.EventOutlineThickness = EditorGUILayout.FloatField("Thickness", m_GazeButton.EventOutlineThickness);
                    break;
                default:
                    break;
            }
        }

    }
}


