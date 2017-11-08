using UnityEngine;
using UnityEngine.UI;
using HoloToolkit.Unity.InputModule;
using System;
using System.Collections;

namespace HoloLensPlanner
{
    [RequireComponent(typeof(Graphic))]
    public class GazeButton : GazeResponder
    {
        public Outline Outline { get; private set; }

        public float EventOutlineThickness { get { return m_EventOutlineThickness; } set { m_EventOutlineThickness = value; } }

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
                Debug.Log("Destroy");
                DestroyImmediate(Outline);
            }
        }

    }
}