using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using GameManager;

public class StartDiceTest
{
    // A Test behaves as an ordinary method
    [Test]
    public void TestRollsADiceOnStart()
    {
        // Arrange
        IGameServer gameServer = new LocalGameServer();
        BackgammonGame game = new BackgammonGame(gameServer, true);
        Board board = game.Board;
        Player player1 = board.Player1;
        Player player2 = board.Player2;

        // Act
        ICommand command = new RollDiceStartCommand(1, Colour.White);
        game.ExecuteCommand(command);

        // Assert
        Assert.IsNotNull(player1.StartDice, "Player 1 Start Dice should not be null");
        Assert.IsNull(player2.StartDice, "Player 2 Start Dice should not be null");
    }

    [Test]
    public void TestCannotRollsStartDiceAfterStart()
    {
        // Arrange
        IGameServer gameServer = new LocalGameServer();
        BackgammonGame game = new BackgammonGame(gameServer, true);
        Board board = game.Board;
        Player player1 = board.Player1;
        Player player2 = board.Player2;

        board.Turn = Colour.White;
        board.ProgressStatus = ProgressStatus.NORMAL_PROGRESS;

        // Act
        ICommand command = new RollDiceStartCommand(1, Colour.White);
        game.ExecuteCommand(command);

        // Assert
        Assert.AreEqual(game.Intent.Name, Intents.NO_ACTION, "Can't roll start dice after the game is started.");
    }

    [Test]
    public void TestCannotResetRollsStartDiceAfterStart()
    {
        // Arrange
        IGameServer gameServer = new LocalGameServer();
        BackgammonGame game = new BackgammonGame(gameServer, true);
        Board board = game.Board;
        Player player1 = board.Player1;
        Player player2 = board.Player2;

        board.Turn = Colour.White;
        board.ProgressStatus = ProgressStatus.NORMAL_PROGRESS;

        // Act
        ICommand command = new ResetRollDiceStartCommand(1);
        game.ExecuteCommand(command);

        // Assert
        Assert.AreEqual(game.Intent.Name, Intents.NO_ACTION, "Can't reset roll start dice after the game is started.");
    }    



    [Test]
    public void TestSuccessfulResetRollsStartDiceAfterStart()
    {
        // Arrange
        IGameServer gameServer = new LocalGameServer();
        BackgammonGame game = new BackgammonGame(gameServer, true);
        Board board = game.Board;
        Player player1 = board.Player1;
        Player player2 = board.Player2;

        player1.StartDice = Dice.THREE;
        player2.StartDice = Dice.THREE;

        // Act
        ICommand command = new ResetRollDiceStartCommand(1);
        game.ExecuteCommand(command);

        // Assert
        Assert.That(player1.StartDice, Is.Null);
        Assert.That(player2.StartDice, Is.Null);
        Assert.That(board.Turn, Is.Null);
    }    

}
