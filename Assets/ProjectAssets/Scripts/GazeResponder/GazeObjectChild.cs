using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace HoloLensPlanner.GazeResponse
{
    /// <summary>
    /// GazeObjectChild applies the OnFocus method to all its immediate children but itself.
    /// </summary>
    public class GazeObjectChild : GazeResponder
    {
        private List<GameObject> m_Children = new List<GameObject>();

        /// <summary>
        /// We need to store init scale values of all immediate children for rescaling back when not in focus.
        /// </summary>
        private Dictionary<GameObject, Vector3> m_DefaultScales = new Dictionary<GameObject, Vector3>();

        protected override void OnColor()
        {

        }

        protected override void OnOutline()
        {

        }

        protected override void OnScale()
        {
            foreach (var child in m_Children)
                OnScale(child.transform, m_DefaultScales[child]);
        }

        protected override void Awake()
        {
            getChildren();
            switch (OnFocus)
            {
                case FocusEvent.Color:
                    break;
                case FocusEvent.Outline:
                    break;
                case FocusEvent.Scale:
                    getScalesOfChildren();
                    break;
            }
        }

        private void getChildren()
        {
            var children = GetComponentsInChildren<Transform>();
            foreach (var child in children)
            {
                if (child == transform)
                    continue;
                m_Children.Add(child.gameObject);
            }
        }

        private void getScalesOfChildren()
        {
            foreach (var child in m_Children)
            {
                m_DefaultScales.Add(child.gameObject, child.transform.localScale);
            }
        }
    }
}
