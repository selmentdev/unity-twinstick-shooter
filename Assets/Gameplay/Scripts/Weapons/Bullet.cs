using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestGame.Core;
using UnityEngine;

namespace TestGame.Weapons
{
    /// <summary>
    /// Implements bullet object logic.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(SphereCollider))]
    public class Bullet : MonoBehaviour
    {
        //
        // Damage caused by this bullet object.
        //
        public float Damage;

        //
        // Bullet pushing force.
        //
        public float Force;

        //
        // A rigid body to push.
        //
        private Rigidbody m_RigidBody;

        //
        // A tracer object.
        //
        public GameObject Tracer;

        private void Start()
        {
            //
            // Capture rigid body and push it.
            //
            this.m_RigidBody = this.GetComponent<Rigidbody>();

            this.m_RigidBody.AddRelativeForce(0.0F, 0.0F, this.Force);
        }

        private void OnDestroy()
        {
            //
            // When destroyed, destroy tracer too.
            //
            if (this.Tracer != null)
            {
                this.Tracer.SetActive(false);
                Destroy(this.Tracer);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            var character = collision.gameObject.GetComponent<CharacterBase>();
            if (character != null)
            {
                //
                // Cause damage to character.
                //
                character.TakeDamage(this.Damage);
            }

            Destroy(this.gameObject);
        }
    }
}
