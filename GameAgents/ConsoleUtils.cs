using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;
using GameObjects;

namespace GameAgents
{
    public class ConsoleUtils
    {
        /// <summary>
        /// Helper that returns the number of the round currently in-play for the given game
        /// </summary>
        public static int GetCurrentRoundNumberFor(Game game)
        {
            if (game == null)
                throw new Exception("Cannot return round for null game");

            if (game.GameRoundsCompleted.IsNullOrEmpty()) { return 1; }

            int gameRound = 1;
            foreach (GameRound round in game.GameRoundsCompleted)
            {
                if (round.AllPlayerTurnsCompleted(game.Players, game.TotalRoundsInThisGame))
                    gameRound++;
            }

            return gameRound;
        }

        /// <summary>
        /// Helper that returns a pretty score table for the given game.  If the optional player
        /// is provided, it is marked with an asterisk (for emphasis) in the resulting output.
        /// </summary>
        public static string GetPrettyScoreTableFor(Game game, Player currentPlayer = null)
        {
            if (game == null || currentPlayer == null) { return "Score unavailable\n"; }//todo - log error here

            StringBuilder bldr = new StringBuilder();
            Dictionary<int, int> scores = game.GetPlayerToScoreMapping();
            bldr.Append("Here's the current game score/standings:\n");
            foreach (KeyValuePair<int, int> tuple in scores)
            {
                bldr.AppendFormat("Player {0}{1}: {2}\n",
                                  tuple.Key,
                                  currentPlayer != null && tuple.Key == currentPlayer.Id ? "* " : "",
                                  tuple.Value);
            }

            return bldr.ToString();
        }

        /// <summary>
        /// Helper that returns a pretty string depicting the dice mapping table
        /// for the given game
        /// </summary>
        public static string GetDiceMappingDescriptionFor(Game game)
        {
            if (game == null) { return "No Dice Mapping Available"; }//todo - log this as an error
            if (game.MapOfDiceValues.IsNullOrEmpty()) { return "Dice mapping values unavailable"; }//todo - log this as an error

            StringBuilder bldr = new StringBuilder();
            bldr.Append("Dice values for this game:\n");
            foreach (KeyValuePair<int, int> tuple in game.MapOfDiceValues)
                bldr.AppendFormat("All {0} die are worth {1} points\n", tuple.Key, tuple.Value);

            return bldr.ToString();
        }
    }
}
