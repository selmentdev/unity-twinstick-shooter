using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestGame.AI;
using TestGame.Core;
using UnityEngine;

namespace TestGame.Bots.Actions
{
    /// <summary>
    /// Bot tries to perform melee attack on target.
    /// </summary>
    public sealed class MeleeAttack : AIAction
    {
        public override void Execute(IAIContext context)
        {
            //
            // Extract bot character.
            //
            var bot = context as BotCharacter;

            //
            // Get navmesh agent.
            //
            var agent = bot.Controller.NavMeshAgent;
            agent.autoBraking = false;

            var targetPosition = bot.Target.position;
            var botPosition = bot.transform.position;

            //
            // Check if target is in melee distance.
            //
            var distance = Vector3.Distance(targetPosition, botPosition);
            var inRange = distance < bot.MeleeAttackRange;

            //
            // This game gives 'soft-guarantee' that each execute action will execute at this interval...
            //
            // It's not preety at all...
            //
            bot.MeleeAttackTimer += this.Interval;

            //
            // It's not preety, but optimal.
            //
            if (inRange && bot.MeleeAttackTimer >= bot.MeleeAttackInterval)
            {
                //
                // We "hit" character this time. Count it as attack.
                //
                var player = bot.Target.gameObject.GetComponent<CharacterBase>();
                bot.MeleeAttackTimer = -bot.MeleeAttackInterval;

                //
                // Reset hit-and-run.
                //
                bot.HitAndRunTimer = 0.0F;

                player.TakeDamage(bot.MeleeAttack);
            }

            //
            // Get close to target as much as possible.
            //
            agent.SetDestination(targetPosition);
            agent.Resume();
        }
    }
}
