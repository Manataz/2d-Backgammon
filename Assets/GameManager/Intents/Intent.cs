namespace GameManager
{
    public enum Intents
    {
        COMMIT,
        UNDO_DRAG,
        DROP,
        ROLL_DICE,
        ROLL_DICE_START,
        NO_ACTION,
        RESET_ROLL_DICE_START,
    }

    public class Intent {
        private Intents name;

        public Intents Name { get => name; }

        private bool mustSignal = false;

        public bool MustSignal {
            get => mustSignal;
            set {
                mustSignal = value;
            }
        }

        private Board board;

        public Board Board { get => board; }

        public Intent(Intents _name, Board _board){
            name = _name;
            board = _board;
        }

    }
}