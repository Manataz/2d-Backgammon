using UnityEngine.Animations;

namespace GameManager
{
    public class StartCommand : ICommand
    {

        public StartCommand(int number) : base(Commands.START, ++number)
        {

        }

        protected override Intent Action(Board board)
        {
            Colour turn;
            if(board.Player1.StartDice > board.Player2.StartDice){
                turn = board.Player1.Colour;
            }else{
                turn = board.Player2.Colour;
            }

            board.ChangeTurn(turn);
            board.ProgressStatus = ProgressStatus.NORMAL_PROGRESS;

            return new StartIntent(board);
        }

        protected override bool isValid(Board board)
        {
            if(board.ProgressStatus != ProgressStatus.JUST_STARTED){
                return false;
            }

            if(board.Turn != null){
                return false;
            }

            if(board.Player1.StartDice == null || board.Player2.StartDice == null){
                return false;
            }
            
            if(board.Player1.StartDice == board.Player2.StartDice){
                return false;
            }

            return true;
        }

    }
}