using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace HoloLensPlanner
{
    public class TextManager : SingleInstance<TextManager>
    {

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (!gameObject.active)
            {
                return;
            }

            var camPos = Camera.main.transform.position + Camera.main.transform.forward;
            var difference = new Vector3(camPos.x + 0.07f, camPos.y + 0.07f, camPos.z);
            gameObject.transform.position = difference;
            gameObject.transform.localScale = Vector3.one * 0.025f;
        }

        public void writeText(string text)
        {
            gameObject.GetComponent<TextMesh>().text = text;
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
