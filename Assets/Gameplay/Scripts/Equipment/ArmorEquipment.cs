using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestGame.Player;
using UnityEngine;

namespace TestGame.Equipment
{
    /// <summary>
    /// Armor equipment. Adds armor to player.
    /// </summary>
    public class ArmorEquipment : BaseEquipment
    {
        public float ArmorPoints;

        public override void Apply(GameObject go)
        {
            var player = go.GetComponent<PlayerCharacter>();

            player.AddArmor(this.ArmorPoints);
        }
    }
}
