using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestGame.Player;
using UnityEngine;

namespace TestGame.Equipment
{
    /// <summary>
    /// Weapon equipment.
    /// </summary>
    public class WeaponEquipment : BaseEquipment
    {
        public GameObject WeaponPrefab;

        public override void Apply(GameObject go)
        {
            var player = go.GetComponent<PlayerCharacter>();

            player.Controller.TakeWeapon(this.WeaponPrefab.GetComponent<Weapons.Weapon>());
        }
    }
}
