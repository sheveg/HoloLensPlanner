using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

namespace HoloLensPlanner.TEST
{
    [Serializable]
    public class Tile : MonoBehaviour
    {
        public float Height;

        public float Width;

        public TileThickness TileThickness;

        public JointThickness JointThickness;

        [HideInInspector]
        public int TextureIndex;

        private Texture2D m_Texture;

        public string Name;

        private void Awake()
        {
            //SaveToJson();
            //LoadFromJson(Name);
        }

        public void SaveToJson()
        {
            string json = JsonUtility.ToJson(this, true);
            string filePath = Path.Combine(Application.streamingAssetsPath + "/SavedTiles/", Name + ".json");
            File.WriteAllText(filePath, json);
        }

        public void LoadFromJson(string name)
        {
            string filePath = Path.Combine(Application.streamingAssetsPath + "/SavedTiles/", name + ".json");
            if (File.Exists(filePath))
            {
                string jsonTile = File.ReadAllText(filePath);
                JsonUtility.FromJsonOverwrite(jsonTile, this);
                m_Texture = GlobalSettings.Instance.TextureLibrary.Textures[TextureIndex];
            }
            
        }
    }
}
