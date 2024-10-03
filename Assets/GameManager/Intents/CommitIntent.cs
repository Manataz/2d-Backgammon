namespace GameManager
{
    public class CommitIntent : Intent {

        public CommitIntent(Board board) : base(Intents.COMMIT, board) {
            MustSignal = true;
        }
    }
}