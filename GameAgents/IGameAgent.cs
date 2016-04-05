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
    /// Implementors of this interface know how to make a move for our dice game
    /// </summary>
    public interface IGameAgent
    {

        /// <summary>
        /// Implementers of this method should know how to make a move for our dice game - in other words,
        /// they should return a list of dice to 'keep' for the given list of available dice to chose from,
        /// for the given player.  The given list of turns and game object are both given to aid with
        /// the construction of artificial intelligence (if necessary.)  They're informational for the most
        /// part.
        /// 
        /// In general, implementors should NOT modify any of the args passed-in.  Additionally: implementors
        /// should always return at least ONE die in the list, in accordance with the rules of our dice game.
        /// </summary>
        List<GameDice> MakeMove(List<GameDice> availableDice, Player playerToMove, List<PlayerTurn> turnsTakenSoFar, Game game);
    }
}
