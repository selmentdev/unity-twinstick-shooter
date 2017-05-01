using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestGame.AI;
using UnityEngine;

namespace TestGame.Bots.Actions
{
    /// <summary>
    /// Distance attack. Bot tries to maintain distance and uses weapon to shoot down target.
    /// </summary>
    public sealed class DistanceAttack : KeepDistance
    {
        //
        // Min angle to shoot.
        //
        public float MinAngle = 10.0F;

        //
        // Max rotation angle.
        //
        public float RotationAngle = 90.0F;

        public override void OnEnter(IAIContext context)
        {
            base.OnEnter(context);

            var bot = context as BotCharacter;

            //
            // Bot updates rotation.
            //
            bot.Controller.NavMeshAgent.updateRotation = false;

            //
            // Reset path evaluation counter. Bot will reevalute path
            //
            bot.Controller.PathEvaluationCounter = 0;
        }

        public override void OnLeave(IAIContext context)
        {
            base.OnLeave(context);

            var bot = context as BotCharacter;

            //
            // Leave rotation update to navmesh agent.
            //
            bot.Controller.NavMeshAgent.updateRotation = true;
        }

        public override void Execute(IAIContext context)
        {
            base.Execute(context);

            //
            // Extract bot character.
            //
            var bot = context as BotCharacter;

            //
            // Get target position.
            //
            var targetPosition = bot.Target.position;

            //
            //
            //
            var lookDirection = Vector3.Normalize(targetPosition - bot.transform.position);

            //
            // Zero out UP direction.
            //
            lookDirection.y = 0.0F;

            
            //
            // Rotate bot towards target.
            //
            var lookAtTarget = Quaternion.LookRotation(lookDirection);
            bot.transform.rotation = Quaternion.RotateTowards(bot.transform.rotation, lookAtTarget, this.RotationAngle * this.Interval);

            //
            // Compute angle between target and forward.
            //
            var angleBetween = Vector3.Angle(lookDirection, bot.transform.forward);
            
            if (angleBetween <= this.MinAngle)
            {
                //
                // Shoot if target is in range.
                //
                bot.Shoot();
            }
            
            Debug.DrawRay(bot.transform.position, bot.transform.forward * 10, Color.green);
        }
    }
}
