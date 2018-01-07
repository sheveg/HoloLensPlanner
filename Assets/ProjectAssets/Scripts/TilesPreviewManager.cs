using HoloLensPlanner;
using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilesPreviewManager : Singleton<TilesPreviewManager> {

    public Tile[] tiles;

    private MyEnum status;

    public SingleView singleView;

    public ArrayView arrayView;

    private static int pageNumber;

    private enum MyEnum
    {
        SingleView,
        ArrayView
    }

	// Use this for initialization
	void Start () {
        //TODO load Tiles from memory
        status = MyEnum.ArrayView;
        //TODO load last used page
        pageNumber = 0;
        setArrayTilePage();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void setArrayTilePage()
    {
        Tile[] page = new Tile[6];
        if (tiles.Length > 5)
        {
            for (int i = 0; i < page.Length; i++)
            {
                page[i] = tiles[pageNumber * 6 + i];
            }
        }
        else
        {
            for (int i = 0; i < tiles.Length; i++)
            {
                page[i] = tiles[i];
            }
        }

        arrayView.SetPage(page);
    }

    //TODO
    private Tile[] loadTilesFromMemory()
    {
        return null;
    }

    public void Navigate(int i)
    {
        Debug.Log(i);
        singleView.Tile = tiles[pageNumber * 6 + i];

        arrayView.Activate(false);
        singleView.Activate(true);

        status = MyEnum.SingleView;
    }
}
