using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace GameObjects
{
    /// <summary>
    /// The Game object contains everything we need to know to save and re-instate the
    /// state of a game at any given point in time - essential for stateless applications
    /// (like web apps.)
    /// </summary>
    [Serializable]
    public class Game
    {
        /// <summary>
        /// NOTE: every game has an ID, but for the purposes of this exercise/prototype
        /// the ID is probably not gonna be used - additionally, the ID may not be an integer -
        /// if these objects are gonna get stored in an object database for example, it might
        /// be something else ...
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// The players in this game
        /// </summary>
        public List<Player> Players { get; set; }

        /// <summary>
        /// The rounds that have transpired (or are in progres) in this game so far
        /// </summary>
        public List<GameRound> GameRoundsCompleted { get; set; }

        /// <summary>
        /// How many total rounds should this game have?
        /// </summary>
        public int TotalRoundsInThisGame { get; set; }

        /// <summary>
        /// How many dice should a player roll on their turn?
        /// </summary>
        public int DiceToRollEachRound { get; set; }

        /// <summary>
        /// Record the id of the player that wins this game, so it doesn't
        /// have to be calculated over and over again, and to indicate when
        /// this game is considered complete
        /// </summary>
        public int WinningPlayerId { get; set; }

        /// <summary>
        /// Returns true if all the rounds associated to this game are complete (ie:
        /// all players have completed their turns for all rounds) and false otherwise.
        /// </summary>
        public virtual bool AllRoundsComplete()
        {
            if (this.GameRoundsCompleted.IsNullOrEmpty()) { return this.TotalRoundsInThisGame == 0; }
            if (this.Players.IsNullOrEmpty()) { return true; }
            if (this.TotalRoundsInThisGame <= 0) { return true; }
            if (this.GameRoundsCompleted.Count < this.TotalRoundsInThisGame) { return false; }

            foreach(GameRound round in this.GameRoundsCompleted)
            {
                if (!round.AllPlayerTurnsCompleted(this.Players, this.DiceToRollEachRound))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// This utility function subtracts the list 'right' from the list 'left', returning
        /// a new list of dice - neither of the given lists is modified.
        /// 
        /// This function is a bit dumb, on purpose.  It assumes that all of 'right' are present
        /// in 'left' and will barf if it discovers that's not true, primarily because the
        /// usage of this function (for now) is pretty specific.
        /// </summary>
        public virtual List<GameDice> SubtractDice(List<GameDice> left, List<GameDice> right)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns true if this object is considered
        /// equal to the given arg, false otherwise (and
        /// false if the given arg isn't a Game type.)
        /// </summary>
        public override bool Equals(object obj)
        {
            Game other = obj as Game;
            if (other == null) { return false; }

            if (this.Id != other.Id) { return false; }
            if (this.TotalRoundsInThisGame != other.TotalRoundsInThisGame) { return false; }
            if (this.DiceToRollEachRound != other.DiceToRollEachRound) { return false; }
            if (this.WinningPlayerId != other.WinningPlayerId) { return false; }

            if (this.Players == null && other.Players != null) { return false; }
            if (this.Players != null && other.Players == null) { return false; }
            if (this.Players.Count != other.Players.Count) { return false; }
            foreach(Player player in this.Players)
            {
                // order doesn't matter for player equality between game objects
                if (!other.Players.Contains(player))
                    return false;
            }

            if (this.GameRoundsCompleted == null && other.GameRoundsCompleted != null) { return false; }
            if (this.GameRoundsCompleted != null && other.GameRoundsCompleted == null) { return false; }
            if (this.GameRoundsCompleted.Count != other.GameRoundsCompleted.Count) { return false; }
            foreach(GameRound round in this.GameRoundsCompleted)
            {
                // order doesn't matter for game round equality between game objects
                if (!other.GameRoundsCompleted.Contains(round))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Returns the hashcode for this object
        /// </summary>
        public override int GetHashCode()
        {
            int hashcode = this.Id.GetHashCode() ^ 
                           this.TotalRoundsInThisGame.GetHashCode() ^ 
                           this.DiceToRollEachRound.GetHashCode() ^ 
                           this.WinningPlayerId.GetHashCode();

            if(!this.Players.IsNullOrEmpty())
            {
                foreach (Player player in this.Players)
                    hashcode ^= player.GetHashCode();
            }

            if(!this.GameRoundsCompleted.IsNullOrEmpty())
            {
                foreach (GameRound round in this.GameRoundsCompleted)
                    hashcode ^= round.GetHashCode();
            }

            return hashcode;
        }

        /// <summary>
        /// Returns a pretty string representation for this object
        /// </summary>
        public override string ToString()
        {
            StringBuilder bldr = new StringBuilder();
            bldr.AppendFormat("Game: Id: {0}, TotalRoundsInThisGame: {1}, DiceToRollEachRound: {2}, WinningPlayerId: {3}, ",
                              this.Id,
                              this.TotalRoundsInThisGame,
                              this.DiceToRollEachRound,
                              this.WinningPlayerId);

            if (this.Players.IsNullOrEmpty())
                bldr.Append("Players: None ");
            else
            {
                bldr.Append("Players: [ ");
                foreach (Player player in this.Players)
                    bldr.AppendFormat("[{0}], ", player.ToString());

                bldr.Append("] ");
            }

            if (this.GameRoundsCompleted.IsNullOrEmpty())
                bldr.Append("GameRoundsCompleted: None ");
            else
            {
                bldr.Append("GameRoundsCompleted: [ ");
                foreach (GameRound round in this.GameRoundsCompleted)
                    bldr.AppendFormat("[{0}], ", round.ToString());

                bldr.Append("] ");
            }

            return bldr.ToString();
        }
    }
}
