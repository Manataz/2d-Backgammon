namespace GameManager
{
    public class StartIntent : Intent {

        public StartIntent(Board board) : base(Intents.NO_ACTION, board) {
            MustSignal = true;
        }
    }
}