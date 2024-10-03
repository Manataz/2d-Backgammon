using UnityEngine.Animations;

namespace GameManager
{

    public class RollDiceCommand : ICommand
    {

        private readonly Colour colour;

        public RollDiceCommand(int number, Colour colour) : base(Commands.ROLL_DICE, ++number)
        {
            this.colour = colour;
        }

        protected override Intent Action(Board board)
        {
            Player playerFromBoard;
            if (board.Player1.Colour == board.Turn)
            {
                playerFromBoard = board.Player1;
            }
            else
            {
                playerFromBoard = board.Player2;
            }

            playerFromBoard.RollDice();

            // check if player can move wait for move, if can't change turn
            if(board.CanDoMovement()){
                board.ProgressStatus = ProgressStatus.WAIT_FOR_MOVE;
            }else{
                board.ProgressStatus = ProgressStatus.NORMAL_PROGRESS;
                board.swapTurn();
            }

            return new RollDiceIntent(board, playerFromBoard.RolledDices);
        }

        protected override bool isValid(Board board)
        {
            if (board.ProgressStatus != ProgressStatus.NORMAL_PROGRESS)
            {
                return false;
            }

            if (board.Turn != colour)
            {
                return false;
            }

            return true;
        }

    }
}