using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using GuiVersion.Commands;
using SweeperCore;

namespace GuiVersion.ViewModels
{
    public class TileViewModel : INotifyPropertyChanged
    {
        public Position Position { get; }
        public TileClickedCommand TileClickedCommand { get; }
        public TileRightClickedCommand TileRightClickedCommand { get; }

        public string ImagePath => $"../../../{_symbolTable[_image]}";

        private TileImage _image;
        public TileImage Image
        { 
            get => _image;
            set
            {
                _image = value;
                OnPropertyChanged(nameof(Image));
                OnPropertyChanged(nameof(ImagePath));
            }
        }

        private readonly Dictionary<TileImage, string> _symbolTable = new Dictionary<TileImage, string>
                                                                    {
                                                                        [TileImage.Hidden] = "Images/empty.png",
                                                                        [TileImage.Cleared] = "Images/empty.png",
                                                                        [TileImage.Mine] = "Images/mine.png",
                                                                        [TileImage.Exploded] = "Images/explosion.png",
                                                                        [TileImage.Flagged] = "Images/flag.png",
                                                                        [TileImage.Questioned] = "Images/question.png",
                                                                        [TileImage.One] = "Images/number-1.png",
                                                                        [TileImage.Two] = "Images/number-2.png",
                                                                        [TileImage.Three] = "Images/number-3.png",
                                                                        [TileImage.Four] = "Images/number-4.png",
                                                                        [TileImage.Five] = "Images/number-5.png",
                                                                        [TileImage.Six] = "Images/number-6.png",
                                                                        [TileImage.Seven] = "Images/number-7.png",
                                                                        [TileImage.Eight] = "Images/number-8.png"

                                                                    };


        public TileViewModel(TileImage image, Position position, ViewModel vm)
            => (Image, Position, TileClickedCommand, TileRightClickedCommand) = (image, position, new TileClickedCommand(vm), new TileRightClickedCommand(vm, this));

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
