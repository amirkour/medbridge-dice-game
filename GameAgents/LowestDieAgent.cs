using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;
using GameObjects;

namespace GameAgents
{
    /// <summary>
    /// This is a simple reflex agent for our dice game - it'll pick the lowest-valued dice
    /// every single time it's asked to make a move.
    /// </summary>
    public class LowestDieAgent : IGameAgent
    {
        /// <summary>
        /// Reflex-implementation: will pick the lowest valued die every time
        /// </summary>
        public virtual List<GameDice> MakeMove(List<GameDice> availableDice, 
                                               Player playerToMove, 
                                               List<PlayerTurn> turnsTakenSoFar, 
                                               Game game)
        {
            if (availableDice.IsNullOrEmpty()) { return null; }
            GameDice lowestDie = availableDice[0];
            for(int i = 1; i < availableDice.Count; i++)
            {
                if (availableDice[i].ActualValue < lowestDie.ActualValue)
                    lowestDie = availableDice[i];
            }

            List<GameDice> dieTokeep = new List<GameDice>();
            dieTokeep.Add(lowestDie);
            return dieTokeep;
        }
    }
}
