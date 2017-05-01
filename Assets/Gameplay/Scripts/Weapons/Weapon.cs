using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TestGame.Weapons
{
    //
    // Considered weapons:
    //
    //                   | clip      | ammo      | dmg       | firerate      | reload    
    //  1. Pistol        | 11/15     | inf       | 15        | 120/m         | 2.0s
    //  2. SMG           | 30        | 4+1       | 25        | 850/m         | 1.5s
    //  3. Assault Rifle | 30        | 4+1       | 35        | 500/m         | 3.0s
    //  4. Sniper Rifle  | 6         | 4+1       | 90-150    | 30/m          | 4.5s
    //

    //
    // Weapon types.
    //
    public enum WeaponType
    {
        Pistol,
        Submachine,
        Assault,
        Sniper,
    }

    public enum WeaponState
    {
        Normal,
        Reloading,
        Empty,
    }

    /// <summary>
    /// Implementes weapon logic.
    /// </summary>
    public class Weapon : MonoBehaviour
    {
        [Header("Weapon Stats")]
        public int MaxClipAmmo;
        public int CurrentClipAmmo;
        public int MaxBagAmmo;
        public int CurrentBagAmmo;
        public WeaponType WeaponType;

        public bool InfiniteAmmo;
        public float Spread;
        public float Damage;
        public float FireInterval;
        public float ReloadTime;

        [Header("Weapon Avatar")]
        public Sprite AvatarImage;

        [Header("Prefabs")]
        public ParticleSystem MuzzleFlash;
        public GameObject BulletTracer;
        public Bullet Bullet;

        [Header("Sockets")]
        public Transform Ejector;

        public void SpawnAmmo(float spread)
        {
            //
            // Spawn bullet and tracer
            //
            {
                var projectile = Instantiate(this.Bullet, this.Ejector.transform.position, this.Ejector.transform.rotation);
                var bullet = projectile.GetComponent<Bullet>();

                var randomSpread = UnityEngine.Random.Range(-spread, spread);

                var debugbefore = projectile.transform.forward.normalized;
                
                projectile.transform.RotateAround(projectile.transform.position, Vector3.up, randomSpread);

                //
                // Initialize bullet.
                //
                bullet.Force = 1000.0F;
                bullet.Damage = this.Damage;

                //
                // And spawn tracer.
                //
                var tracer = Instantiate(this.BulletTracer, this.Ejector.transform.position, this.Ejector.transform.rotation);
                tracer.transform.RotateAround(projectile.transform.position, Vector3.up, randomSpread);
                bullet.Tracer = tracer;
            }

            //
            // Make some effects
            //
            if (this.MuzzleFlash != null)
            {
                GameObject.Instantiate(this.MuzzleFlash, this.Ejector.transform.position, this.Ejector.transform.rotation);
            }
        }

        //
        // Weapon state.
        //
        private WeaponState m_State;

        //
        // Inter-shoot timeout timer.
        //
        private float m_Timer;

        public bool IsReloading
        {
            get
            {
                return this.m_State == WeaponState.Reloading;
            }
        }

        private void OnEnable()
        {
            this.m_Timer = 0.0F;

            //
            // To be sure, recopmute state.
            //
            this.m_State =
                (this.CurrentClipAmmo > 0 || this.CurrentBagAmmo > 0)
                    ? WeaponState.Normal
                    : WeaponState.Empty;
        }

        private void Update()
        {
            this.m_Timer += Time.deltaTime;

            if (this.m_State == WeaponState.Reloading)
            {
                if (this.m_Timer > this.ReloadTime)
                {
                    this.m_Timer = 0.0F;
                    this.m_State = WeaponState.Normal;
                }
            }
        }

        public void Reload()
        {
            if (this.m_State == WeaponState.Normal)
            {
                if (this.InfiniteAmmo)
                {
                    //
                    // Just refuel bag.
                    //
                    this.CurrentBagAmmo = this.MaxBagAmmo;
                }
                else if (this.CurrentClipAmmo == this.MaxClipAmmo)
                {
                    //
                    // No reload when clip is full.
                    //
                    return;
                }

                //
                // Mark it as reloading.
                //
                this.m_State = WeaponState.Reloading;
                this.m_Timer = 0.0F;

                //
                // Do ammo reload.
                //
                var reload = Mathf.Min(this.CurrentBagAmmo, this.MaxClipAmmo - this.CurrentClipAmmo);
                this.CurrentClipAmmo += reload;
                this.CurrentBagAmmo -= reload;
                
                if (this.CurrentBagAmmo == 0 && this.CurrentClipAmmo == 0)
                {
                    //
                    // Mark weapon as empty.
                    //
                    this.m_State = WeaponState.Empty;
                }
            }
        }

        public void Shoot()
        {
            if (this.m_State == WeaponState.Normal && this.m_Timer >= this.FireInterval)
            {
                //
                // Weapon can shoot when it's in normal state (not emtpy or not reloading) and some time passed since last shoot.
                //
                this.m_Timer = 0.0F;

                //
                // Update clip ammo.
                //
                --this.CurrentClipAmmo;

                //
                // Spawn bullet.
                //
                this.SpawnAmmo(this.Spread);

                if (this.CurrentClipAmmo <= 0)
                {
                    //
                    // Clip empty. Reload.
                    //
                    this.Reload();
                }
            }
        }

        public void GrabAmmo(Weapon weapon)
        {
            //
            // It's better to reload before grabbing ammo :)
            //
            var totalAmmo = weapon.CurrentBagAmmo + weapon.CurrentClipAmmo;

            this.CurrentBagAmmo = Mathf.Min(this.CurrentBagAmmo + totalAmmo, this.MaxBagAmmo);

            this.CurrentClipAmmo = this.MaxClipAmmo;

            this.m_State = WeaponState.Normal;
        }
    }
}