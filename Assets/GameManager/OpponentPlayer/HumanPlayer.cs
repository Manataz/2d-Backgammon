using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GameManager
{
    public class HumanPlayer : IOpponentPlayer
    {
        public override void RequestCommand(Board board)
        {
            throw new NotImplementedException();
        }
    }
}