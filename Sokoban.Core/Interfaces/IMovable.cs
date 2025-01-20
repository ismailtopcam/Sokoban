using Sokoban.Core.Enums;

namespace Sokoban.Core.Interfaces
{
    public interface IMovable
    {
        bool CanMove(Direction direction);
        void Move(Direction direction);
    }
}
