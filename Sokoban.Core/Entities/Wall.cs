namespace Sokoban.Core.Entities
{
    public class Wall
    {
        public int X { get; }
        public int Y { get; }
        public bool IsBreakable { get; }

        public Wall(int x, int y, bool isBreakable = false)
        {
            X = x;
            Y = y;
            IsBreakable = isBreakable;
        }
    }
}
