using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HoloToolkit.Unity;

namespace HoloLensPlanner
{
    public class InstructionMenu : SingleInstance<InstructionMenu>
    {
        [SerializeField]
        private Text InstructionText;

        [SerializeField]
        private GameObject FloatingInstructionPrefab;

        public string Instruction {
            get { return InstructionText.text; }
            set { InstructionText.text = value; }
        }

        public void ShowFloatingInstruction(float showTime = 2f)
        {
            //if (MenuHub.Instance.CurrentMenu != gameObject)
            //{
            //    MenuHub.Instance.ShowMenu(gameObject);
            //}
            //StartCoroutine(floatingInstructionCoroutine(showTime));
        }

        private IEnumerator floatingInstructionCoroutine(float showTime)
        {
            var instruction = Instantiate(FloatingInstructionPrefab);
            instruction.GetComponentInChildren<Text>().text = Instruction;
            yield return new WaitForSeconds(showTime);
            Destroy(instruction);
        }
    }
}
