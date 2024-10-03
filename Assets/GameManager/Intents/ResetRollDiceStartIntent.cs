namespace GameManager
{
    public class ResetRollDiceIntent : Intent {

        public ResetRollDiceIntent(Board board) : base(Intents.RESET_ROLL_DICE_START, board) {

        }
    }
}