using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestGame.AI;
using UnityEngine;

namespace TestGame.Bots.Scorers
{
    /// <summary>
    /// This scorer evaluates whether agent sees its target.
    /// </summary>
    public class TargetInSight : AIScorer
    {
        /// <summary>
        /// Maximum sight range.
        /// </summary>
        public float SightRange;

        /// <summary>
        /// Field of view.
        /// </summary>
        public float SightAngularSpan;

        /// <summary>
        /// Positive score.
        /// </summary>
        public float PositiveScore;

        /// <summary>
        /// Negative score.
        /// </summary>
        public float NegativeScore;

        public override float Score(IAIContext context)
        {
            //
            // Extract bot.
            //
            var bot = context as BotCharacter;

            //
            // Compute direction to target.
            //
            var direction = Vector3.Normalize(bot.Target.transform.position - bot.transform.position);

            //
            // Create target to target.
            //
            var ray = new Ray(bot.transform.position, direction);

            RaycastHit result;
            if (Physics.Raycast(ray, out result, this.SightRange))
            {
                //
                // Ray hits something.
                //
                if (result.transform.GetInstanceID() == bot.Target.GetInstanceID())
                {
                    //
                    // Ray hit player. Check angle.
                    //

                    var angle = Mathf.Abs(Vector3.Angle(bot.transform.forward, ray.direction));

                    if (angle <= this.SightAngularSpan)
                    {
                        //
                        // Player is visible in specified angle. Go ahead.
                        //
                        return this.PositiveScore;
                    }
                }
            }

            return this.NegativeScore;
        }
    }
}
