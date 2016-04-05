using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Utils;
using GameObjects;

namespace GameAgents
{

    /// <summary>
    /// This agent will print instructions and game state to a text-writer, and prompt for input
    /// to/on a text-reader, to pick which of the given dice to keep/return for a game move.
    /// 
    /// If, at construction time, no implementations are given for a TextWriter and/or TextReader,
    /// this class defaults to the console.
    /// 
    /// This agent is suitable for usage in a console app, where a user can be asked to chose which
    /// dice to keep for a game move.
    /// </summary>
    public class UserInputConsoleAgent : IGameAgent
    {
        // all prompting and pretty-printing will be done to the following writer
        protected TextWriter _out;

        // all input will be taken from the following reader
        protected TextReader _reader;

        /// <summary>
        /// Default constructor will default input/output to/from Console.In/Console.Out
        /// </summary>
        public UserInputConsoleAgent()
        {
            _out = Console.Out;
            _reader = Console.In;
        }

        /// <summary>
        /// Construction overload will permit a given text writer/reader - if either
        /// is null, this constructor falls back to Console.In/Out
        /// </summary>
        public UserInputConsoleAgent(TextWriter newOut, TextReader newIn)
        {
            _out = newOut != null ? newOut : Console.Out;
            _reader = newIn != null ? newIn : Console.In;
        }

        /// <summary>
        /// Helper that returns true if at least one of the booleans in the given
        /// value collection of bools is false.  Returns true otherwise.  Also
        /// returns false if the given collection is null/empty.
        /// </summary>
        protected bool AtLeastOneIsFalse(Dictionary<int,bool>.ValueCollection bools)
        {
            if (bools == null) { return false; }
            foreach(bool next in bools)
            {
                if (!next) { return true; }
            }

            return false;
        }

        protected void PrintStatusFor(Game game, Player currentPlayer)
        {
            if (game == null) { return; } // todo - log this error

            _out.WriteLine("");
            _out.WriteLine("Latest game stats ...");

            int roundNumber = 0;
            if(!game.GameRoundsCompleted.IsNullOrEmpty())
            {
                foreach(GameRound round in game.GameRoundsCompleted)
                {
                    if (round.AllPlayerTurnsCompleted(game.Players, game.TotalRoundsInThisGame))
                        roundNumber++;
                }
            }

            _out.WriteLine(String.Format("You are currently in round {0} of {1} for this game", ConsoleUtils.GetCurrentRoundNumberFor(game), game.TotalRoundsInThisGame));
            _out.WriteLine(ConsoleUtils.GetPrettyScoreTableFor(game, currentPlayer));
            _out.WriteLine(ConsoleUtils.GetDiceMappingDescriptionFor(game));
        }

        protected void PrintHelp()
        {
            _out.WriteLine("");
            _out.WriteLine("*** HELP ***");
            _out.WriteLine("");
            _out.WriteLine("This is the coolest dice game ever!");
            _out.WriteLine("");
            _out.WriteLine("The point of the game is to come out with the LOWEST score possible.");
            _out.WriteLine("");
            _out.WriteLine("Every round, you'll have to roll a bunch of dice and keep AT LEAST ONE every");
            _out.WriteLine("time you roll (but you can keep more if you want) - you'll continue");
            _out.WriteLine("rolling/keeping dice until all the dice have been kept, and then the");
            _out.WriteLine("other players will do the same - for every round, all players roll/hold dice");
            _out.WriteLine("in this fashion, and your score will get added up from round-to-round.");
            _out.WriteLine("The player with the lowest score at the end wins, so try to hang on to low");
            _out.WriteLine("valued dice!");
            _out.WriteLine("");
            _out.WriteLine("All available selections for this round are printed out with brackets around them, and");
            _out.WriteLine("before you can stop, you must elect to keep at least one die from the available choices.");
            _out.WriteLine("");
            _out.WriteLine("Once you select one or more dice to keep, they'll be marked - you can re-select these");
            _out.WriteLine("die to change your mind.");
            _out.WriteLine("");
        }
        
