using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestGame.AI;

namespace TestGame.Bots.Scorers
{
    /// <summary>
    /// This scorer determines whether bot has weapon.
    /// </summary>
    public sealed class HasWeapon : AIScorer
    {
        /// <summary>
        /// Expected weapon state.
        /// </summary>
        public bool Expected;

        /// <summary>
        /// Value to return when weapon is in expected state.
        /// </summary>
        public float PositiveScore;

        /// <summary>
        /// Value to return when weapon is not in expected state.
        /// </summary>
        public float NegativeScore;

        public override float Score(IAIContext context)
        {
            //
            // Extract Bot character data.
            //
            var bot = context as BotCharacter;

            //
            // Check if bot weapon state has expected value.
            //
            var actual = bot.Weapon != null;

            //
            // We promote actions that has expected state and penaltize those with different one.
            //
            return (actual == this.Expected) ? this.PositiveScore : this.NegativeScore;
        }
    }
}
