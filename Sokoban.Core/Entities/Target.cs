namespace Sokoban.Core.Entities
{
    public class Target
    {
        public int X { get; }
        public int Y { get; }
        public bool IsOccupied { get; set; }

        public Target(int x, int y)
        {
            X = x;
            Y = y;
            IsOccupied = false;
        }
    }
}
