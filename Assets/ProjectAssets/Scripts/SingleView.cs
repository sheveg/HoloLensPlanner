using HoloLensPlanner;
using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleView : Singleton<SingleView> {

    public Tile Tile { get; set; }

    public RectTransform spawnPoint;

    public float scale;

    private GameObject tilePreview;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Activate(bool active)
    {
        this.gameObject.SetActive(active);

        if (active == true)
        {
            tilePreview = Instantiate(Tile.gameObject, spawnPoint.position, Quaternion.identity, spawnPoint);
            tilePreview = scaleObject(tilePreview);
        }
        else
        {
            Destroy(tilePreview);
        }
    }

    private GameObject scaleObject(GameObject gameObject)
    {
        gameObject.transform.localScale *= scale;
        return gameObject;
    }
}
