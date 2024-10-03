using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace GameManager
{

    public class BackgammonGame : INotifyPropertyChanged
    {

        private Board board;

        public Board Board
        {
            get => board;
        }

        private Intent intent = null;

        public Intent Intent
        {
            get => intent;
            set
            {
                intent = value;
                OnPropertyChanged();
            }
        }

        private bool testRun;

        public bool TestRun
        {
            get => testRun;
        }

        private List<ICommand> moveHistory;

        private int gameTime;

        public int GameTime
        {
            get => gameTime;
            set
            {
                gameTime = value;
                OnPropertyChanged();
            }
        }

        private IGameServer gameServer;

        public BackgammonGame(IGameServer gameServer, bool testRun)
        {
            board = new Board();
            this.gameServer = gameServer;

            moveHistory = new List<ICommand>();
            this.testRun = testRun;

            // attach GameServer CommanderListener
            this.gameServer.NewCommandSent += OnGameServerCommandSent;
        }

        private void OnGameServerCommandSent()
        {
            ExecuteCommand(gameServer.OpponentCommand);
        }

        public void ExecuteCommand(ICommand command)
        {

            moveHistory.Add(command);

            // make a copy of current board and safely send into intent
            Intent intent = command.Run(new Board(board));

            if (intent.MustSignal)
            {
                gameServer.Signal(command);
            }

            Intent = intent;

            if (testRun || intent.Name == Intents.NO_ACTION)
            {
                UpdateBoard(intent.Board);
            }

        }

        public void UpdateBoard(Board board)
        {
            this.board.Copy(board);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Distructor, deattach listeners
        /// </summary>
        ~BackgammonGame()
        {
            gameServer.NewCommandSent -= OnGameServerCommandSent;
        }

    }

}

