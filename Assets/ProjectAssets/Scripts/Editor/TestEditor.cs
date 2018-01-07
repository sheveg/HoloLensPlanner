using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using HoloLensPlanner;

[CustomEditor(typeof(TilesGenerator))]
public class TestEditor : Editor {

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        TilesGenerator gen = (TilesGenerator)target;

        if (GUILayout.Button("test"))
            gen.createTiles(gen.Tile, gen.Plane, gen.SpawnPoint, gen.DirectionPoint);
    }
}
