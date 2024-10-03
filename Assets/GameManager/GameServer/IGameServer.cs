using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GameManager
{
    public abstract class IGameServer : INotifyPropertyChanged {

        public abstract void Signal(ICommand command);

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
        public event Action NewCommandSent;

        private ICommand opponentCommand;

        public ICommand OpponentCommand {
            get => opponentCommand;
            set {
                opponentCommand = value;
                NewCommandSent?.Invoke();
            }
        }
    }
}