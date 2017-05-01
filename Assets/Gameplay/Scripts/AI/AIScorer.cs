using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestGame.AI
{
    /// <summary>
    /// Represents scoring AI object.
    /// </summary>
    public abstract class AIScorer
    {
        /// <summary>
        /// Evaluates score qualifiers.
        /// </summary>
        /// <param name="context">A context.</param>
        /// <returns>Score</returns>
        public abstract float Score(IAIContext context);
    }
}
