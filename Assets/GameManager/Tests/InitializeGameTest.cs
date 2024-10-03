using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using GameManager;

public class InitializeGameTest
{
    // A Test behaves as an ordinary method
    [Test]
    public void InitializeGameTestSimplePasses()
    {
        // Arrange
        IGameServer gameServer = new LocalGameServer();
        BackgammonGame game = new BackgammonGame(gameServer, true);
        Board board = game.Board;
        Player player1 = board.Player1;
        Player player2 = board.Player2;

        // Act

        // Assert
        Assert.IsTrue(game.TestRun);
        Assert.IsNull(board.Turn);
        Assert.That(player1, Is.Not.Null);
        Assert.That(player2, Is.Not.Null);
        Assert.IsTrue(player1.Colour == Colour.White);
        Assert.IsTrue(player2.Colour == Colour.Black);
        Assert.That(player1.StartDice, Is.Null);
        Assert.That(player2.StartDice, Is.Null);

        Assert.AreEqual(player1.History.Count, 0, "Player 1 History is not empty at initialize");
        Assert.AreEqual(player2.History.Count, 0, "Player 1 History is not empty at initialize");

        Assert.AreEqual(player1.Points[Point.Z11], 2);
        Assert.AreEqual(player2.Points[Point.Z11], 2);

        Assert.AreEqual(player1.Points[Point.Z26], 5);
        Assert.AreEqual(player2.Points[Point.Z26], 5);    

        Assert.AreEqual(player1.Points[Point.Z35], 3);
        Assert.AreEqual(player2.Points[Point.Z35], 3);   

        Assert.AreEqual(player1.Points[Point.Z41], 5);
        Assert.AreEqual(player2.Points[Point.Z41], 5);                        
    }

}
