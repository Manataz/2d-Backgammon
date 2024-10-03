using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using UnityEngine.AI;

namespace GameManager
{

    public class Board : DataModel
    {
        /// <summary>
        /// TODO change this from game config
        /// Players can have it own timer based on its class
        /// </summary>
        const int TIMER = 30;

        private Player player1;

        public Player Player1
        {
            get => player1;
        }

        private Player player2;

        public Player Player2
        {
            get => player2;
        }

        private Colour? turn = null;

        public Colour? Turn
        {
            get => turn;
            set
            {
                turn = value;
                OnPropertyChanged();
            }
        }

        private Colour? doubleTurn = null;

        public Colour? DoubleTurn
        {
            get => doubleTurn;
            set
            {
                doubleTurn = value;
                OnPropertyChanged();
            }
        }

        private Colour? winner = null;

        public Colour? Winner
        {
            get => winner;
            set
            {
                winner = value;
                OnPropertyChanged();
            }
        }

        private int doubleCoins = 0;

        public int DoubleCoins
        {
            get => doubleCoins;
            set
            {
                doubleCoins = value;
                OnPropertyChanged();
            }
        }

        private List<Point> highlightedPoints;

        public List<Point> HighlightedPoints
        {
            get => highlightedPoints;
            set
            {
                highlightedPoints = value;
                OnPropertyChanged();
            }
        }

        private ProgressStatus progressStatus;

        public ProgressStatus ProgressStatus
        {
            get => progressStatus;
            set
            {
                progressStatus = value;
                OnPropertyChanged();
            }
        }

        private FinishStatus? finishStatus;

        public FinishStatus? FinishStatus
        {
            get => finishStatus;
            set
            {
                finishStatus = value;
                OnPropertyChanged();
            }
        }

        public Board()
        {

            player1 = new Player(Colour.White);
            player2 = new Player(Colour.Black);

            finishStatus = null;
            progressStatus = ProgressStatus.JUST_STARTED;

            highlightedPoints = new List<Point>();
            winner = null;
            doubleCoins = 0;
            doubleTurn = null;

        }

        public Board(Board refBoard) : this()
        {
            Copy(refBoard);
        }

        /// <summary>
        /// It'll copy new board data to existing board
        /// TODO: it can be optimised to check that only set some items if they are changed
        /// </summary>
        /// <param name="newBoard"></param>
        public void Copy(Board newBoard)
        {
            Turn = newBoard.Turn;
            DoubleTurn = newBoard.DoubleTurn;
            Winner = newBoard.Winner;
            DoubleCoins = newBoard.DoubleCoins;
            HighlightedPoints = newBoard.HighlightedPoints;
            ProgressStatus = newBoard.ProgressStatus;
            FinishStatus = newBoard.FinishStatus;

            player1.Copy(newBoard.player1);
            player2.Copy(newBoard.player2);
        }

        /// <summary>
        /// Changes board state to ready state including clear history moves, dices etc.
        /// </summary>
        public void ToReadyState()
        {
            Player1.History.Clear();
            Player2.History.Clear();

            Player1.RolledDices.Clear();
            Player1.RemainingDices.Clear();

            Player2.RolledDices.Clear();
            Player2.RemainingDices.Clear();

            HighlightedPoints.Clear();
        }

        public void ChangeTurn(Colour turn)
        {
            Turn = turn;
            if (player1.Colour == turn)
            {
                player1.Timer = TIMER;
                player2.Timer = 0;
            }
            else
            {
                player2.Timer = 0;
                player1.Timer = 0;
            }
        }

        public Player GetPlayerBasedOnTurn()
        {
            if (Turn == Colour.White)
            {
                return player1;
            }
            else
            {
                return player2;
            }
        }

        public Player GetOppositePlayerBasedOnTurn()
        {
            if (Turn == Colour.White)
            {
                return player2;
            }
            else
            {
                return player1;
            }
        }

        public void swapTurn()
        {
            if (Turn == Colour.White)
            {
                ChangeTurn(Colour.Black);
            }
            else
            {
                ChangeTurn(Colour.White);
            }
        }

        public void MakeTemporaryMove(Point start, Point end){
            Player player = GetPlayerBasedOnTurn();
            Player opponent = GetOppositePlayerBasedOnTurn();

            // add current board to history of player
            player.CopyCurrentPointsToHistory();
            opponent.CopyCurrentPointsToHistory();

            player.Points[start]--;
            player.Points[end]++;

            if(opponent.Points[(Point)((int)Point.HOME - (int)end)] > 0){
                opponent.Points[(Point)((int)Point.HOME - (int)end)] = 0;
                opponent.Points[Point.OUT]++;
            }
        }

        public bool IsPlayerWinner(Player player){
            return player.Points[Point.HOME] == 15;
        }

