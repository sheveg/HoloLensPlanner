using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;

public class PreviewNavigator : Singleton<PreviewNavigator> {

    public RectTransform ObjectMenu;
    public RectTransform ObjectPreview;
    public RectTransform PreviewSpawnPoint;

    //TODO: make scale logarithmic
    public float scale;

    private GameObject tilePreview;

    public void Navigate(TileMenuObject tile)
    {
        ObjectMenu.gameObject.SetActive(false);
        ObjectPreview.gameObject.SetActive(true);

        tilePreview = Instantiate(tile.TileMesh, PreviewSpawnPoint.position, Quaternion.identity, PreviewSpawnPoint);
        tilePreview.name = "TilePreview";
        tilePreview = scaleObject(tilePreview);
    }

    public GameObject scaleObject(GameObject gameObject)
    {
        //TODO make sure object doesn't get bigger than view
        gameObject.transform.localScale *= scale;
        return gameObject;
    }

    public void backButton()
    {
        ObjectPreview.gameObject.SetActive(false);
        ObjectMenu.gameObject.SetActive(true);

        GameObject.Destroy(tilePreview);
    }
}
