using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TestGame.Equipment
{
    /// <summary>
    /// Base class for all equipments on scene.
    /// </summary>
    /// <remarks>
    /// Each equipment knows how to apply it's content to player.
    /// </remarks>
    public abstract class BaseEquipment : MonoBehaviour
    {
        /// <summary>
        /// Applies equipment to game object.
        /// </summary>
        /// <param name="go">A game object.</param>
        public abstract void Apply(GameObject go);
    }
}
