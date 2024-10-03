using UnityEngine.Animations;

namespace GameManager
{
    public class UndoCommand : ICommand
    {


        public UndoCommand(int number) : base(Commands.UNDO, ++number)
        {
        }

        protected override Intent Action(Board board)
        {
            board.Player1.RevertHistory();
            board.Player2.RevertHistory();

            return new NoActionIntent(board);
        }

        protected override bool isValid(Board board)
        {
            if (board.ProgressStatus != ProgressStatus.WAIT_FOR_MOVE)
            {
                return false;
            }

            Player player = board.GetPlayerBasedOnTurn();

            if (player.IsDragging == true)
            {
                return false;
            }

            if(player.History.Count == 0){
                return false;
            }

            return true;
        }
        
    }
}