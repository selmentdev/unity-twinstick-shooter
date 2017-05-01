using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TestGame.Weapons
{
    /// <summary>
    /// Implements laser sighting.
    /// </summary>
    public class LaserSighting : MonoBehaviour
    {
        //
        // A line renderer to render laser.
        //
        private LineRenderer m_LineRenderer;

        //
        // Default fallback distance for laser when raycast hits nothing.
        //
        public float FallbackDistance = 40.0F;

        private void Start()
        {
            //
            // Captures and initializes line renderer.
            //
            this.m_LineRenderer = this.GetComponent<LineRenderer>();
            this.m_LineRenderer.numPositions = 2;
        }

        private void Update()
        {
            //
            // Make forward ray.
            //
            var ray = new Ray(transform.position, transform.forward);

            //
            // Start line at ray origin.
            //
            m_LineRenderer.SetPosition(0, ray.origin);

            //
            // Perform raycast.
            //
            RaycastHit hit;

            var distance = this.FallbackDistance;

            if (Physics.Raycast(ray, out hit))
            {
                //
                // We have hit - stop lazer ray at that point.
                //
                distance = Mathf.Min(distance, hit.distance);
            }

            //
            // Set end point.
            //
            m_LineRenderer.SetPosition(1, ray.GetPoint(distance));
        }
    }
}