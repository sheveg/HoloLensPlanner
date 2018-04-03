using UnityEngine;


namespace HoloLensPlanner
{
    public class SymbolKeyboardWithNumbers : MonoBehaviour
    {
        [SerializeField]
        private UnityEngine.UI.Button m_PageBck = null;

        [SerializeField]
        private UnityEngine.UI.Button m_PageFwd = null;

        private void Update()
        {
            // Visual reflection of state.
            m_PageBck.interactable = KeyboardWithNumbers.Instance.IsShifted;
            m_PageFwd.interactable = !KeyboardWithNumbers.Instance.IsShifted;
        }
    }
}