        /// Prompts for which of the given available dice to keep
        /// for this game move, and returns those dice.
        /// 
        /// All input/output is done via _reader/_out
        /// </summary>
        public List<GameDice> MakeMove(List<GameDice> availableDice, 
                                       Player playerToMove, 
                                       List<PlayerTurn> turnsTakenSoFar, 
                                       Game game)
        {
            _out.WriteLine("Time to make a move!");
            if(availableDice.IsNullOrEmpty())
            {
                _out.WriteLine("Whoops!  There are no available dice to chose from - nothing left to do, halting!");
                return null;
            }

            // keep track of which available dice indices have been selected.
            // if they're selected, they're unavailable (false) - otherwise, they're
            // available (true) in this map:
            Dictionary<int, bool> availability = new Dictionary<int, bool>();
            for (int i = 0; i < availableDice.Count; i++)
                availability[i] = true;

            bool halt = false;
            
            // enter a never-ending loop, prompting the user for input.
            // only halt when they tell us to, and even then, only if they've
            // selected a die to keep.
            do
            {
                _out.WriteLine("");
                _out.WriteLine("Here are the dice available/selected:");
                for (int i = 0; i < availableDice.Count; i++)
                {
                    if (!availability.ContainsKey(i)) { continue; }// todo - log this error

                    if (availability[i])
                        _out.WriteLine(String.Format("[{0}]\t{1}", i, availableDice[i].FaceValue));
                    else
                        _out.WriteLine(String.Format(" {0} \t{1}\tSELECTED", i, availableDice[i].FaceValue));
                }
                _out.WriteLine("------------------------");
                _out.WriteLine("[S]\tFor up-to-date game stats");
                _out.WriteLine("[H]\tFor help/instructions");
                _out.WriteLine("[Q]\tWhen you're done making your selection");
                _out.WriteLine("------------------------");
                _out.WriteLine("Please make a selection:");

                string input = _reader.ReadLine();
                int parsedSelectionNumber = 0;


                if (input.IsNullOrEmpty())
                {
                    _out.WriteLine("Please make a valid selection");
                    _out.WriteLine("");
                    continue;
                }

                switch(input.Substring(0,1).ToLower())
                {
                    // user wants to quit/stop
                    case "q":
                    case "Q":

                        // only let the user quit if they've successfully selected one die.
                        // in other words: at least one die is no longer available
                        if (this.AtLeastOneIsFalse(availability.Values))
                        {
                            _out.WriteLine("");
                            _out.WriteLine("Halting ...");
                            _out.WriteLine("");
                            halt = true;
                        }
                        else
                        {
                            _out.WriteLine("");
                            _out.WriteLine("You must elect to keep AT LEAST ONE die!");
                            _out.WriteLine("");
                        }
                        break;

                    // user wants game stats
                    case "s":
                    case "S":
                        this.PrintStatusFor(game, playerToMove);
                        break;

                    // user needs help
                    case "h":
                    case "H":
                        this.PrintHelp();
                        break;

                    default:
                        if(Int32.TryParse(input.Substring(0,1), out parsedSelectionNumber))
                        {
                            if(parsedSelectionNumber >= 0 && parsedSelectionNumber < availableDice.Count)
                            {
                                // as the user selects dice to keep, it'll act as a toggle, so they can
                                // change their mind if they want
                                availability[parsedSelectionNumber] = !availability[parsedSelectionNumber];
                            }
                            else
                            {
                                _out.WriteLine("");
                                _out.WriteLine("Please enter a valid number/selection");
                                _out.WriteLine("");
                            }
                        }
                        else
                        {
                            _out.WriteLine("");
                            _out.WriteLine("Please enter a valid letter/number from the available options!");
                            _out.WriteLine("");
                        }
                        break;
                }

            } while (halt == false);

            List<GameDice> diceToKeep = new List<GameDice>();
            foreach(KeyValuePair<int,bool> tuple in availability)
            {
                // all die not available for selection are those that the user
                // chose to keep - toss 'em in the return list
                if (!tuple.Value)
                    diceToKeep.Add(availableDice[tuple.Key]);
            }

            return diceToKeep;
        }
    }
}
