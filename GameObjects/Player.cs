using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameObjects
{
    /// <summary>
    /// This class would typically be associated to a 'User' object, but for the purposes
    /// of prototyping it's gonna be mostly barebone/placeholder
    /// </summary>
    [Serializable]
    public class Player
    {
        /// <summary>
        /// Every player has a 'type', like "AI - Easy" or "Human" etc.
        /// At runtime, we can map this 'type' to a game agent that actually
        /// knows how to play our dice game.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Returns true if this object is considered
        /// equal to the given arg, false otherwise (and
        /// false if the given arg isn't a Player type.)
        /// </summary>
        public override bool Equals(object obj)
        {
            Player other = obj as Player;
            if (other == null)
                return false;

            return Equals(other.Type, this.Type);
        }

        /// <summary>
        /// Returns the haschode of this object
        /// </summary>
        public override int GetHashCode()
        {
            int hashcode = 0;
            if (this.Type != null)
                hashcode ^= this.Type.GetHashCode();

            return hashcode;
        }

        /// <summary>
        /// Returns a pretty string representation for this object
        /// </summary>
        public override string ToString()
        {
            string pretty = "Player: ";
            if (this.Type != null)
                pretty += "type='" + this.Type + "'"; // string concatenation, not such a big deal for a few measly strings

            return pretty;
        }
    }
}
