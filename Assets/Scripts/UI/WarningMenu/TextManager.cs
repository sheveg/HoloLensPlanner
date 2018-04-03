using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity;
using UnityEngine.UI;

namespace HoloLensPlanner
{
    public class TextManager : SingleInstance<TextManager>
    {
        [SerializeField]
        private Text WarningText;

        [SerializeField]
        private Text TutorialText;

        public void ShowWarning(string warning, float showTime = 2f)
        {
            StartCoroutine(warningCoroutine(warning, showTime));
        }

        public void ShowTutorial(string tutorialText)
        {
            TutorialText.text = tutorialText;
            TutorialText.transform.parent.gameObject.SetActive(true);
        }

        public void ShowTutorial(string tutorialText, float showTime = 2f)
        {
            StartCoroutine(tutorialCoroutine(tutorialText, showTime));
        }

        public void HideTutorial()
        {
            TutorialText.transform.parent.gameObject.SetActive(false);
        }

        private IEnumerator warningCoroutine(string warning, float showTime)
        {
            WarningText.text = warning;
            WarningText.transform.parent.gameObject.SetActive(true);
            yield return new WaitForSeconds(showTime);
            WarningText.transform.parent.gameObject.SetActive(false);
        }

        private IEnumerator tutorialCoroutine(string tutorialText, float showTime)
        {
            TutorialText.text = tutorialText;
            TutorialText.transform.parent.gameObject.SetActive(true);
            yield return new WaitForSeconds(showTime);
            TutorialText.transform.parent.gameObject.SetActive(false);
        }
    }

}
