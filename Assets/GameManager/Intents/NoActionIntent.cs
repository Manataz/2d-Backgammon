namespace GameManager
{
    public class NoActionIntent : Intent {

        public NoActionIntent(Board board, bool mustSignal = false) : base(Intents.NO_ACTION, board) {
            MustSignal = mustSignal;
        }
    }
}