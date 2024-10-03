using System;
using System.Threading.Tasks;

namespace GameManager
{

    public enum Commands {
        COMMIT,
        DRAG_START,
        UNDO,
        DROP,
        ROLL_DICE,
        REQUEST_DOUBLE,
        ACCEPT_DOUBLE,
        RESIGN,
        ROLL_DICE_START,
        RESET_ROLL_DICE_START,
        START
    }

    public abstract class ICommand {
        private Commands name;

        /// <summary>
        /// The number shows the order of commands, every new command should increment the last number
        /// </summary>
        private int number;

        public Commands Name { get => name; }

        protected abstract Intent Action(Board board);

        protected abstract bool isValid(Board board);

        public bool isAvailable(Board board)
        {
            return isValid(board);
        }

        public Intent Run(Board board){

            if(isValid(board)){
                return Action(board);
            }

            return new NoActionIntent(board);
        }

        public ICommand(Commands name, int number){
            this.name = name;
            this.number = number;
        }
    }
}