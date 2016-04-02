using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Utils;
using GameObjects;

namespace GameObjectsTests
{
    [TestClass]
    public class GameTests
    {
        [TestMethod]
        public void GameTests_AllRoundsComplete_ReturnsTrue_WhenAllRoundsComplete()
        {
            var mockPlayers = new Mock<List<Player>>();
            int dicePerRound = 2;
            int roundsThisGame = 2;

            // our test game will have 2 game rounds - mock them so that they indicate that they're complete
            var mockRoundOne = new Mock<GameRound>();
            var mockRoundTwo = new Mock<GameRound>();
            mockRoundOne.Setup(round => round.AllPlayerTurnsCompleted(mockPlayers.Object, dicePerRound)).Returns(true);
            mockRoundTwo.Setup(round => round.AllPlayerTurnsCompleted(mockPlayers.Object, dicePerRound)).Returns(true);
            List<GameRound> rounds = new List<GameRound>();
            rounds.Add(mockRoundOne.Object);
            rounds.Add(mockRoundTwo.Object);

            // and setup our test game with our mock players and rounds
            Game game = new Game()
            {
                TotalRoundsInThisGame = roundsThisGame,
                DiceToRollEachRound = dicePerRound,
                Players = mockPlayers.Object,
                GameRoundsCompleted = rounds
            };

            Assert.IsTrue(game.AllRoundsComplete());
        }

        [TestMethod]
        public void GameTests_AllRoundsComplete_ReturnsFalse_WhenFewerCompletedRoundsThanTotal()
        {
            var mockPlayers = new Mock<List<Player>>();
            mockPlayers.Object.Add(new Player());
            int dicePerRound = 2;
            int totalRounds = 2;

            var mockRoundOne = new Mock<GameRound>();
            mockRoundOne.Setup(round => round.AllPlayerTurnsCompleted(mockPlayers.Object, dicePerRound)).Returns(true);

            List<GameRound> rounds = new List<GameRound>();
            rounds.Add(mockRoundOne.Object);

            Game game = new Game()
            {
                TotalRoundsInThisGame = totalRounds,
                DiceToRollEachRound = dicePerRound,
                GameRoundsCompleted = rounds,
                Players = mockPlayers.Object
            };

            Assert.IsFalse(game.AllRoundsComplete());
        }

        [TestMethod]
        public void GameTests_AllRoundsComplete_ReturnsFalse_WhenRoundsIncomplete()
        {
            var mockPlayers = new Mock<List<Player>>();
            mockPlayers.Object.Add(new Player());
            int dicePerRound = 2;
            int totalRounds = 2;

            var mockRoundOne = new Mock<GameRound>();
            var mockRoundTwo = new Mock<GameRound>();
            mockRoundOne.Setup(round => round.AllPlayerTurnsCompleted(mockPlayers.Object, dicePerRound)).Returns(true);
            mockRoundTwo.Setup(round => round.AllPlayerTurnsCompleted(mockPlayers.Object, dicePerRound)).Returns(false);

            List<GameRound> rounds = new List<GameRound>();
            rounds.Add(mockRoundOne.Object);
            rounds.Add(mockRoundTwo.Object);

            Game game = new Game()
            {
                TotalRoundsInThisGame = totalRounds,
                DiceToRollEachRound = dicePerRound,
                GameRoundsCompleted = rounds,
                Players = mockPlayers.Object
            };

            Assert.IsFalse(game.AllRoundsComplete());
        }
    }
}
