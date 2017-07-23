using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestGame.AI;
using UnityEngine;

namespace TestGame.Bots.Actions
{
    /// <summary>
    /// Idle action. Bot just wanders around map.
    /// </summary>
    public class Idle : AIAction
    {
        public override void Execute(IAIContext context)
        {
            var bot = context as BotCharacter;

            //
            // In idle action we randomly choose direction to go.
            //
            
            var randomAngle = UnityEngine.Random.Range(0.0F, 359.0F);
            var randomDistance = UnityEngine.Random.Range(2.0F, 20.0F);

            var rotation = Quaternion.AngleAxis(randomAngle, Vector3.up);

            //
            // Choose new destination.
            //
            var destination = bot.transform.position + (rotation * bot.transform.forward * randomDistance);

            //
            // Update agent.
            //
            var agent = bot.Controller.NavMeshAgent;
            agent.SetDestination(destination);
            agent.isStopped = false;
        }
    }
}
