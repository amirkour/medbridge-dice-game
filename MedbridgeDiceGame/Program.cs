using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;
using GameAgents;
using GameObjects;

namespace MedbridgeDiceGame
{
    public class Program
    {
        /// <summary>
        /// This helper will simply configure/return a game object according to the rules
        /// lain out by the interview, which can be found in the README for this repo/project.
        /// </summary>
        protected Game GetPreConfiguredDefaultGame(out Player humanPlayer)
        {
            humanPlayer = new Player() { Id = 1, Type = AgentFactory.AGENT_TYPE_HUMAN_CONSOLE };

            // our default game will have a human/console player, and 3 reflex players
            List<Player> players = new List<Player>();
            players.Add(humanPlayer);
            players.Add(new Player() { Id = 2, Type = AgentFactory.AGENT_TYPE_AI_REFLEX });
            players.Add(new Player() { Id = 3, Type = AgentFactory.AGENT_TYPE_AI_REFLEX });
            players.Add(new Player() { Id = 4, Type = AgentFactory.AGENT_TYPE_AI_REFLEX });

            // we'll roll 5 dice per round
            int dicePerRound = 5;

            // and play for 4 rounds
            int totalRounds = 4;

            // we're gonna roll standard 6-sided dice, but 4's are worth 0!
            Dictionary<int, int> dice = new Dictionary<int, int>()
            {
                { 1, 1 },
                { 2, 2 },
                { 3, 3 },
                { 4, 0 },
                { 5, 5 },
                { 6, 6 }
            };

            Game game = new Game()
            {
                Players = players,
                TotalRoundsInThisGame = totalRounds,
                DiceToRollEachRound = dicePerRound,
                MapOfDiceValues = dice
            };

            return game;
        }

        /// <summary>
        /// Helper that will perform the 'turn' of the given player in the given game and associated
        /// that turn to the given game round
        /// </summary>
        protected void TakeTurnForPlayerInRound(Game game, Player player, GameRound round)
        {
            if (game == null || player == null || round == null) { throw new Exception("Cannot take player turn for null game and/or player and/or round"); }

            IGameAgent playerAgent = AgentFactory.GetAgentFor(player.Type);
            if(playerAgent == null)
                throw new Exception(String.Format("Could not retrieve a game agent for player {0}", player.Id));

            List<GameDice> remainingDice = game.GetRolledDice(game.DiceToRollEachRound);
            if (remainingDice.IsNullOrEmpty())
                throw new Exception("Failed to retrieve rolled dice for this game");

            Console.WriteLine(String.Format("Time for player {0} to take their turn!", player.Id));
            List<PlayerTurn> turnsTakenSoFar = new List<PlayerTurn>();
            int turnNumber = 1;
            while(remainingDice.Count > 0)
            {
                List<GameDice> diceToKeep = playerAgent.MakeMove(remainingDice, player, turnsTakenSoFar, game);
                if (diceToKeep.IsNullOrEmpty())
                    throw new Exception(String.Format("Player {0} did not make a valid move", player.Id));

                PlayerTurn nextTurn = new PlayerTurn()
                {
                    AvailableDice = remainingDice,
                    KeptDice = diceToKeep,
                    PlayerId = player.Id,
                    TurnNumber = turnNumber
                };

                round.TurnsTakenSoFar.Add(nextTurn);
                turnsTakenSoFar.Add(nextTurn);

                remainingDice = game.GetRolledDice(remainingDice.Count - diceToKeep.Count);
                turnNumber++;
            }

            Console.WriteLine(String.Format("Player {0}'s turn is done!", player.Id));
        }

        /// <summary>
        /// Helper that executes the next round of the given game, saving all info that occurs
        /// during the round to the game itself - the caller will probably want to save/persist
        /// this object after this method returns, because it'll be updated with all kinds of new
        /// round info.
        /// </summary>
        protected void ExecuteNextRoundOf(Game game)
        {
            if (game == null) { throw new Exception("Cannot execute a round for a null game object"); }

            // make sure this game has a non-null list of rounds ready to go
            if (game.GameRoundsCompleted == null)
                game.GameRoundsCompleted = new List<GameRound>();

            int nextRoundNumber = game.GameRoundsCompleted.Count + 1;
            Player nextStartingPlayer = game.GetNextStartingPlayer();
            if (nextStartingPlayer == null)
                throw new Exception("Failed to retrieve starting player for round " + nextRoundNumber);

            GameRound nextRound = new GameRound()
            {
                RoundNumber = nextRoundNumber,
                StartingPlayerId = nextStartingPlayer.Id,
                TurnsTakenSoFar = new List<PlayerTurn>()
            };

            Console.Out.WriteLine(String.Format("Starting round {0} - player with id {1} goes first ...", nextRoundNumber, nextStartingPlayer.Id));

            // give this round's starting player a shot to go, then let everybody else go in order
            this.TakeTurnForPlayerInRound(game, nextStartingPlayer, nextRound);
            
            foreach(Player player in game.Players)
            {
                if (player.Id == nextStartingPlayer.Id) { continue; }
                this.TakeTurnForPlayerInRound(game, player, nextRound);
            }

            // all done - add this round to the game and we're off to the races!
            Console.WriteLine(String.Format("Round {0} complete!", nextRoundNumber));
            game.GameRoundsCompleted.Add(nextRound);
        }

        protected void PrintResultsOf(Game game, Player humanPlayer)
        {
            if (game == null) { throw new Exception("Cannot print results of null game"); }

            Console.Out.WriteLine("Here's are the final scores for the game ...");
            Console.WriteLine(ConsoleUtils.GetPrettyScoreTableFor(game, humanPlayer));
            Console.WriteLine("");

            if (game.WinningPlayerIds.IsNullOrEmpty())
                Console.WriteLine("Whoops - it looks like the winners weren't properly recorded!?");
            else if(game.WinningPlayerIds.Count == 1)
            {
                Console.WriteLine(String.Format("Player with id {0} wins the game!", game.WinningPlayerIds[0]));
            }
            else
            {
                Console.WriteLine("Players with the following IDs win with a tie:");
                foreach (int id in game.WinningPlayerIds)
                    Console.WriteLine(id);
            }
        }

        public void PlayGame()
        {
            Console.WriteLine("Setting up a default dice game ...");
            Player humanPlayer = null;
            Game gameToPlay = this.GetPreConfiguredDefaultGame(out humanPlayer);

            Console.WriteLine("Entering main game loop ...");
            while (!gameToPlay.IsGameOver())
                this.ExecuteNextRoundOf(gameToPlay);

            this.PrintResultsOf(gameToPlay, humanPlayer);
        }

        public static void Main(string[] args)
        {
            Program prog = new Program();
            prog.PlayGame();
        }
    }
}
