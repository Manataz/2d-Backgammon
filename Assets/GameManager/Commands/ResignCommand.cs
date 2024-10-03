using UnityEngine.Animations;

namespace GameManager
{
    public class ResignCommand : ICommand
    {

        private readonly Colour colour;
        private readonly FinishStatus finishStatus;

        public ResignCommand(int number, Colour colour, FinishStatus finishStatus) : base(Commands.RESIGN, ++number)
        {
            this.colour = colour;
            this.finishStatus = finishStatus;
        }

        protected override Intent Action(Board board)
        {
            if(board.Player1.Colour == colour){
                board.Winner = board.Player2.Colour;
            }else{
                board.Winner = board.Player1.Colour;
            }

            board.FinishStatus = finishStatus;

            return new NoActionIntent(board, true);
        }

        protected override bool isValid(Board board)
        {
            return true;
        }

    }
}