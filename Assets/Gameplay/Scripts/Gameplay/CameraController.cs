using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TestGame.Gameplay
{
    /// <summary>
    /// Sets up camera above & behind target.
    /// </summary>
    public class CameraController : MonoBehaviour
    {
        /// <summary>
        /// A target object.
        /// </summary>
        public Transform Target;

        /// <summary>
        /// A distance behind target.
        /// </summary>
        public float Distance = 10.0F;

        /// <summary>
        /// An angle.
        /// </summary>
        public float Angle = 45.0F;
        
        private void LateUpdate()
        {
            //
            // Move camera at distance by specified versor.
            //
            this.transform.position = this.Target.position + Quaternion.AngleAxis(this.Angle, this.Target.transform.right) * (-this.Target.forward * this.Distance);

            //
            // And make camera to look at target.
            //
            this.transform.LookAt(this.Target);
        }
    }
}
