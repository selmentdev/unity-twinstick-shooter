using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestGame.Player;
using UnityEngine;

namespace TestGame.Equipment
{
    /// <summary>
    /// Medkit equipment.
    /// </summary>
    public class MedkitEquipment : BaseEquipment
    {
        public float HealthPoints;

        public override void Apply(GameObject go)
        {
            var player = go.GetComponent<PlayerCharacter>();

            player.AddHealth(this.HealthPoints);
        }
    }
}
