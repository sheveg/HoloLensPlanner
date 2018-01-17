using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity;

namespace HoloLensPlanner
{
    public class GlobalSettings : Singleton<GlobalSettings>
    {
        public TextureLibrary TextureLibrary;

        public PathLibrary PathLibrary;

        protected override void Awake()
        {
            base.Awake();
            TextureLibrary.LoadPNGsJPGsIntoTextures(PathLibrary.TileTexturesPath);
            DontDestroyOnLoad(this);
        }
    }
}



