namespace GameManager
{
    public class DropIntent : Intent {

        private Point point;

        public Point Point {
            get => point;
        }

        public DropIntent(Board board, Point point) : base(Intents.DROP, board) {
            this.point = point;
        }
    }
}