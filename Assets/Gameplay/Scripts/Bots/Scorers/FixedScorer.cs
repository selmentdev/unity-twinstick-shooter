using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestGame.AI;

namespace TestGame.Bots.Scorers
{
    /// <summary>
    /// Fixed scorer. Just returns fixed score.
    /// </summary>
    public class FixedScorer : AIScorer
    {
        //
        // Fixed score value.
        //
        public float FixedScore = 0.0F;

        public override float Score(IAIContext context)
        {
            return this.FixedScore;
        }
    }
}
