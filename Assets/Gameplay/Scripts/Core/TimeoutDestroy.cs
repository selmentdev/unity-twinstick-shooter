using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TestGame.Core
{
    /// <summary>
    /// Destroy object after specified timeout.
    /// </summary>
    public class TimeoutDestroy : MonoBehaviour
    {
        public float Timeout = 5.0F;

        private void OnEnable()
        {
            Destroy(this.gameObject, this.Timeout);
        }
    }
}
