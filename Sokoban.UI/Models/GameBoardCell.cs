using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Sokoban.UI.Models
{
    public class GameBoardCell : INotifyPropertyChanged
    {
        private string _imageSource;
        public string ImageSource
        {
            get => _imageSource;
            set
            {
                if (_imageSource != value)
                {
                    _imageSource = value;
                    Debug.WriteLine($"Setting image source: {value}");
                    OnPropertyChanged();
                }
            }
        }

        public GameBoardCell()
        {
            ImageSource = "pack://application:,,,/Sokoban.UI;component/Images/floor.png";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
