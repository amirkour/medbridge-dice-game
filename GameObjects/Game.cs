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
        /// During a dice game, the user(s) may have opted to specify some
        /// dice as 'wild' or 'modified' - ie: a 3 may be worth 0, etc.
        /// The following hash will map 'face' values to 'actual' values
        /// for this game
        /// </summary>
        public Dictionary<int,int> MapOfDiceValues { get; set; }

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
            if(left == null) { throw new Exception("Cannot subtract any dice from a null list"); }
            if(left.Count <= 0)
            {
                if (right.IsNullOrEmpty()) { return new List<GameDice>(); }

                throw new Exception("Cannot subtract non-empty right list from empty left list");
            }

            List<GameDice> result = new List<GameDice>();

            // at this point, we have a 'left' list with elements - if 'right' is null/empty just copy 'left'
            if(right.IsNullOrEmpty())
            {
                left.ForEach(die => result.Add(die));
                return result;
            }

            // we need to tally-up the dice in 'left' and then incrementally 'subtract'
            // the stuff from the 'right' list
            Dictionary<GameDice, int> map = new Dictionary<GameDice, int>();
            foreach(GameDice die in left)
            {
                if (map.ContainsKey(die))
                    map[die] += 1;
                else
                    map[die] = 1;
            }

            foreach(GameDice dieToRemove in right)
            {
                if (!map.ContainsKey(dieToRemove))
                    throw new Exception(String.Format("Cannot subtract a die that's in the right list but not the left: {0}", dieToRemove.ToString()));

                if (map[dieToRemove] <= 0)
                    throw new Exception(String.Format("Cannot subtract a die that's in-excess in the right list: {0}", dieToRemove.ToString()));

                map[dieToRemove] -= 1;
            }

            foreach(KeyValuePair<GameDice, int> nextDieTuple in map)
            {
                if (nextDieTuple.Value <= 0) { continue; }
                for (int i = 0; i < nextDieTuple.Value; i++)
                    result.Add(nextDieTuple.Key);
            }

            return result;
        }

        /// <summary>
        /// Just an internal helper fashioned after Int.TryParse et. al. - will 
        /// get/populate the smallest and largest key of the given mapping of 
        /// integer-to-integer.  Return strue if successful, false otherwise.
        /// </summary>
        protected virtual bool TryGetMinAndMaxKeys(Dictionary<int,int> map, out int min, out int max)
        {
            min = max = 0;
            if (map.IsNullOrEmpty()) { return false; }
            min = max = map.Keys.First();
            foreach(KeyValuePair<int,int> tuple in map)
            {
                if (tuple.Key < min) { min = tuple.Key; }
                if (tuple.Key > max) { max = tuple.Key; }
            }

            return true;
        }

        /// <summary>
        /// This method simply returns a list of randomly generated dice - the caller
        /// can specify how many such dice they want w/ the passed-in arg
        /// </summary>
        public virtual List<GameDice> GetRolledDice(int numDiceToRoll)
        {
            if (this.MapOfDiceValues.IsNullOrEmpty()) { throw new Exception("Cannot generate rolled dice without a mapping of face-to-actual values"); }
            int minFaceValue = 0;
            int maxFaceValue = 0;
            if (!this.TryGetMinAndMaxKeys(this.MapOfDiceValues, out minFaceValue, out maxFaceValue))
                throw new Exception("Failed to retrieve min and/or max face value for the dice in this game");

            maxFaceValue++; // Random.Next(int,int) is not inclusive on the 'max' value
            Random rand = new Random(DateTime.Now.Millisecond);
            List<GameDice> result = new List<GameDice>();
            for (int i = 0; i < numDiceToRoll; i++)
            {
                int nextFaceValue = rand.Next(minFaceValue, maxFaceValue);
                int nextActualValue = this.MapOfDiceValues.ContainsKey(nextFaceValue) ? this.MapOfDiceValues[nextFaceValue] : nextFaceValue;
                result.Add(new GameDice() { FaceValue = nextFaceValue, ActualValue = nextActualValue });
            }

            return result;
        }

        /// <summary>
        /// As every game progresses, every player in the game should get a chance
        /// to start a round - this method will give back the player who should start
        /// the next round.
        /// 
        /// For now, the algorithm is just 'round robin'
        /// </summary>
        public virtual Player GetNextStartingPlayer()
        {
            if (this.Players.IsNullOrEmpty()) { throw new Exception("Cannot get a starting player when the game has no/null players"); }
            if (this.GameRoundsCompleted.IsNullOrEmpty()) { return this.Players[0]; }

            // map each player in this game to a bool - true if they've started, false otherwise
            Dictionary<int, bool> mapStarters = new Dictionary<int, bool>();
            this.Players.ForEach(player => mapStarters[player.Id] = false);

            // now hit each game round - if the round has a starting player id that doesn't
            // exist in this game, it's a data discrepancy that needs to be reported.
            // otherwise, toggle the status for that player in our mapping
            foreach(GameRound round in this.GameRoundsCompleted)
            {
                if (!mapStarters.ContainsKey(round.StartingPlayerId))
                    throw new Exception(String.Format("Encountered starting player id {0} in one of this game's rounds - this player id doesn't exist in the current game!", round.StartingPlayerId));
                else
                    mapStarters[round.StartingPlayerId] = true;
            }

            // now pick a random starter.  if everyone has already started,
            // just randomly pick the next starter
            Player nextStarter = null;
            foreach(KeyValuePair<int,bool> tuple in mapStarters)
            {
                if(tuple.Value == false)
                {
                    nextStarter = this.Players.First(player => player.Id == tuple.Key);
                    break;
                }
            }

            if (nextStarter == null)
                nextStarter = this.Players[0];

            return nextStarter;
        }

        /// <summary>
        /// Returns a mapping of player-id to score - the total score for each
        /// player in this game so far.
        /// 
        /// This method should always return something, even if all scores are 0.
        /// If there's a game round associated to this game that has a starting player that's
        /// NOT in this game, this method will throw (it's a data discrepancy that should be
        /// reported.)
        /// </summary>
        public virtual Dictionary<int,int> GetPlayerToScoreMapping()
        {
            Dictionary<int, int> scores = new Dictionary<int, int>();

            // if there aren't any players in the game, then there shouldn't be any game rounds either.
            // if there are rounds, throw.  otherwise, just return the empty list of scores.
            if(this.Players.IsNullOrEmpty())
            {
                if (!this.GameRoundsCompleted.IsNullOrEmpty()) { throw new Exception("This game has no players, but there are recorded game rounds present - this should not be possible!?"); }

                return scores;
            }

            // likewise, if there are no rounds recorded, just kick back 0-scores for everybody
            if(this.GameRoundsCompleted.IsNullOrEmpty())
            {
                this.Players.ForEach(player => scores[player.Id] = 0);
                return scores;
            }

            foreach(GameRound round in this.GameRoundsCompleted)
            {
                Dictionary<int, int> roundScores = round.GetRoundScore();
                if (roundScores.IsNullOrEmpty())
                    continue;

                foreach(KeyValuePair<int,int> tuple in roundScores)
                {
                    if (scores.ContainsKey(tuple.Key))
                        scores[tuple.Key] += tuple.Value;
                    else
                        scores[tuple.Key] = tuple.Value;
                }
            }

            // now we have to audit the scores - if there's a player ID in their for whom no player
            // exists in this game, we should report it here cuz it's a data discrepancy.
            //
            // and while we're tallying-up the player IDs, let's make sure the score table has
            // SOMETHING recorded for them
            HashSet<int> validIds = new HashSet<int>();
            foreach(Player player in this.Players)
            {
                validIds.Add(player.Id);
                if (!scores.ContainsKey(player.Id))
                    scores[player.Id] = 0;
            }

            foreach(KeyValuePair<int,int> tuple in scores)
            {
                if(!validIds.Contains(tuple.Key))
                    throw new Exception(String.Format("This game has a score recorded for player with id {0} but no such player exists in this game!", tuple.Key));
            }

            return scores;
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

            if (this.MapOfDiceValues == null && other.MapOfDiceValues != null) { return false; }
            if (this.MapOfDiceValues != null && other.MapOfDiceValues == null) { return false; }
            if (this.MapOfDiceValues.Count != other.MapOfDiceValues.Count) { return false; }
            foreach(KeyValuePair<int,int> tuple in this.MapOfDiceValues)
            {
                if (!other.MapOfDiceValues.ContainsKey(tuple.Key) || other.MapOfDiceValues[tuple.Key] != tuple.Value)
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

            if (!this.Players.IsNullOrEmpty()) { this.Players.ForEach(player => hashcode ^= player.GetHashCode()); }
            if (!this.GameRoundsCompleted.IsNullOrEmpty()) { this.GameRoundsCompleted.ForEach(round => hashcode ^= round.GetHashCode()); }
            if(!this.MapOfDiceValues.IsNullOrEmpty())
            {
                foreach (KeyValuePair<int, int> tuple in this.MapOfDiceValues)
                    hashcode ^= tuple.GetHashCode();
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

            if (this.MapOfDiceValues.IsNullOrEmpty())
                bldr.Append("MapOfDiceValues: None");
            else
            {
                bldr.Append("MapOfDiceValues: [");
                foreach (KeyValuePair<int, int> tuple in this.MapOfDiceValues)
                    bldr.AppendFormat("({0}:{1}), ", tuple.Key, tuple.Value);

                bldr.Append("] ");
            }

            return bldr.ToString();
        }
    }
}
