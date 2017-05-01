using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestGame.AI;
using UnityEngine;

namespace TestGame.Bots.Scorers
{
    /// <summary>
    /// Evaluates score based on distance to player.
    /// </summary>
    public class DistanceToPlayer : AIScorer
    {
        /// <summary>
        /// Treshold distance.
        /// </summary>
        public float Treshold;

        /// <summary>
        /// Score for distance below treshold.
        /// </summary>
        public float PositiveScore;

        /// <summary>
        /// Score for distance above reshold.
        /// </summary>
        public float NegativeScore;

        public override float Score(IAIContext context)
        {
            var bot = context as BotCharacter;

            //
            // Just return distance to player. Attack action will kick in eventually.
            //
            var distance = Vector3.Distance(bot.Target.position, bot.transform.position);
            
            //
            // Choose score.
            //
            return (distance <= this.Treshold) ? this.PositiveScore : this.NegativeScore;
        }
    }
}
