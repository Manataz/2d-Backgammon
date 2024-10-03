using System.Collections.Generic;
using UnityEngine.Animations;

namespace GameManager
{
    public class DragStartCommand : ICommand
    {

        Point startPoint;

        public DragStartCommand(int number, Point point) : base(Commands.DRAG_START, ++number)
        {
            startPoint = point;
        }

        protected override Intent Action(Board board)
        {

            Player player = board.GetPlayerBasedOnTurn();
            Player opposite = board.GetOppositePlayerBasedOnTurn();

            player.DragStartPoint = startPoint;
            player.DragEndPoint = null;

            if (board.CanDoMovementByPlayerByStartPoint(player, opposite, startPoint))
            {
                player.IsDragging = true;

                // update highlight boards
                board.HighlightedPoints = board.GetPossibleMovementsByPlayerByStartPoint(player, opposite, startPoint);
            }
            else
            {
                player.DragEndPoint = startPoint;
            }

            return new NoActionIntent(board);
        }

        protected override bool isValid(Board board)
        {
            if (board.ProgressStatus != ProgressStatus.WAIT_FOR_MOVE)
            {
                return false;
            }

            Player player = board.GetPlayerBasedOnTurn();

            if (player.Points[startPoint] == 0)
            {
                return false;
            }

            if (player.History.Count == player.RolledDices.Count)
            {
                return false;
            }

            if (player.IsDragging)
            {
                return false;
            }

            return true;
        }

    }
}