using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity;

namespace HoloLensPlanner
{
    public class GlobalSettings : Singleton<GlobalSettings>
    {
        public TEST.TextureLibrary TextureLibrary;

        public TEST.PathLibrary PathLibrary;

        protected override void Awake()
        {
            base.Awake();
            TextureLibrary.LoadPNGsIntoTextures(PathLibrary.TileTexturesPath);
        }
    }
}



