using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace GameObjects
{
    /// <summary>
    /// GameRound objects model all the information needed to represent a round
    /// in our dice game - which players have taken their turns, which dice did they
    /// have available, which dice did they keep, and who started the round, etc...
    /// everything you'd need to restore the state of a round.
    /// </summary>
    [Serializable]
    public class GameRound
    {
        /// <summary>
        /// Which round is this?  Round 1, round 2?
        /// </summary>
        public int RoundNumber { get; set; }

        /// <summary>
        /// What's the ID of the player that started this round?
        /// </summary>
        public int StartingPlayerId { get; set; }

        /// <summary>
        /// Store all the information for all the turns taken so far in this game round
        /// </summary>
        public List<PlayerTurn> TurnsTakenSoFar { get; set; }

        /// <summary>
        /// In a given game round, a player is considered to have completed their turn if they
        /// have elected to keep a number of dice equal to the total dice per game round.
        /// 
        /// For example: if players are supposed to roll/keep 10 dice per round in a given game,
        /// but player 1 has only kept 2 (or none at all) during this round, then player 1's turn
        /// isn't complete for this round.
        /// 
        /// The following method returns true if all the given players have kept the given
        /// number of total dice-per-round, or false otherwise.  This method will also barf if
        /// there are players present in this game round that are NOT in the given list of players.
        /// That would be a data-discrepancy, and at runtime we should signal it ASAP.
        /// </summary>
        public virtual bool AllPlayerTurnsCompleted(List<Player> players, int diceToRollEachRound)
        {
            // if there are no players given, the only way this round can be considered 'complete'
            // is if no turns have been taken
            if (players.IsNullOrEmpty())
            {
                if (this.TurnsTakenSoFar.IsNullOrEmpty()) { return true; }

                // no players, but turns have been taken - it's a data discrepancy!
                throw new Exception("Data discrepancy encountered - this game round has turns recorded, but no players were provided to determine turn completion - please verify data integrity for this game");
            }

            // if nobody has taken a turn in this round, then the only way the round can
            // be considered 'complete' is if nobody has to roll any dice!
            if (this.TurnsTakenSoFar.IsNullOrEmpty()) { return diceToRollEachRound == 0; }

            // iterate on the turns in this round, and keep track of how many dice each player has kept
            Dictionary<int, int> mapPlayerToKeptDice = new Dictionary<int, int>();
            foreach(PlayerTurn turn in this.TurnsTakenSoFar)
            {
                if (mapPlayerToKeptDice.ContainsKey(turn.PlayerId))
                    mapPlayerToKeptDice[turn.PlayerId] += turn.KeptDice != null ? turn.KeptDice.Count : 0;
                else
                    mapPlayerToKeptDice[turn.PlayerId] = turn.KeptDice != null ? turn.KeptDice.Count : 0;
            }

            // if this game round has a player turn recorded for someone that's not
            // in the given list of players, we should report that - it's a data discrepancy
            foreach(int playerId in mapPlayerToKeptDice.Keys)
            {
                if (!players.Exists(player => player.Id == playerId))
                    throw new Exception(String.Format("Data discrepancy detected - player with id {0} has taken a turn in this game round, but was not included in the list of players to check for turn completion", playerId));
            }

            // now check to see if all players have kept the right number of dice - 
            // if even one hasn't, then return false
            foreach (Player player in players)
            {
                if (!mapPlayerToKeptDice.ContainsKey(player.Id)) { return false; }
                if (mapPlayerToKeptDice[player.Id] != diceToRollEachRound) { return false; }
            }

            // everything checks out - this round contains all the given players (and not more)
            // and each one has kept the right number of dice
            return true;
        }

        /// <summary>
        /// This method will return a mapping of player-id
        /// to score.
        /// </summary>
        public virtual Dictionary<int,int> GetRoundScore()
        {
            Dictionary<int, int> mapPlayerToScore = new Dictionary<int, int>();
            if (this.TurnsTakenSoFar.IsNullOrEmpty()) { return mapPlayerToScore; }

            foreach(PlayerTurn turn in this.TurnsTakenSoFar)
            {
                if (turn.KeptDice.IsNullOrEmpty()) { continue; }
                foreach(GameDice keptDie in turn.KeptDice)
                {
                    if (mapPlayerToScore.ContainsKey(turn.PlayerId))
                        mapPlayerToScore[turn.PlayerId] += keptDie.ActualValue;
                    else
                        mapPlayerToScore[turn.PlayerId] = keptDie.ActualValue;
                }
            }

            return mapPlayerToScore;
        }

        /// <summary>
        /// Returns true if this object is considered
        /// equal to the given arg, false otherwise (and
        /// false if the given arg isn't a GameRound type.)
        /// </summary>
        public override bool Equals(object obj)
        {
            GameRound other = obj as GameRound;
            if (other == null) { return false; }

            if(this.RoundNumber != other.RoundNumber){ return false; }
            if(this.StartingPlayerId != other.StartingPlayerId) { return false; }

            if (this.TurnsTakenSoFar == null && other.TurnsTakenSoFar != null) { return false; }
            if (this.TurnsTakenSoFar != null && other.TurnsTakenSoFar == null) { return false; }
            if (this.TurnsTakenSoFar.Count != other.TurnsTakenSoFar.Count) { return false; }
            for(int i = 0; i < this.TurnsTakenSoFar.Count; i++)
            {
                // order matters for the equality of the turns taken in a game round
                if (!this.TurnsTakenSoFar[i].Equals(other.TurnsTakenSoFar[i]))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Returns hashcode for this object
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            int hashcode = this.RoundNumber.GetHashCode() ^ this.StartingPlayerId.GetHashCode();
            if(!this.TurnsTakenSoFar.IsNullOrEmpty())
            {
                foreach (PlayerTurn turn in this.TurnsTakenSoFar)
                    hashcode ^= turn.GetHashCode();
            }

            return hashcode;
        }

        /// <summary>
        /// Returns a pretty string representation of this object
        /// </summary>
        public override string ToString()
        {
            StringBuilder bldr = new StringBuilder();
            bldr.AppendFormat("GameRound: RoundNumber: {0}, StartingPlayerId: {1}, ", this.RoundNumber, this.StartingPlayerId);

            if (this.TurnsTakenSoFar.IsNullOrEmpty())
                bldr.Append("TurnsTakenSoFar: None");
            else
            {
                bldr.Append("TurnsTakenSoFar: [");
                foreach (PlayerTurn turn in this.TurnsTakenSoFar)
                    bldr.AppendFormat("[{0}], ", turn.ToString());

                bldr.Append("]");
            }
            return bldr.ToString();
        }
    }
}
