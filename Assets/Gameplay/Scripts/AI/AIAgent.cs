using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TestGame.AI
{
    /// <summary>
    /// Implements AI Agent.
    /// </summary>
    /// <remarks>
    /// This AI implements Utility AI approach. Agent chooses best action based on it's score.
    /// Best action is choosen periodically. Each subsequent action may request it's own interval granulation.
    /// </remarks>
    public abstract class AIAgent : MonoBehaviour
    {
        //
        // Actual evaluation timeout.
        //
        private float m_EvaluationTimeout = 0.0F;

        /// <summary>
        /// Evaluation interval.
        /// </summary>
        public float EvaluationInterval = 5.0F; // in seconds.
        
        //
        // Currently executed action
        //
        private AIAction m_CurrentAction = null;

        //
        // Timeout for current action evaluation.
        //
        private float m_CurrentActionEvaluationTimeout = 0.0F;

        //
        // List of actions for this agent.
        //
        private AIAction[] m_Actions;

        /// <summary>
        /// Gets currently executed actions.
        /// </summary>
        public AIAction CurrentAction
        {
            get
            {
                return this.m_CurrentAction;
            }
        }

        /// <summary>
        /// Initializes agent AI actions.
        /// </summary>
        /// <param name="actions">An array of actions.</param>
        protected void InitializeAgent(AIAction[] actions)
        {
            this.m_Actions = actions;
        }
        
        /// <summary>
        /// Provies context of action evaluation for this agent.
        /// </summary>
        /// <remarks>
        /// Agent controller must override this method in order o retrieve current context.
        /// </remarks>
        /// <returns>
        /// A evaluation context.
        /// </returns>
        protected abstract IAIContext ProvideContext();

        //
        // Choose action with best score.
        //
        private AIAction ChooseBestAction()
        {
            Debug.Assert(this.m_Actions != null);

            if (this.m_Actions != null)
            {
                var context = this.ProvideContext();

                var actionsCount = this.m_Actions.Length;

                //
                // Check if we have already any actions at all.
                //
                if (actionsCount > 0)
                {
                    //
                    // Assume that first action has best score.
                    //
                    var bestAction = this.m_Actions[0];
                    var bestScore = bestAction.Score(context);

                    for (var i = 1; i < this.m_Actions.Length; ++i)
                    {
                        //
                        // Compute score of next possible action.
                        //
                        var currentAction = this.m_Actions[i];
                        var currentScore = currentAction.Score(context);

                        //
                        // Check if that action has better score.
                        //
                        if (currentScore > bestScore)
                        {
                            //
                            // Mark new best action.
                            //
                            bestAction = currentAction;
                            bestScore = currentScore;
                        }
                    }

                    return bestAction;
                }
            }

            return null;
        }

        protected virtual void Start()
        {
            this.m_CurrentAction = this.ChooseBestAction();

            if (this.m_CurrentAction != null)
            {
                //
                // If we have action, enter it on start.
                //
                // Note: possible bug - reusing agents from pool may not enter this method right now.
                //
                this.m_CurrentAction.OnEnter(this.ProvideContext());
            }
        }

        protected virtual void Update()
        {
            //
            // Update evaluation timers.
            //
            this.m_EvaluationTimeout -= Time.deltaTime;
            this.m_CurrentActionEvaluationTimeout -= Time.deltaTime;
            
            if (this.m_EvaluationTimeout <= 0.0F)
            {
                //
                // Try to choose best action.
                //
                this.m_EvaluationTimeout = this.EvaluationInterval;

                var bestAction = this.ChooseBestAction();

                if (bestAction != null)
                {
                    if (this.m_CurrentAction != bestAction)
                    {
                        if(this.m_CurrentAction != null)
                        {
                            //
                            // Notify current action that we are leaving it.
                            //
                            this.m_CurrentAction.OnLeave(this.ProvideContext());
                        }

                        //
                        // Set current action and restart evaluation..
                        //
                        this.m_CurrentAction = bestAction;
                        this.m_CurrentActionEvaluationTimeout = 0.0F;

                        //
                        // Notify action that it's executing.
                        //
                        this.m_CurrentAction.OnEnter(this.ProvideContext());
                    }
                }

            }

            //
            // Check if we can execute current action.
            //
            if (this.m_CurrentAction != null && this.m_CurrentActionEvaluationTimeout <= 0.0F)
            {
                this.m_CurrentActionEvaluationTimeout = this.m_CurrentAction.Interval;

                this.m_CurrentAction.Execute(this.ProvideContext());
            }
        }
    }
}
