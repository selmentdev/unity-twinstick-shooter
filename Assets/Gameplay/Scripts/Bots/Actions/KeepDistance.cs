using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestGame.AI;
using UnityEngine;

namespace TestGame.Bots.Actions
{
    /// <summary>
    /// Bot tries to keep distance to player.
    /// </summary>
    public class KeepDistance : AIAction
    {
        public int PathEvaluationLimit = 10;

        public override void Execute(IAIContext context)
        {
            //
            // Extract bot character.
            //
            var bot = context as BotCharacter;

            //
            // Get target position.
            //
            var targetPosition = bot.Target.position;


            if (bot.Controller.PathEvaluationCounter >= PathEvaluationLimit)
            {
                bot.Controller.PathEvaluationCounter = 0;

                //
                // This action tries to attack player with weapon held by bot.
                //
                // Bot tries to perform shot from distance. So, if player gets closer to bot, it runs away without shooting.
                //
                var randomRange = UnityEngine.Random.Range(BotController.Range1 + 1, BotController.Range2 - 1);


                //
                // First, compute direction vector between weapon and target.
                // If bot doesn't have weapon - just it's position.
                //
                var sourcePosition = ((bot.Weapon != null) ? bot.Weapon.Ejector.transform : bot.transform).position;

                Vector3 direction = Vector3.Normalize(sourcePosition - targetPosition);

                //
                // Compute random angle to turn direction.
                //
                var randomAngle = UnityEngine.Random.Range(-15.0F, 15.0F);
                var randomRotation = Quaternion.AngleAxis(randomAngle, Vector3.up);

                //
                // Compute target position.
                //
                var rotatedDirection = randomRotation * direction;

                var moveDirection = rotatedDirection * randomRange;

                var destination = targetPosition + moveDirection;

                //
                // Get navmesh agent.
                //
                var agent = bot.Controller.NavMeshAgent;

                //
                // Resume path at destination.
                //
                agent.SetDestination(destination);
                agent.isStopped = false;

                Debug.DrawLine(bot.transform.position, destination, Color.red);
            }

            ++bot.Controller.PathEvaluationCounter;
        }
    }
}
