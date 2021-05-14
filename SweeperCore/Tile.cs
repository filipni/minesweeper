using System.Collections.Generic;

namespace SweeperCore
{
    record Tile
    {
        public Position Position { get; init; }
        public TileState State { get; init; }
        public bool HasMine { get; init; }
        public int AdjacentMines { get; init; }

        public bool Cleared
        {
            get => State == TileState.Revealed || State == TileState.Exploded;
        }

        public bool Marked
        {
            get => State == TileState.Flagged || State == TileState.Questioned;
        }

        public TileImage Image
        {
            get =>
                State switch
                {
                    TileState.Hidden => TileImage.Hidden,
                    TileState.Exploded => TileImage.Exploded,
                    TileState.Flagged => TileImage.Flagged,
                    TileState.Questioned => TileImage.Questioned,
                    TileState.Revealed when HasMine => TileImage.Mine,
                    TileState.Revealed when AdjacentMines != 0 => _adjacentMinesImages[AdjacentMines],
                    TileState.Revealed when AdjacentMines == 0 => TileImage.Cleared,
                    _ => TileImage.Cleared
                };
        }

        private static readonly Dictionary<int, TileImage> _adjacentMinesImages = new Dictionary<int, TileImage>
        {
            [1] = TileImage.One,
            [2] = TileImage.Two,
            [3] = TileImage.Three,
            [4] = TileImage.Four,
            [5] = TileImage.Five,
            [6] = TileImage.Six,
            [7] = TileImage.Seven,
            [8] = TileImage.Eight
        };
    }
}