using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestGame.AI;
using UnityEngine;
using UnityEngine.AI;

namespace TestGame.Bots
{
    public class BotController : AIAgent
    {
        public const float Range1 = 10.0F;
        public const float Range2 = 20.0F;

        //
        // Remarks:
        //
        // This bot uses Utility AI Theory to implement actual bot AI.
        //
        private static AIAction[] Actions = new AIAction[]
        {
            //
            // Move to player:
            //
            // Bot moves to player shoot distance to player.
            //
            new Actions.MoveToPlayer()
            {
                Interval = 0.3F,
                Scorers = new AIScorer[]
                {
                    new Scorers.FixedScorer()
                    {
                        FixedScore = 100.0F,
                    },
                }
            },

            //
            // Melee attack is available when bot doesn't have weapon / ammunition and it's close to player.
            //
            new Actions.MeleeAttack()
            {
                Interval = 0.5F,
                Scorers = new AIScorer[]
                {
                    new Scorers.DistanceToPlayer()
                    {
                        Treshold = Range2,
                        PositiveScore = 100.0F,
                        NegativeScore = -100.0F
                    },
                    new Scorers.HasWeapon()
                    {
                        Expected = false,
                        PositiveScore = 100.0F,
                        NegativeScore = -100.0F
                    },
                }
            },
            //
            // Keep distance and try encircle player.
            //
            new Actions.KeepDistance()
            {
                Interval = 0.2F,
                PathEvaluationLimit = 3,
                Scorers = new AIScorer[]
                {
                    //
                    // Player is visible.
                    //
                    new Scorers.DistanceToPlayer()
                    {
                        Treshold = Range2,
                        PositiveScore = 100.0F,
                        NegativeScore = -100.0F
                    },
                    //
                    // May have weapon.
                    //
                    new Scorers.HasWeapon()
                    {
                        Expected = true,
                        PositiveScore = 300.0F,
                        NegativeScore = -100.0F
                    },
                    //
                    // Hit-and-run mode is promoted.
                    //
                    new Scorers.MeleeHitAndRun()
                    {
                        Treshold = 2.0F,
                        PositiveScore = 500.0F,
                        NegativeScore = 0.0F,
                    }
                }
            },
            new Actions.DistanceAttack()
            {
                //
                // Distance attack requires much quickier reevaluation.
                //
                Interval = 0.05F,
                MinAngle = 10.0F,
                RotationAngle = 540.0F,
                PathEvaluationLimit = 10,
                Scorers  = new AIScorer[]
                {
                    new Scorers.DistanceToPlayer()
                    {
                        Treshold = Range2,
                        PositiveScore = 100.0F,
                        NegativeScore = -100.0F
                    },
                    new Scorers.HasWeapon()
                    {
                        Expected = true,
                        PositiveScore = 300.0F,
                        NegativeScore = -100.0F
                    },
                    new Scorers.TargetInSight()
                    {
                        SightRange = Mathf.Lerp(Range1, Range2, 0.5F),
                        SightAngularSpan = 89.0F,
                        PositiveScore = 100,
                        NegativeScore = -200,
                    }
                }
            }
        };

        public NavMeshAgent NavMeshAgent;

        private BotCharacter m_Character;

        public int PathEvaluationCounter = 0;

        protected override IAIContext ProvideContext()
        {
            return this.m_Character;
        }

        protected override void Start()
        {
            this.NavMeshAgent = this.GetComponent<NavMeshAgent>();
            this.m_Character = this.GetComponent<BotCharacter>();
            this.m_Character.Target = GameObject.FindGameObjectWithTag("Player").transform;

            this.InitializeAgent(BotController.Actions);

            //
            //  BUG:
            //      Agent jumps large distance when spawned.
            //
            //      This solves that problem.
            // 
            //  More:
            //      http://answers.unity3d.com/questions/771908/navmesh-issue-with-spawning-players.html
            //
            this.NavMeshAgent.enabled = true;
            this.NavMeshAgent.speed = this.m_Character.Speed;

            base.Start();
        }
    }
}
