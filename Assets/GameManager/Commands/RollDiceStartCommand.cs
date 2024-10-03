using UnityEngine.Animations;

namespace GameManager
{
    public class RollDiceStartCommand : ICommand
    {

        private readonly Colour colour;

        public RollDiceStartCommand(int number, Colour colour) : base(Commands.ROLL_DICE_START, ++number)
        {
            this.colour = colour;
        }

        protected override Intent Action(Board board)
        {
            Player playerFromBoard;
            if(board.Player1.Colour == colour){
                playerFromBoard = board.Player1;
            }else{
                playerFromBoard = board.Player2;
            }

            playerFromBoard.RollStartDice();

            return new RollDiceStartIntent(board, playerFromBoard, playerFromBoard.StartDice);
        }

        protected override bool isValid(Board board)
        {
            if(board.ProgressStatus != ProgressStatus.JUST_STARTED){
                return false;
            }

            if(board.Turn != null){
                return false;
            }

            return true;
        }

    }
}