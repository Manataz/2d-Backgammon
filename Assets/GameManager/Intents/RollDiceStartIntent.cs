namespace GameManager
{
    public class RollDiceStartIntent : Intent {

        private Player player;

        public Player Player { get => player; }

        private Dice? dice;

        public Dice? Dice { get => dice; }

        public RollDiceStartIntent(Board board, Player player, Dice? dice) : base(Intents.ROLL_DICE_START, board) {
            this.player = player;
            this.dice = dice;

            MustSignal = true;
        }
    }
}