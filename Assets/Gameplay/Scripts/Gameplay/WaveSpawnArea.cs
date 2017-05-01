using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.AI;

namespace TestGame.Gameplay
{
    public class WaveSpawnArea : MonoBehaviour
    {
        public float Radius;
        
        public void SpawnEnemy(GameObject go)
        {
            var randomAngle = UnityEngine.Random.Range(0.0F, 359);
            var randomRadius = UnityEngine.Random.Range(0.0F, Radius);

            var arm = this.transform.forward * randomRadius;
            var orientation = Quaternion.AngleAxis(randomAngle, Vector3.up);
            var location = this.transform.position + (orientation * arm);

            var direction = Vector3.down;

            var instance = GameObject.Instantiate(go);
            instance.transform.parent = WaveController.Instance.Bots;
            instance.transform.position = location;
            instance.transform.rotation = Quaternion.AngleAxis(UnityEngine.Random.Range(0.0F, 359.0F), Vector3.up);
        }
    }
}