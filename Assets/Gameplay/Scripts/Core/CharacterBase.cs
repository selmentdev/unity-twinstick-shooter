using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TestGame.Core
{
    /// <summary>
    /// Base character class.
    /// </summary>
    public class CharacterBase : MonoBehaviour
    {
        [Header("Configuration")]
        public float Health;
        public float HealthMax;
        public float SuperHealthMax;

        public float Armor;
        public float ArmorMax;
        public float SuperArmorMax;

        public bool IsAlive = true;

        public virtual void TakeDamage(float damage)
        {
            if (this.IsAlive)
            {
                //
                // Compute damage on armor and health.
                //
                var armorDamage = Mathf.Min(this.Armor, damage);
                var healthDamage = Mathf.Min(this.Health, damage - armorDamage);

                //
                // Update proper states.
                //
                this.Armor -= armorDamage;
                this.Health -= healthDamage;
                
                if (this.Health <= 0.0F)
                {
                    //
                    // Notify that character died. Additionally, latch to not die again.
                    //
                    this.IsAlive = false;
                    this.OnCharacterDied();
                }
            }
        }

        /// <summary>
        /// This method is called when character dies.
        /// </summary>
        protected virtual void OnCharacterDied()
        {
        }

        public void AddHealth(float points)
        {
            this.Health = Mathf.Min(this.Health + points, this.SuperHealthMax);
        }

        public void AddArmor(float points)
        {
            this.Armor = Mathf.Min(this.Armor + points, this.SuperArmorMax);
        }
    }
}
