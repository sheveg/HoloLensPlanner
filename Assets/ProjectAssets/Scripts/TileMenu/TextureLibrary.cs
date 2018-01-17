using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace HoloLensPlanner
{

    [CreateAssetMenu(menuName = "Libraries/TextureLibrary")]
    public class TextureLibrary : ScriptableObject
    {

        private const int texturePixelSize = 128;

        public Texture2D[] Textures;
        
        public void LoadPNGsJPGsIntoTextures(string path)
        {
            var directory = new DirectoryInfo(path);
            var directoryInfo = directory.GetFiles();

            directoryInfo = directoryInfo.Where(s => s.FullName.EndsWith(".jpg") || s.FullName.EndsWith(".png")).ToArray();

            int i = 0;

            Textures = new Texture2D[directoryInfo.Length];

            foreach (var file in directoryInfo)
            {
                if (file.FullName.EndsWith(".jpg") || file.FullName.EndsWith(".png"))
                {
                    Textures[i] = new Texture2D(texturePixelSize, texturePixelSize);
                    Textures[i].LoadImage(File.ReadAllBytes(file.FullName));
                }
                    i++;              
            }
        }
    }
}


