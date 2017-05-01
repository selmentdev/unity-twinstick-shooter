using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TestGame.Triggers
{
    /// <summary>
    /// Implements sliding door object.
    /// </summary>
    [RequireComponent(typeof(BoxCollider))]
    public class SlidingDoor : MonoBehaviour
    {
        //
        // A wing to open/close.
        //
        public GameObject Wing;

        //
        // Wing state.
        //
        public bool IsOpened;

        //
        // Wing moving speed.
        //
        public float Speed;

        //
        // Initial wing door position.
        //
        private Vector3 m_InitialPosition;
        
        private void Start()
        {
            //
            // Capture initial position.
            //
            this.m_InitialPosition = this.Wing.transform.localPosition;
        }

        private void FixedUpdate()
        {
            //
            // Get target wing extent.
            //
            var leftExtent = this.IsOpened ? this.m_InitialPosition : Vector3.zero;

            //
            // Compute speed.
            //
            var speed = this.Speed * Time.fixedDeltaTime;

            //
            // Capture actual positions.
            //
            var leftPosition = this.Wing.transform.localPosition;

            //
            // Move Xs toward new extents.
            //
            leftPosition = Vector3.MoveTowards(leftPosition, leftExtent, speed);

            //
            // Update wing positions.
            //
            this.Wing.transform.localPosition = leftPosition;
        }

        private void OnTriggerEnter(Collider other)
        {
            //
            // Open doors.
            //
            this.IsOpened = true;
        }

        private void OnTriggerExit(Collider other)
        {
            //
            // Open doors.
            //
            this.IsOpened = false;
        }

        private void OnTriggerStay(Collider other)
        {
            //
            // Open doors.
            //
            this.IsOpened = true;
        }
    }
}
