using UnityEngine.Animations;

namespace GameManager
{
    public class DropCommand : ICommand
    {

        Point? endPoint = null;

        public DropCommand(int number, Point? point) : base(Commands.DROP, ++number)
        {
            endPoint = point;
        }

        protected override Intent Action(Board board)
        {

            Player player = board.GetPlayerBasedOnTurn();

            player.DragEndPoint = endPoint;
            player.IsDragging = false;

            bool isPossibleMovement = false;
            foreach (Point p in board.HighlightedPoints)
            {
                if (p == endPoint)
                {
                    isPossibleMovement = true;
                }
            }

            board.HighlightedPoints.Clear();

            if (endPoint != null && isPossibleMovement)
            {
                // remove from remaining dice
                player.ReduceRemainingDice((int)endPoint - (int)player.DragStartPoint);
                // perform move
                board.MakeTemporaryMove(player.DragStartPoint, (Point) endPoint);

                // send intent
                return new DropIntent(board, (Point)endPoint);
            }
            else
            {
                // undo drag
                // send intent
                return new UndoDragIntent(board);
            }
        }

        protected override bool isValid(Board board)
        {
            if (board.ProgressStatus != ProgressStatus.WAIT_FOR_MOVE)
            {
                return false;
            }

            Player player = board.GetPlayerBasedOnTurn();

            if (player.IsDragging == false)
            {
                return false;
            }

            return true;
        }
    }
}