using UnityEngine.Animations;

namespace GameManager
{
    public class AcceptDoubleCommand : ICommand
    {

        public AcceptDoubleCommand(int number) : base(Commands.ACCEPT_DOUBLE, ++number)
        {

        }

        protected override Intent Action(Board board)
        {
            board.DoubleCoins = board.DoubleCoins == 0 ? 2 : board.DoubleCoins * 2;
            board.ProgressStatus = ProgressStatus.NORMAL_PROGRESS;
            board.DoubleTurn = board.DoubleTurn == Colour.White ? Colour.Black : Colour.White;

            return new NoActionIntent(board, true);
        }

        protected override bool isValid(Board board)
        {
            if(board.ProgressStatus != ProgressStatus.DOUBLE_REQUEST){
                return false;
            }

            if(board.DoubleTurn == null){
                return false;
            }

            return true;
        }

    }
}