        public bool CanDoMovement()
        {
            if (turn == player1.Colour)
            {
                return CanDoMovementByPlayer(player1, player2);
            }
            else
            {
                return CanDoMovementByPlayer(player2, player1);
            }
        }

        public bool CanDoMovementByPlayerByStartPoint(Player player, Player opponent, Point startPoint)
        {

            bool haveAnyPieceOutsideZ4 = HaveAnyPieceOutsideZ4(player);
            bool haveAnyPieceOutsideGame = HaveAnyPieceOutsideGame(player);

            if (haveAnyPieceOutsideGame)
            {
                return startPoint == Point.OUT && CanSit(player, opponent, Point.OUT);
            }
            else
            {

                if (player.Points[startPoint] > 0 && CanSit(player, opponent, startPoint))
                {
                    return true;
                }

                if (!haveAnyPieceOutsideZ4)
                {

                    if (player.Points[startPoint] > 0 && CanGoHome(player, startPoint))
                    {
                        return true;
                    }
                }

            }

            return false;
        }

        public List<Point> GetPossibleMovementsByPlayerByStartPoint(Player player, Player opponent, Point startPoint)
        {

            bool haveAnyPieceOutsideZ4 = HaveAnyPieceOutsideZ4(player);
            bool haveAnyPieceOutsideGame = HaveAnyPieceOutsideGame(player);

            var _points = new List<Point>();

            if (haveAnyPieceOutsideGame)
            {
                if (startPoint == Point.OUT)
                {
                    foreach(Dice dice in player.RemainingDices){
                        if(CanSitByGivenDice(player, opponent, startPoint, dice)){
                            _points.Add((Point)((int)startPoint + (int)dice));
                        }
                    }
                }
            }
            else
            {

                if (player.Points[startPoint] > 0 && CanSit(player, opponent, startPoint))
                {
                    foreach(Dice dice in player.RemainingDices){
                        if(CanSitByGivenDice(player, opponent, startPoint, dice)){
                            _points.Add((Point)((int)startPoint + (int)dice));
                        }                        
                    }
                }

                if (!haveAnyPieceOutsideZ4)
                {
                    if (player.Points[startPoint] > 0 && CanGoHome(player, startPoint))
                    {
                        _points.Add(Point.HOME);
                    }
                }

            }

            return _points;
        }

        public bool CanDoMovementByPlayer(Player player, Player opponent)
        {

            bool haveAnyPieceOutsideZ4 = HaveAnyPieceOutsideZ4(player);
            bool haveAnyPieceOutsideGame = HaveAnyPieceOutsideGame(player);

            if (haveAnyPieceOutsideGame)
            {
                return CanSit(player, opponent, Point.OUT);
            }
            else
            {

                for (Point c = Point.Z11; c <= Point.Z46; c++)
                {
                    if (player.Points[c] > 0 && CanSit(player, opponent, c))
                    {
                        return true;
                    }
                }
                if (!haveAnyPieceOutsideZ4)
                {
                    for (Point c = Point.Z41; c <= Point.Z46; c++)
                    {
                        if (player.Points[c] > 0 && CanGoHome(player, c))
                        {
                            return true;
                        }
                    }
                }

            }

            return false;
        }

        private bool HaveAnyPieceOutsideZ4(Player player)
        {
            for (Point i = Point.OUT; i < Point.Z41; i++)
            {
                if (player.Points[i] > 0)
                {
                    return true;
                }
            }

            return false;
        }

        private bool HaveAnyPieceOutsideGame(Player player)
        {
            return player.Points[Point.OUT] > 0;
        }

        private bool CanSit(Player player, Player opponent, Point fromPoint)
        {
            foreach (Dice dice in player.RemainingDices)
            {
                if(CanSitByGivenDice(player, opponent, fromPoint, dice)){
                    return true;
                }
            }

            return false;
        }

        private bool CanSitByGivenDice(Player player, Player opponent, Point fromPoint, Dice dice)
        {
            return opponent.Points[(int)Point.HOME - (int)dice - fromPoint] <= 1;
        }

        /// <summary>
        /// Can go home does not check canSit, and does not have access to opponent data
        /// </summary>
        /// <param name="player"></param>
        /// <param name="fromPoint"></param>
        /// <returns></returns>
        private bool CanGoHome(Player player, Point fromPoint)
        {

            foreach (Dice dice in player.RemainingDices)
            {
                bool canGo = true;
                if ((int)fromPoint + (int)dice > (int)Point.HOME)
                {
                    for (Point p = Point.Z41; p < fromPoint; p++)
                    {
                        if (player.Points[p] > 0)
                        {
                            return false;
                        }
                    }
                }

                if (canGo)
                {
                    return true;
                }
            }

            return false;
        }
    }

}
