using UnityEngine.Animations;

namespace GameManager
{
    public class RequestDoubleCommand : ICommand
    {

        public RequestDoubleCommand(int number) : base(Commands.REQUEST_DOUBLE, ++number)
        {

        }

        protected override Intent Action(Board board)
        {
            board.ProgressStatus = ProgressStatus.DOUBLE_REQUEST;
            board.DoubleTurn = board.Turn;

            return new NoActionIntent(board, true);
        }

        protected override bool isValid(Board board)
        {
            if(board.ProgressStatus != ProgressStatus.NORMAL_PROGRESS){
                return false;
            }

            if(board.DoubleTurn != null && board.DoubleTurn != board.Turn){
                return false;
            }

            return true;
        }
    }
}