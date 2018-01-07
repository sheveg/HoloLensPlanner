using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace HoloLensPlanner.TEST
{
    [CustomEditor(typeof(Tile)), CanEditMultipleObjects]
    public class TileEditor : Editor
    {
        private Tile m_Tile;
        private int m_TileTextureIndex;

        private void Awake()
        {
            m_Tile = (Tile)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();
            List<string> textures = new List<string>();
            foreach (var texture in GlobalSettings.Instance.TextureLibrary.Textures)
            {
                textures.Add(texture.name);
            }
            m_TileTextureIndex = EditorGUILayout.Popup("Texture", m_TileTextureIndex, textures.ToArray());
            m_Tile.TextureIndex = m_TileTextureIndex;
        }
    }

}

