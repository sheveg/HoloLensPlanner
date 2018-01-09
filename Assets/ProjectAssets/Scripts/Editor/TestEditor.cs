using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using HoloLensPlanner;
using HoloLensPlanner.TEST;

[CustomEditor(typeof(TilesGenerator))]
public class TestEditor : Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        TilesGenerator gen = (TilesGenerator)target;

        if (GUILayout.Button("test"))
            gen.SpawnTilesOnFloor(TileMenuManager.Instance.SavedTiles[0], gen.Plane, gen.Spawn, gen.Direction);
    }
}
