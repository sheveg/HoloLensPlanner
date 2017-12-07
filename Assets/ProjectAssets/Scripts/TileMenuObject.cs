using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class TileMenuObject : MonoBehaviour {

    public GameObject TileMesh;


	// Use this for initialization
	void Start () {
        GetComponent<Button>().onClick.AddListener(() => PreviewNavigator.Instance.Navigate(this));
	}
}
