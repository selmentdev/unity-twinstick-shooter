using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestGame.Gameplay;
using UnityEditor;
using UnityEngine;

namespace TestGame.Assets
{
    [CustomEditor(typeof(WaveSpawnArea))]
    public class WaveSpawnAreaEditor : Editor
    {
        private void OnSceneGUI()
        {
            var area = this.target as WaveSpawnArea;

            if (area == null)
            {
                return;
            }

            //Handles.CylinderCap(0, area.transform.position, Quaternion.identity, area.Radius);
            Handles.CircleCap(0, area.transform.position, Quaternion.AngleAxis(90.0F, Vector3.right), area.Radius);
        }
    }
}
