using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using GameManager;

public class StartGameTest
{
    [Test]
    public void TestSuccessfulStartAfterRollingStartDices()
    {
        // Arrange
        IGameServer gameServer = new LocalGameServer();
        BackgammonGame game = new BackgammonGame(gameServer, true);
        Board board = game.Board;
        Player player1 = board.Player1;
        Player player2 = board.Player2;

        player1.StartDice = Dice.THREE;
        player2.StartDice = Dice.SIX;

        // Act
        ICommand command = new StartCommand(1);
        game.ExecuteCommand(command);

        // Assert
        Assert.AreEqual(board.Turn, Colour.Black);
        Assert.AreEqual(board.ProgressStatus, ProgressStatus.NORMAL_PROGRESS, "Progress Status must change to normal on successful start");
    }

    [Test]
    public void TestFailingStartIfRollingDicesIsStillInProgress()
    {
        // Arrange
        IGameServer gameServer = new LocalGameServer();
        BackgammonGame game = new BackgammonGame(gameServer, true);
        Board board = game.Board;
        Player player1 = board.Player1;
        Player player2 = board.Player2;

        player1.StartDice = Dice.THREE;

        // Act
        ICommand command = new StartCommand(1);
        game.ExecuteCommand(command);

        // Assert
        Assert.That(board.Turn, Is.Null);
        Assert.AreEqual(board.ProgressStatus, ProgressStatus.JUST_STARTED, "Progress Status must not change to normal on fail start");
        Assert.That(game.Intent.Name == Intents.NO_ACTION);
    }    


    [Test]
    public void TestFailingStartIfRollingDicesAreEven()
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
        ICommand command = new StartCommand(1);
        game.ExecuteCommand(command);

        // Assert
        Assert.That(board.Turn, Is.Null);
        Assert.AreEqual(board.ProgressStatus, ProgressStatus.JUST_STARTED, "Progress Status must not change to normal on fail start");
        Assert.That(game.Intent.Name == Intents.NO_ACTION);
    }       

}
