using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestGame.AI;
using UnityEngine;

namespace TestGame.Bots.Actions
{
    /// <summary>
    /// Moves bot to player.
    /// </summary>
    public class MoveToPlayer : AIAction
    {
        public override void Execute(IAIContext context)
        {
            var bot = context as BotCharacter;

            //
            // Capture few variables.
            //
            var agent = bot.Controller.NavMeshAgent;
            var botPosition = bot.transform.position;
            var targetPosition = bot.Target.transform.position;
            
            //
            // Reset agent.
            //
            agent.SetDestination(targetPosition);
            agent.autoBraking = true;
            agent.isStopped = false;
        }
    }
}
