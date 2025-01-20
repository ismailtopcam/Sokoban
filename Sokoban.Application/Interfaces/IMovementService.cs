using Sokoban.Core.Entities;
using Sokoban.Core.Enums;

public interface IMovementService
{
    bool TryMovePlayer(Player player, Direction direction, List<Box> boxes, List<Wall> walls);
    bool TryPullBox(Player player, Direction direction, List<Box> boxes, List<Wall> walls);
    bool TryStrongPush(Player player, List<Box> boxes, Direction direction, List<Wall> walls);
    bool TrySprintMove(Player player, Direction direction, List<Box> boxes, List<Wall> walls);
    bool TryThrowBox(Player player, Box box, Direction direction, List<Box> boxes, List<Wall> walls);
    bool TrySkateboardMove(Player player, Direction direction, List<Box> boxes, List<Wall> walls);
}