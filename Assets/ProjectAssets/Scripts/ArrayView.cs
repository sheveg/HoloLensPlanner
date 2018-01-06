using HoloLensPlanner;
using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArrayView : Singleton<ArrayView> {

    private Tile[] page;
    public RectTransform[] spawnPoints;
    private GameObject[] pageObjects;

	// Use this for initialization
	void Start () {
        page = new Tile[6];
        pageObjects = new GameObject[6];

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            spawnPoints[i].gameObject.GetComponent<Button>().onClick.AddListener(() => TilesPreviewManager.Instance.Navigate(i));
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetPage(Tile[] newPage)
    {
        if (newPage.Length <= 6)
        {
            page = newPage;
            InstantiatePage();
        }
    }

    private void InstantiatePage()
    {
        for (int i = 0; i < page.Length; i++)
        {
            //TODO probably include Image as Parent
            if (page[i] != null)
            {
                pageObjects[i] = Instantiate(page[i].gameObject, spawnPoints[i].position, Quaternion.identity, spawnPoints[i]);
                pageObjects[i] = rotateObject(pageObjects[i]);
            } 
        }
    }

    public void Activate(bool active)
    {
        this.gameObject.SetActive(active);
    }

    public GameObject rotateObject(GameObject gameObject)
    {
        gameObject.transform.localEulerAngles = new Vector3(20, -20, 0);
        return gameObject;
    }

}
