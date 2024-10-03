using UnityEngine.Animations;

namespace GameManager
{
    public class CommitCommand : ICommand
    {

        public CommitCommand(int number) : base(Commands.COMMIT, ++number)
        {

        }

        protected override Intent Action(Board board)
        {
            board.ToReadyState();

            Player player = board.GetPlayerBasedOnTurn();

            // check winner
            if(board.IsPlayerWinner(player)){
                board.Winner = board.Turn;
                board.FinishStatus = FinishStatus.FinishGame;
            }else{
                board.swapTurn();
            }

            return new CommitIntent(board);
        }

        protected override bool isValid(Board board)
        {
            if (board.ProgressStatus != ProgressStatus.WAIT_FOR_MOVE)
            {
                return false;
            }

            if(board.CanDoMovement()){
                return false;
            }

            return true;
        }
    }
}