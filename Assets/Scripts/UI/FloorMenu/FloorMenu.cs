using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity;

namespace HoloLensPlanner
{
    public class FloorMenu : SingleInstance<FloorMenu>
    {
        private void scanRoom()
        {

        }

        private void createFloor()
        {
            MenuHub.Instance.Pin();
            RoomManager.Instance.CreateFloorPlane();
        }

        private void editFloor()
        {

        }
    }
}


