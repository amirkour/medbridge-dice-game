using System;
using System.Threading;
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
            game.MapOfDiceValues = new Dictionary<int, int>();
            game.MapOfDiceValues[0] = 0;

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
        public void GameTests_GetRolledDice_GeneratesDice()
        {
            int numDieToRoll = 5;
            Game game = new Game();
            game.MapOfDiceValues = new Dictionary<int, int>();
            game.MapOfDiceValues[1] = 1;
            game.MapOfDiceValues[2] = 2;
            game.MapOfDiceValues[3] = 3;
            game.MapOfDiceValues[4] = 4;
            game.MapOfDiceValues[5] = 5;
            game.MapOfDiceValues[6] = 6;

            List<GameDice> listOne = game.GetRolledDice(numDieToRoll);
            Assert.IsNotNull(listOne);
            Assert.AreEqual(listOne.Count, numDieToRoll);
        }

        [TestMethod]
        public void GameTests_GetRolledDice_Throws_IfDiceMappingIsNullOrEmpty()
        {
            Exception e = null;
            Game game = new Game();
            try
            {
                game.GetRolledDice(5);
                Assert.Fail("This test should have thrown an exception");
            }
            catch(Exception ex)
            {
                e = ex;
            }

            Assert.IsNotNull(e);
        }

        [TestMethod]
        public void GameTests_GetNextStartingPlayer_Throws_IfThereAreNoPlayers()
        {
            Game game = new Game() { Players = null };
            Exception e = null;
            try
            {
                game.GetNextStartingPlayer();
                Assert.Fail("This test was supposed to throw an exception");
            }
            catch(Exception ex)
            {
                e = ex;
            }

            Assert.IsNotNull(e);
        }

        [TestMethod]
        public void GameTests_GetNextStartingPlayer_ReturnsPlayer_ThatHasntStartedYet()
        {
            List<Player> players = new List<Player>();
            players.Add(new Player() { Id = 1 });
            players.Add(new Player() { Id = 2 });

            List<GameRound> rounds = new List<GameRound>();

            Game game = new Game()
            {
                Players = players,
                GameRoundsCompleted = rounds
            };

            // fetch a starting player, and then set that players as having started and
            // try again - the second time we fetch a starting player, it shouldn't be
            // equal to the first!
            Player firstStarter = game.GetNextStartingPlayer();
            Assert.IsNotNull(firstStarter);
            Assert.IsTrue(firstStarter.Equals(players[0]) || firstStarter.Equals(players[1]));

            rounds.Add(new GameRound() { StartingPlayerId = firstStarter.Id });

            Player secondStarter = game.GetNextStartingPlayer();
            Assert.IsNotNull(secondStarter);
            Assert.AreNotEqual(firstStarter, secondStarter);
            Assert.AreNotEqual(secondStarter.Id, game.GameRoundsCompleted[0].StartingPlayerId);
        }

        [TestMethod]
        public void GameTests_GetNextStartingPlayer_Throws_IfARoundHadAStartingPlayerNotInTheGame()
        {
            List<Player> players = new List<Player>();
            players.Add(new Player() { Id = 1 });
            List<GameRound> rounds = new List<GameRound>();
            rounds.Add(new GameRound() { StartingPlayerId = 5 }); // this player isn't in the list!

            Game game = new Game()
            {
                Players = players,
                GameRoundsCompleted = rounds
            };
            Exception e = null;
            try
            {
                game.GetNextStartingPlayer();
                Assert.Fail("This test should have thrown an exception");
            }
            catch(Exception ex)
            {
                e = ex;
            }

            Assert.IsNotNull(e);
        }

        [TestMethod]
        public void GameTests_GetPlayerToScoreMapping_Throws_WhenARoundHasAPlayerNotInTheGame()
        {
            List<Player> players = new List<Player>();
            players.Add(new Player() { Id = 1 }); // only 1 player in this game, and they have ID=1

            Dictionary<int, int> roundOneScores = new Dictionary<int, int>();
            roundOneScores[5] = 10; // player with id 5 has score 10 in round one

            var mockRound = new Mock<GameRound>();
            mockRound.Setup(round => round.GetRoundScore()).Returns(roundOneScores);
            List<GameRound> rounds = new List<GameRound>();
            rounds.Add(mockRound.Object);

            Game game = new Game()
            {
                Players = players,
                GameRoundsCompleted = rounds
            };

            Exception e = null;
            try
            {
                game.GetPlayerToScoreMapping();
                Assert.Fail("This test should have thrown an exception");
            }
            catch(Exception ex)
            {
                e = ex;
            }

            Assert.IsNotNull(e);
        }

        [TestMethod]
        public void GameTests_GetPlayerToScoreMapping_ReturnsMappingOfPlayers_EvenWhenNobodyHasScored()
        {
            List<Player> players = new List<Player>();
            players.Add(new Player() { Id = 1 });
            players.Add(new Player() { Id = 2 });
            Game game = new Game() { Players = players };

            Dictionary<int, int> map = game.GetPlayerToScoreMapping();
            Assert.IsNotNull(map);
            Assert.AreEqual(map.Count, players.Count);
            Assert.IsTrue(map.ContainsKey(players[0].Id));
            Assert.IsTrue(map.ContainsKey(players[1].Id));
            Assert.IsTrue(map[1] == 0);
            Assert.IsTrue(map[2] == 0);
        }

        [TestMethod]
        public void GameTests_GetPlayerToScoreMapping_ReturnsMappingOfScores()
        {
            List<Player> players = new List<Player>();
            players.Add(new Player() { Id = 1 });
            players.Add(new Player() { Id = 2 });

            var mockRoundOne = new Mock<GameRound>();
            Dictionary<int, int> roundOneScores = new Dictionary<int, int>();
            roundOneScores[1] = 3;
            roundOneScores[2] = 1;
            mockRoundOne.Setup(round => round.GetRoundScore()).Returns(roundOneScores);

            var mockRoundTwo = new Mock<GameRound>();
            Dictionary<int, int> roundTwoScores = new Dictionary<int, int>();
            roundTwoScores[2] = 3;
            mockRoundTwo.Setup(round => round.GetRoundScore()).Returns(roundTwoScores);

            List<GameRound> rounds = new List<GameRound>();
            rounds.Add(mockRoundOne.Object);
            rounds.Add(mockRoundTwo.Object);

            Game game = new Game()
            {
                Players = players,
                GameRoundsCompleted = rounds
            };

            // get the scores - they should be the sum of all round scores for each player
            Dictionary<int, int> scores = game.GetPlayerToScoreMapping();
            Assert.IsNotNull(scores);
            Assert.AreEqual(scores.Count, players.Count);
            Assert.IsTrue(scores.ContainsKey(1));
            Assert.IsTrue(scores.ContainsKey(2));
            Assert.AreEqual(scores[1], 3);
            Assert.AreEqual(scores[2], 4);
        }

        [TestMethod]
        public void GameTests_GetLowestScoringPlayers_ReturnsNull_InAbsenceOfPlayers()
        {
            Game game = new Game()
            {
                Players = null
            };

            Assert.IsNull(game.GetLowestScoringPlayers());

            game.Players = new List<Player>();
            Assert.IsNull(game.GetLowestScoringPlayers());
        }

        [TestMethod]
        public void GameTests_GetLowestScoringPlayers_ReturnsEveryone_InAbsenceOfGameRounds()
        {
            Game game = new Game()
            {
                Players = new List<Player>(),
                GameRoundsCompleted = null
            };
            game.Players.Add(new Player() { Id = 1 });
            game.Players.Add(new Player() { Id = 2 });

            List<Player> lowScorers = game.GetLowestScoringPlayers();
            Assert.IsNotNull(lowScorers);
            Assert.AreEqual(game.Players.Count, lowScorers.Count);
            Assert.IsTrue(lowScorers.Contains(game.Players[0]));
            Assert.IsTrue(lowScorers.Contains(game.Players[1]));
        }

        [TestMethod]
        public void GameTests_GetLowestScoringPlayers_Throws_IfScoresHavePlayerNotInGame()
        {
            List<Player> players = new List<Player>();
            players.Add(new Player() { Id = 1 }); // only one player in this game, with ID=1

            // now generate a mock score table, with scores for a player not in this game
            Dictionary<int, int> scores = new Dictionary<int, int>();
            scores[1] = 1;
            scores[2] = 3;//this is the bogus one

            var mockGame = new Mock<Game>();
            mockGame.Setup(game => game.GetPlayerToScoreMapping()).Returns(scores);
            mockGame.Object.Players = players;
            Exception e = null;
            try
            {
                mockGame.Object.GetLowestScoringPlayers();
                Assert.Fail("This test should have thrown an exception");
            }
            catch(Exception ex)
            {
                e = ex;
            }

            Assert.IsNotNull(e);
        }

        [TestMethod]
        public void GameTests_GetLowestScoringPlayers_ReturnsLowestScoringPlayer()
        {
            List<Player> players = new List<Player>();
            players.Add(new Player() { Id = 1 });
            players.Add(new Player() { Id = 2 });

            // assemble some mock game rounds and scores
            Dictionary<int, int> roundOneScore = new Dictionary<int, int>();
            roundOneScore[1] = 3;
            roundOneScore[2] = 1; // so player 2 is the lowest scorer in round 1
            var mockRoundOne = new Mock<GameRound>();
            mockRoundOne.Setup(round => round.GetRoundScore()).Returns(roundOneScore);

            Dictionary<int, int> roundTwoScore = new Dictionary<int, int>();
            roundTwoScore[1] = 1;
            roundTwoScore[2] = 1; // both players tie in round 2 - so layer 2 is still the lowest!
            var mockRoundTwo = new Mock<GameRound>();
            mockRoundTwo.Setup(round => round.GetRoundScore()).Returns(roundTwoScore);

            List<GameRound> rounds = new List<GameRound>();
            rounds.Add(mockRoundOne.Object);
            rounds.Add(mockRoundTwo.Object);

            Game game = new Game()
            {
                Players = players,
                GameRoundsCompleted = rounds
            };

            List<Player> lowScorers = game.GetLowestScoringPlayers();
            Assert.IsNotNull(lowScorers);
            Assert.AreEqual(lowScorers.Count, 1);
            Assert.AreEqual(lowScorers[0], game.Players[1]);
        }

        [TestMethod]
        public void GameTests_IsGameOver_ReturnsFalse_WhenAllRoundsNotComplete()
        {
            var mockGame = new Mock<Game>();
            mockGame.Setup(game => game.AllRoundsComplete()).Returns(false);
            Assert.IsFalse(mockGame.Object.IsGameOver());
        }

        [TestMethod]
        public void GameTests_IsGameOver_ReturnsTrue_WhenAllRoundsComplete()
        {
            List<Player> winners = new List<Player>();
            winners.Add(new Player() { Id = 1 });

            var mockGame = new Mock<Game>();
            mockGame.Setup(game => game.AllRoundsComplete()).Returns(true);
            mockGame.Setup(game => game.GetLowestScoringPlayers()).Returns(winners);

            mockGame.Object.WinningPlayerIds = null;
            mockGame.Object.Players = new List<Player>();
            Assert.IsTrue(mockGame.Object.IsGameOver());
            Assert.IsNotNull(mockGame.Object.WinningPlayerIds);
            Assert.AreEqual(mockGame.Object.WinningPlayerIds[0], winners[0].Id);
        }

        [TestMethod]
        public void GameTests_IsGameOver_ReturnsTrue_WhenWinningIDsArePresent()
        {
            Game game = new Game()
            {
                WinningPlayerIds = new List<int>()
            };

            game.WinningPlayerIds.Add(1);
            Assert.IsTrue(game.IsGameOver());
        }
    }
}
