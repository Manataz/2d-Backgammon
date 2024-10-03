using System.Collections.Generic;

namespace GameManager
{
    public class RollDiceIntent : Intent {

        private List<Dice> dices;

        public List<Dice> Dice { get => dices; }

        public RollDiceIntent(Board board, List<Dice> dices) : base(Intents.ROLL_DICE, board) {
            this.dices = dices;

            MustSignal = true;
        }
    }
}