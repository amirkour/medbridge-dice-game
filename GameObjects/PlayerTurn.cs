using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace GameObjects
{
    /// <summary>
    /// PlayerTurn objects are a capture/snapshot of a player's turn - it's relative
    /// position in a round, the dice the player had available at that turn, the dice
    /// they kept, etc.  Everything you'd need to know to recreate a game turn at any
    /// point in the game (which is important for stateless games.)
    /// </summary>
    [Serializable]
    public class PlayerTurn
    {
        /// <summary>
        /// Which player took this turn?
        /// </summary>
        public int PlayerId { get; set; }

        /// <summary>
        /// The relative number of this turn in a game round
        /// </summary>
        public int TurnNumber { get; set; }

        /// <summary>
        /// The dice that were/are available for selection to a player for this turn
        /// </summary>
        public List<GameDice> AvailableDice { get; set; }

        /// <summary>
        /// The dice that a player kept for this turn
        /// </summary>
        public List<GameDice> KeptDice { get; set; }

        /// <summary>
        /// Returns true if this object is considered
        /// equal to the given arg, false otherwise (and
        /// false if the given arg isn't a PlayerTurn type.)
        /// </summary>
        public override bool Equals(object obj)
        {
            PlayerTurn other = obj as PlayerTurn;
            if (other == null)
                return false;

            if (this.PlayerId != other.PlayerId)
                return false;

            if (this.TurnNumber != other.TurnNumber)
                return false;

            if (this.AvailableDice == null && other.AvailableDice != null) { return false; }
            if (this.AvailableDice != null && other.AvailableDice == null) { return false; }
            foreach(GameDice die in this.AvailableDice)
            {
                if (!other.AvailableDice.Contains(die))
                    return false;
            }

            if (this.KeptDice == null && other.KeptDice != null) { return false; }
            if (this.KeptDice != null && other.KeptDice == null) { return false; }
            foreach(GameDice die in this.KeptDice)
            {
                if (!other.KeptDice.Contains(die))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Returns hashcode for this object
        /// </summary>
        public override int GetHashCode()
        {
            int hashcode = this.PlayerId.GetHashCode() ^ this.TurnNumber.GetHashCode();
            if(this.AvailableDice != null)
            {
                foreach (GameDice die in this.AvailableDice)
                    hashcode ^= die.GetHashCode();
            }
            if (this.KeptDice != null)
            {
                foreach (GameDice die in this.KeptDice)
                    hashcode ^= die.GetHashCode();
            }

            return hashcode;
        }

        /// <summary>
        /// Returns a pretty string representation of this object
        /// </summary>
        public override string ToString()
        {
            StringBuilder bldr = new StringBuilder();
            bldr.AppendFormat("PlayerTurn: PlayerId: {0}, TurnNumber: {1}, ", this.PlayerId, this.TurnNumber);
            if (this.AvailableDice.IsNullOrEmpty())
                bldr.Append("AvailableDice: None, ");
            else
            {
                bldr.Append("AvailableDice: [");
                foreach (GameDice die in this.AvailableDice)
                    bldr.AppendFormat("[{0}],", die.ToString());

                bldr.Append("], ");
            }

            if (this.KeptDice.IsNullOrEmpty())
                bldr.Append("KeptDice: None, ");
            else
            {
                bldr.Append("KeptDice: [");
                foreach (GameDice die in this.KeptDice)
                    bldr.AppendFormat("[{0}],", die.ToString());

                bldr.Append("], ");
            }

            return bldr.ToString();
        }
    }
}
