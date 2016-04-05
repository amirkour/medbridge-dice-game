using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameObjects
{
    /// <summary>
    /// GameDice objects help model the mapping of a die's face value to
    /// it's actual game-time value.  For instance: sometimes a game is setup
    /// such that 4's are wild - a GameDice could model that information at
    /// runtime.
    /// </summary>
    [Serializable]
    public class GameDice
    {
        /// <summary>
        /// Every die has a face value - the number on it's face!
        /// </summary>
        public int FaceValue { get; set; }

        /// <summary>
        /// Every die has an ACTUAL value - ie: 4s are wild, or worth 12, or whatever
        /// the game setup dictates
        /// </summary>
        public int ActualValue { get; set; }

        /// <summary>
        /// Returns true if this object is considered
        /// equal to the given arg, false otherwise (and
        /// false if the given arg isn't a GameDice type.)
        /// </summary>
        public override bool Equals(object obj)
        {
            GameDice other = obj as GameDice;
            if (other == null)
                return false;

            return other.FaceValue == this.FaceValue &&
                other.ActualValue == this.ActualValue;
        }

        /// <summary>
        /// Return hashcode for this object
        /// </summary>
        public override int GetHashCode()
        {
            return this.FaceValue.GetHashCode() ^ this.ActualValue.GetHashCode();
        }

        /// <summary>
        /// Returns a pretty string representation for this object
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "GameDice: FaceValue = " + this.FaceValue + ", ActualValue = " + this.ActualValue;
        }
    }
}
