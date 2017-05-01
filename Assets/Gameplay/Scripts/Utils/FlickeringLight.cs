using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TestGame.Utils
{
    [RequireComponent(typeof(Light))]
    public class FlickeringLight : MonoBehaviour
    {
        public float Cutoff = 0.9F;
        private Light m_Light;

        private void Start()
        {
            this.m_Light = GetComponent<Light>();
        }

        private void Update()
        {
            if(UnityEngine.Random.value > this.Cutoff)
            {
                m_Light.enabled = !m_Light.enabled;
            }
        }
    }
}
