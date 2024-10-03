namespace GameManager
{
    public class UndoDragIntent : Intent {
        public UndoDragIntent(Board board) : base(Intents.UNDO_DRAG, board) {
        }
    }
}