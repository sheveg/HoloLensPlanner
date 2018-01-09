using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace HoloLensPlanner.GazeResponse
{
    /// <summary>
    /// Custom inspector for a <see cref="GazeUIObject"/>. Adds/Removes an <see cref="UnityEngine.UI.Outline"/> component dependent on the <see cref="GazeResponder.OnFocus"/> variable. 
    /// </summary>
    [CustomEditor(typeof(GazeUIObject)), CanEditMultipleObjects]
    public class GazeButtonEditor : GazeResponderEditor
    {
        GazeUIObject m_GazeButton;
        protected override void Awake()
        {
            base.Awake();
            m_GazeButton = (GazeUIObject)target;
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


