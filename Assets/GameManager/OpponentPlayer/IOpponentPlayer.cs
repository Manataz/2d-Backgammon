using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GameManager
{
    public abstract class IOpponentPlayer {
        public abstract void RequestCommand(Board board);
    }
}