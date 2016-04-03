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

        [TestMethod]
        public void GameTests_SubtractDice_Throws_WhenLeftIsNull()
        {
            Game game = new Game();
            List<GameDice> right = new List<GameDice>();
            List<GameDice> left = null;
            Exception e = null;
            try
            {
                game.SubtractDice(left, right);
                Assert.Fail("This test was supposed to throw an exception but it didn't");
            }
            catch(Exception ex)
            {
                e = ex;
            }

            Assert.IsNotNull(e);
        }

        [TestMethod]
        public void GameTests_SubtractDice_ReturnsEmptyList_WhenLeftIsEmpty_AndRightIsNullOrEmpty()
        {
            List<GameDice> left = new List<GameDice>();
            List<GameDice> right = null;
            Game game = new Game();

            List<GameDice> result = game.SubtractDice(left, right);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Count, 0);

            right = new List<GameDice>();
            result = game.SubtractDice(left, right);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Count, 0);
        }

        [TestMethod]
        public void GameTests_SubtractDice_Throws_WhenRightHasElementsNotInLeft()
        {
            List<GameDice> left = new List<GameDice>();
            List<GameDice> right = new List<GameDice>();
            right.Add(new GameDice() { ActualValue = 1, FaceValue = 1 });
            Game game = new Game();

            Exception e = null;
            try
            {
                game.SubtractDice(left, right);
                Assert.Fail("This test should have thrown an exception");
            }
            catch (Exception ex)
            {
                e = ex;
            }

            Assert.IsNotNull(e);

            // now just put something bogus in 'left' and make sure it still barfs
            left.Add(new GameDice() { ActualValue = right[0].ActualValue + 1, FaceValue = right[0].FaceValue + 1 });
            e = null;
            try
            {
                game.SubtractDice(left, right);
                Assert.Fail("This test should have failed but didn't");
            }
            catch(Exception ex)
            {
                e = ex;
            }

            Assert.IsNotNull(e);
        }

        [TestMethod]
        public void GameTests_SubtractDice_ReturnsListWithElementsOfLeft_WhenRightIsEmptyAndLeftIsNot()
        {
            List<GameDice> left = new List<GameDice>();
            List<GameDice> right = new List<GameDice>();
            left.Add(new GameDice() { ActualValue = 1, FaceValue = 1 });
            Game game = new Game();

            List<GameDice> result = game.SubtractDice(left, right);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Count, 1);
            Assert.IsTrue(result.Contains(left[0]));
            Assert.IsFalse(result == left); // it shouldn't be the exact same list
        }

        [TestMethod]
        public void GameTests_SubtractDice_ReturnsSubtractedElementsInNewList_WhenRightSubtractedFromLeft()
        {
            List<GameDice> left = new List<GameDice>();
            List<GameDice> right = new List<GameDice>();
            Game game = new Game();

            left.Add(new GameDice() { ActualValue = 1, FaceValue = 1 });
            left.Add(new GameDice() { ActualValue = 2, FaceValue = 2 });
            right.Add(left[1]);

            List<GameDice> result = game.SubtractDice(left, right);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Count, 1);
            Assert.IsTrue(result.Contains(left[0]));
            Assert.AreEqual(left.Count, 2);  // the original lists should not have been modified at all
            Assert.AreEqual(right.Count, 1);
        }

        [TestMethod]
        public void GameTests_SubtractDice_CanHandleDupeDice()
        {
            List<GameDice> left = new List<GameDice>();
            List<GameDice> right = new List<GameDice>();
            Game game = new Game();

            left.Add(new GameDice() { ActualValue = 1, FaceValue = 1 });
            left.Add(new GameDice() { ActualValue = 1, FaceValue = 1 });
            right.Add(new GameDice() { ActualValue = 1, FaceValue = 1 });
            List<GameDice> result = game.SubtractDice(left, right);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Count, 1);
            Assert.IsTrue(result.Contains(left[0]));
            Assert.AreEqual(left.Count, 2);
            Assert.AreEqual(right.Count, 1);
        }

        [TestMethod]
        public void GameTests_GetRolledDice_Throws_ForNegativeArg()
        {
            Game game = new Game();
            Exception e = null;
            try
            {
                game.GetRolledDice(-1);
                Assert.Fail("This test should have thrown an exception");
            }
            catch(Exception ex)
            {
                e = ex;
            }

            Assert.IsNotNull(e);
        }

        [TestMethod]
        public void GameTests_GetRolledDice_IsRandom()
        {
            int numDieToRoll = 5;
            Game game = new Game();

            List<GameDice> listOne = game.GetRolledDice(numDieToRoll);
            List<GameDice> listTwo = game.GetRolledDice(numDieToRoll);
            Assert.IsNotNull(listOne);
            Assert.IsNotNull(listTwo);
            Assert.AreEqual(listOne.Count, listTwo.Count);
            Assert.AreEqual(listOne.Count, numDieToRoll);
            Assert.AreNotEqual(listOne[0], listTwo[0]);
            Assert.AreNotEqual(listOne[1], listTwo[1]);
            Assert.AreNotEqual(listOne[2], listTwo[2]);
            Assert.AreNotEqual(listOne[3], listTwo[3]);
            Assert.AreNotEqual(listOne[4], listTwo[4]);
        }
    }
}
