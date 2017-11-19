using UnityEngine;
using UnityEngine.UI;
using HoloToolkit.Unity.InputModule;
using System;
using System.Collections;

namespace HoloLensPlanner
{
    /// <summary>
    /// GazeButton transforms an Unity UI component into an object which responds to the HoloLens gaze. Depending on the <see cref="GazeResponder.OnFocus"/> variable
    /// the UI object reacts with the corresponding event, e.g. it scales up when <see cref="FocusEvent.Scale"/> is selected. 
    /// </summary>
    [RequireComponent(typeof(Graphic))]
    public class GazeButton : GazeResponder
    {
        /// <summary>
        /// Unity UI outline component
        /// </summary>
        public Outline Outline { get; private set; }

        /// <summary>
        /// How thick the outline is.
        /// </summary>
        public float EventOutlineThickness { get { return m_EventOutlineThickness; } set { m_EventOutlineThickness = value; } }

        [SerializeField, HideInInspector]
        private float m_EventOutlineThickness = 2f;

        private void Start()
        {
            switch (OnFocus)
            {
                case FocusEvent.Outline:
                    SetOutline();
                    Outline.enabled = false;
                    break;
                default:
                    break;
            }
        }

        protected override void OnColor()
        {
            base.OnColor();
        }

        protected override void OnOutline()
        {
            Outline.enabled = m_IsFocused;
        }

        protected override void OnScale()
        {
            base.OnScale();
        }

        /// <summary>
        /// If <see cref="GazeResponder.OnFocus"/> is set to <see cref="FocusEvent.Outline"/> it adds an outline component, otherwise it removes it.  
        /// </summary>
        public void SetOutline()
        {
            if (OnFocus == FocusEvent.Outline)
            {
                if ((Outline = GetComponent<Outline>()) == null)
                {
                    Outline = gameObject.AddComponent<Outline>();
                } 
                Outline.effectColor = EventOutlineColor;
                Outline.effectDistance = new Vector2(EventOutlineThickness, -EventOutlineThickness);
            }
            else if (OnFocus != FocusEvent.Outline && Outline != null)
            {
                DestroyImmediate(Outline);
            }
        }

    }
}