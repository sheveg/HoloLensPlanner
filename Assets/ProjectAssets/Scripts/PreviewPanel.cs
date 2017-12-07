using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewPanel : MonoBehaviour {

    public RectTransform selector;
    public GameObject[] tiles;

	// Use this for initialization
	void Start () {
        tiles = new GameObject[selector.transform.childCount];
        for(int i = 0; i < selector.transform.childCount; i++)
        {
            tiles[i] = selector.transform.GetChild(i).transform.GetChild(0).gameObject;
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
