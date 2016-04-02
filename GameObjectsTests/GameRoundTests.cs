using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GameObjects;
using Utils;

namespace GameObjectsTests
{
    [TestClass]
    public class GameRoundTests
    {
        [TestMethod]
        public void GameRoundTests_AllPlayerTurnsCompleted_ReturnsTrue_WhenKeptDiceEqualsTotalDicePerRound()
        {
            // setup the dummy/test players
            List<Player> players = new List<Player>();
            players.Add(new Player() { Id = 1 });
            players.Add(new Player() { Id = 2 });

            // and the number of dice each player rolls per round
            int dicePerRound = 2;

            // now setup/simulate each players turns - each player should have dicePerRound 'kept' dice total
            List<PlayerTurn> turns = new List<PlayerTurn>();

            // let's say player 1 had only 1 turn, where they kept two dice
            turns.Add(new PlayerTurn() { PlayerId = 1, AvailableDice = null, KeptDice = new List<GameDice>() });
            turns[0].KeptDice.Add(new GameDice() { ActualValue = 1, FaceValue = 1 });
            turns[0].KeptDice.Add(new GameDice() { ActualValue = 2, FaceValue = 2 });

            // but player 2 took 2 turns, and kept 1 die each turn for a total of 2
            turns.Add(new PlayerTurn() { PlayerId = 2, AvailableDice = null, KeptDice = new List<GameDice>() });
            turns[1].KeptDice.Add(new GameDice() { ActualValue = 1, FaceValue = 1 });
            turns.Add(new PlayerTurn() { PlayerId = 2, AvailableDice = null, KeptDice = new List<GameDice>() });
            turns[1].KeptDice.Add(new GameDice() { ActualValue = 3, FaceValue = 3 });

            // now our game round to test
            GameRound round = new GameRound() { TurnsTakenSoFar = turns };
            Assert.IsTrue(round.AllPlayerTurnsCompleted(players, dicePerRound));
        }

        [TestMethod]
        public void GameRoundTests_AllPlayerTurnsCompleted_ReturnsFalse_WhenAGivenPlayerIsNotInThisRound()
        {
            // let's setup a list of players for this round
            List<Player> players = new List<Player>();
            players.Add(new Player() { Id = 1 });
            players.Add(new Player() { Id = 2 });
            int dicePerRound = 1;

            // now setup a list of player turns, and make sure one of the players is excluded
            List<PlayerTurn> turns = new List<PlayerTurn>();
            turns.Add(new PlayerTurn() { KeptDice = new List<GameDice>(), PlayerId = 1 });
            turns[0].KeptDice.Add(new GameDice() { ActualValue = 1, FaceValue = 1 });

            // now t-up our game round for testing - if one of the players isn't in the round,
            // it just means they haven't gotten to take their turn for this round!
            GameRound round = new GameRound() { TurnsTakenSoFar = turns };
            Assert.IsFalse(round.AllPlayerTurnsCompleted(players, dicePerRound));
        }

        [TestMethod]
        public void GameRoundTests_AllPlayerTurnsCompleted_ReturnsFalse_WhenKeptDiceNotEqualToDicePerRound()
        {
            // easy case - put each player in the round, but make sure they haven't kept the total
            // dice per round
            int dicePerRound = 2;
            List<Player> players = new List<Player>();
            players.Add(new Player() { Id = 1 });
            players.Add(new Player() { Id = 2 });

            List<PlayerTurn> turns = new List<PlayerTurn>();
            turns.Add(new PlayerTurn() { KeptDice = new List<GameDice>(), PlayerId = 1 });
            turns[0].KeptDice.Add(new GameDice() { FaceValue = 1, ActualValue = 1 });

            turns.Add(new PlayerTurn() { KeptDice = new List<GameDice>(), PlayerId = 2 });
            turns[1].KeptDice.Add(new GameDice() { FaceValue = 1, ActualValue = 1 });

            GameRound round = new GameRound() { TurnsTakenSoFar = turns };
            Assert.IsFalse(round.AllPlayerTurnsCompleted(players, dicePerRound));
        }

        [TestMethod]
        public void GameRoundTests_AllPlayerTurnsCompleted_Throws_WhenAPlayerInTheRoundIsNotPresentInListOfPlayers()
        {
            // setup a game round, during which a player turn transpired for a player that isn't in the list
            // of player args.  this would correspond to a corrupt database entry or something ...
            List<Player> players = new List<Player>();
            players.Add(new Player() { Id = 1 });
            int dicePerRound = 1;

            List<PlayerTurn> turns = new List<PlayerTurn>();
            turns.Add(new PlayerTurn() { PlayerId = 5, KeptDice = new List<GameDice>() });
            turns[0].KeptDice.Add(new GameDice() { FaceValue = 1, ActualValue = 1 });

            GameRound round = new GameRound() { TurnsTakenSoFar = turns };
            Exception e = null;
            try
            {
                round.AllPlayerTurnsCompleted(players, dicePerRound);
                Assert.Fail("Expected an exception but got none");
            }
            catch (Exception ex)
            {
                e = ex;
            }

            Assert.IsNotNull(e);
        }
    }
}
