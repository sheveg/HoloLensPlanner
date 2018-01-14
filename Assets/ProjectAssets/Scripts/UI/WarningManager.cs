using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity;
using UnityEngine.UI;

namespace HoloLensPlanner
{
    public class WarningManager : Singleton<WarningManager>
    {
        [SerializeField]
        private Text WarningText;

        public void ShowWarning(string warning, float showTime = 2f)
        {
            StartCoroutine(warningCoroutine(warning, showTime));
        }

        private IEnumerator warningCoroutine(string warning, float showTime)
        {
            WarningText.text = warning;
            WarningText.transform.parent.gameObject.SetActive(true);
            yield return new WaitForSeconds(showTime);
            WarningText.transform.parent.gameObject.SetActive(false);
        }
    }

}
