using UnityEngine.Animations;

namespace GameManager
{
    public class ResetRollDiceStartCommand : ICommand
    {

        public ResetRollDiceStartCommand(int number) : base(Commands.RESET_ROLL_DICE_START, ++number)
        {
        }

        protected override Intent Action(Board board)
        {
            board.Turn = null;
            board.Player1.StartDice = null;
            board.Player2.StartDice = null;

            return new ResetRollDiceIntent(board);
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