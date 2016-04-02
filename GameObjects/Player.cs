using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

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
        /// Typically every player will have a unique ID corresponding to a database
        /// entry - for this prototype, it probably won't be used since there won't
        /// be a database
        /// </summary>
        public int Id { get; set; }

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

            return this.Id == other.Id && Equals(other.Type, this.Type);
        }

        /// <summary>
        /// Returns the haschode of this object
        /// </summary>
        public override int GetHashCode()
        {
            int hashcode = this.Id.GetHashCode();
            if (this.Type != null)
                hashcode ^= this.Type.GetHashCode();

            return hashcode;
        }

        /// <summary>
        /// Returns a pretty string representation for this object
        /// </summary>
        public override string ToString()
        {
            StringBuilder bldr = new StringBuilder();
            bldr.AppendFormat("Player: Id: {0}, ", this.Id);
            if (this.Type.IsNullOrEmpty())
                bldr.Append("Type: none");
            else
                bldr.AppendFormat("Type: {0}", this.Type);

            return bldr.ToString();
        }
    }
}
