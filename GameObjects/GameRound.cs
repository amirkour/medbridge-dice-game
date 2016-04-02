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

        public bool AllPlayerTurnsCompleted(List<Player> players, int diceToRollEachRound)
        {
            throw new NotImplementedException("I'm not implemented!?");
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
