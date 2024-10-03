using System;
using System.Collections.Generic;

namespace GameManager
{

    public class Player : DataModel
    {

        private Colour colour;

        public Colour Colour
        {
            get => colour;
            set
            {
                colour = value;
                OnPropertyChanged();
            }
        }

        private Dice? startDice;

        public Dice? StartDice
        {
            get => startDice;
            set
            {
                startDice = value;
                OnPropertyChanged();
            }
        }

        private List<Dice> rolledDices;

        public List<Dice> RolledDices
        {
            get => rolledDices;
            set
            {
                rolledDices = value;
                OnPropertyChanged();
            }
        }

        private List<Dice> remainingDices;

        public List<Dice> RemainingDices
        {
            get => remainingDices;
            set
            {
                remainingDices = value;
                OnPropertyChanged();
            }
        }

        private string name;

        public string Name
        {
            get => name;
            set
            {
                name = value;
                OnPropertyChanged();
            }
        }

        private int coins;

        public int Coins
        {
            get => coins;
            set
            {
                coins = value;
                OnPropertyChanged();
            }
        }

        private string playerClass;

        public string PlayerClass
        {
            get => playerClass;
            set
            {
                playerClass = value;
                OnPropertyChanged();
            }
        }

        private int timer;

        public int Timer
        {
            get => timer;
            set
            {
                timer = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Represents number of points in each column
        /// </summary>
        private Dictionary<Point, int> points;

        /// <summary>
        /// Represents number of points in each column
        /// </summary>
        public Dictionary<Point, int> Points
        {
            get => points;
            set
            {
                points = value;
                OnPropertyChanged();
            }
        }

        private bool isDragging = false;

        public bool IsDragging
        {
            get => isDragging;
            set
            {
                isDragging = value;
                OnPropertyChanged();
            }
        }

        private Point dragStartPoint;

        public Point DragStartPoint
        {
            get => dragStartPoint;
            set
            {
                dragStartPoint = value;
                OnPropertyChanged();
            }
        }

        private Point? dragEndPoint;
        public Point? DragEndPoint
        {
            get => dragEndPoint;
            set
            {
                dragEndPoint = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Keeps the history of player moves in one turn, in case of undo or commit. 
        /// Each record represents the whole board and both player's history shoud get updated simultanously.
        /// </summary>
        private List<Dictionary<Point, int>> history;

        /// <summary>
        /// Keeps the history of player moves in one turn, in case of undo or commit. 
        /// Each record represents the whole board and both player's history shoud get updated simultanously.
        /// </summary>
        public List<Dictionary<Point, int>> History
        {
            get => history;
            set
            {
                history = value;
                OnPropertyChanged();
            }
        }

        private DoubleRequestStatus doubleRequestStatus;

        public DoubleRequestStatus DoubleRequestStatus
        {
            get => doubleRequestStatus;
            set
            {
                doubleRequestStatus = value;
                OnPropertyChanged();
            }
        }

        private DoubleResponseStatus doubleResponseStatus;

        public DoubleResponseStatus DoubleResponseStatus
        {
            get => doubleResponseStatus;
            set
            {
                doubleResponseStatus = value;
                OnPropertyChanged();
            }
        }

        private static Random random = new Random();

        public Player(Colour colour)
        {
            rolledDices = new List<Dice>();
            remainingDices = new List<Dice>();
            history = new List<Dictionary<Point, int>>();
            this.colour = colour;

            points = new Dictionary<Point, int>();
            points[Point.OUT] = 0;
            points[Point.Z11] = 2;
            points[Point.Z12] = 0;
            points[Point.Z13] = 0;
            points[Point.Z14] = 0;
            points[Point.Z15] = 0;
            points[Point.Z16] = 0;
            points[Point.Z21] = 0;
            points[Point.Z22] = 0;
            points[Point.Z23] = 0;
            points[Point.Z24] = 0;
            points[Point.Z25] = 0;
            points[Point.Z26] = 5;
            points[Point.Z31] = 0;
            points[Point.Z32] = 0;
            points[Point.Z33] = 0;
            points[Point.Z34] = 0;
            points[Point.Z35] = 3;
            points[Point.Z36] = 0;
            points[Point.Z41] = 5;
            points[Point.Z42] = 0;
            points[Point.Z43] = 0;
            points[Point.Z44] = 0;
            points[Point.Z45] = 0;
            points[Point.HOME] = 0;

            timer = 0;
            isDragging = false;
            startDice = null;
            doubleRequestStatus = DoubleRequestStatus.NO_REQUEST;
            doubleResponseStatus = DoubleResponseStatus.NO_REQUEST;

        }

        public void RollStartDice()
        {
            Array values = Enum.GetValues(typeof(Dice));
            StartDice = (Dice)values.GetValue(random.Next(values.Length));
        }

        public void RollDice()
        {
            Array values = Enum.GetValues(typeof(Dice));

            Dice firstDice = (Dice)values.GetValue(random.Next(values.Length));
            Dice secondDice = (Dice)values.GetValue(random.Next(values.Length));

            rolledDices.Clear();
            rolledDices.Add(firstDice);
            rolledDices.Add(secondDice);

            remainingDices.Clear();
            remainingDices.Add(firstDice);
            remainingDices.Add(secondDice);

            if (firstDice == secondDice)
            {
                rolledDices.Add(firstDice);
                rolledDices.Add(firstDice);
                remainingDices.Add(firstDice);
                remainingDices.Add(firstDice);
            }

            OnPropertyChanged(nameof(RolledDices));
        }

        public void ReduceRemainingDice(int diceNumber)
        {

            // find dice with the same number
            foreach (Dice dice in RemainingDices)
            {
                if (diceNumber == (int)dice)
                {
                    RemainingDices.Remove(dice);
                    return;
                }
            }

            // if no dice found with the same number
            // remove the biggest one
            Dice maxDice = RemainingDices[0];
            foreach (Dice dice in RemainingDices)
            {
                if (dice > maxDice)
                {
                    maxDice = dice;
                }
            }

            RemainingDices.Remove(maxDice);

            OnPropertyChanged(nameof(RemainingDices));
        }

        /// <summary>
        /// It'll copy new player data to existing player
        /// TODO: it can be optimised to check that only set some items if they are changed
        /// </summary>
        /// <param name="newPlayer"></param>
        public void Copy(Player newPlayer)
        {
            StartDice = newPlayer.StartDice;
            RolledDices = newPlayer.RolledDices;
            RemainingDices = newPlayer.RemainingDices;
            Name = newPlayer.Name;
            Coins = newPlayer.Coins;
            PlayerClass = newPlayer.PlayerClass;
            Timer = newPlayer.Timer;
            Points = newPlayer.Points;
            IsDragging = newPlayer.IsDragging;
            DragStartPoint = newPlayer.DragStartPoint;
            DragEndPoint = newPlayer.dragEndPoint;
            History = newPlayer.History;
            DoubleRequestStatus = newPlayer.DoubleRequestStatus;
            DoubleResponseStatus = newPlayer.DoubleResponseStatus;
        }

        public void CopyCurrentPointsToHistory()
        {
            Dictionary<Point, int> copy = new Dictionary<Point, int>(Points);
            History.Add(copy);
        }

        public void RevertHistory()
        {
            Points = new Dictionary<Point, int>(History[History.Count - 1]);
            History.RemoveAt(History.Count - 1);
        }
    }
}
