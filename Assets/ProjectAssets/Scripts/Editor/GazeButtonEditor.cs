using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace HoloLensPlanner
{
    /// <summary>
    /// Custom inspector for a <see cref="GazeButton"/>. Adds/Removes an <see cref="UnityEngine.UI.Outline"/> component dependent on the <see cref="GazeResponder.OnFocus"/> variable. 
    /// </summary>
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
                    // hide the outline component in the inspector
                    m_GazeButton.Outline.hideFlags = HideFlags.HideInInspector;
                    m_GazeButton.EventOutlineThickness = EditorGUILayout.FloatField("Thickness", m_GazeButton.EventOutlineThickness);
                    break;
                default:
                    break;
            }
        }

    }
}


