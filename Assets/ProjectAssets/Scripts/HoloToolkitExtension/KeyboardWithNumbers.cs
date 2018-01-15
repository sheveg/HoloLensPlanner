using System;
using UnityEngine;
using UnityEngine.UI;
using HoloToolkit.Unity;
using HoloToolkit.UI.Keyboard;

#if UNITY_WSA || UNITY_STANDALONE_WIN
using UnityEngine.Windows.Speech;
#endif


namespace HoloLensPlanner
{
    /// <summary>
    /// We extend the keyboard of the toolkit with a numerical keyboard.
    /// </summary>
    public class KeyboardWithNumbers : Keyboard
    {
        /// <summary>
        /// The panel that contains the number keys.
        /// </summary>
        public Image NumberKeyboard = null;

        /// <summary>
        /// We need a reference to the background so we can scale it, when the keyboard type is set to numbers or from numbers to another type to scale it back again.
        /// </summary>
        public Image Background = null;

        /// <summary>
        /// Cached ref for performance.
        /// </summary>
        private RectTransform m_BackgroundRectTransform = null;

        /// <summary>
        /// Background image width of the numbers keyboard panel.
        /// </summary>
        private float m_NumberBackgroundWidth = -1f;

        /// <summary>
        /// General background with of the whole keyboard.
        /// </summary>
        private float m_BackgroundWidth = -1f;

        /// <summary>
        /// Activates a specific keyboard layout, and any sub keys.
        /// </summary>
        /// <param name="keyboardType"></param>
        protected override void ActivateSpecificKeyboard(LayoutType keyboardType) // // change 15.01.18 by Egor, to extend the keyboard functionality to be able to show a number keyboard in a derived class
        {
            DisableAllKeyboards();
            ResetKeyboardState();

            switch (keyboardType)
            {
                case LayoutType.URL:
                    {
                        scaleBackgroundToOriginalSize();
                        ShowAlphaKeyboard();
                        TryToShowURLSubkeys();
                        break;
                    }

                case LayoutType.Email:
                    {
                        scaleBackgroundToOriginalSize();
                        ShowAlphaKeyboard();
                        TryToShowEmailSubkeys();
                        break;
                    }

                case LayoutType.Symbol:
                    {
                        scaleBackgroundToOriginalSize();
                        ShowSymbolKeyboard();
                        break;
                    }
                case LayoutType.Number:
                    {
                        ShowNumberKeyboard();
                        break;
                    }

                case LayoutType.Alpha:
                default:
                    {
                        scaleBackgroundToOriginalSize();
                        ShowAlphaKeyboard();
                        TryToShowAlphaSubkeys();
                        break;
                    }
            }
        }

        protected override void DisableAllKeyboards()
        {
            base.DisableAllKeyboards();
            NumberKeyboard.gameObject.SetActive(false);
        }

        public void ShowNumberKeyboard()
        {
            checkParameters();

            m_BackgroundRectTransform.sizeDelta = new Vector2(m_NumberBackgroundWidth, m_BackgroundRectTransform.sizeDelta.y);

            NumberKeyboard.gameObject.SetActive(true);
            m_LastKeyboardLayout = LayoutType.Number;
        }

        private void scaleBackgroundToOriginalSize()
        {
            checkParameters();

            if (m_LastKeyboardLayout == LayoutType.Number)
            {
                m_BackgroundRectTransform.sizeDelta = new Vector2(m_BackgroundWidth, m_BackgroundRectTransform.sizeDelta.y);
            }
        }

        private void checkParameters()
        {
            // width is not assigned yet
            if (m_NumberBackgroundWidth < 0f)
                m_NumberBackgroundWidth = NumberKeyboard.GetComponent<RectTransform>().sizeDelta.x;
            // rect transform of background is not assigned yet
            if (m_BackgroundRectTransform == null)
                m_BackgroundRectTransform = Background.GetComponent<RectTransform>();
            if (m_BackgroundWidth < 0f)
                m_BackgroundWidth = m_BackgroundRectTransform.sizeDelta.x;
        }
    }
}


