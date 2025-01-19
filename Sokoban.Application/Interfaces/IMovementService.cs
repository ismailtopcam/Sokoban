using Sokoban.Core.Entities;
using Sokoban.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sokoban.Application.Interfaces
{
    public interface IMovementService
    {
        bool TryMovePlayer(Player player, Direction direction, List<Box> boxes, List<Wall> walls);
        bool CanMoveBox(Box box, Direction direction, List<Box> boxes, List<Wall> walls);
        bool TryPullBox(Player player, Box box, Direction direction, List<Box> boxes, List<Wall> walls);
        bool TryStrongPush(Player player, List<Box> boxes, Direction direction, List<Wall> walls);
        bool TrySprintMove(Player player, Direction direction, List<Box> boxes, List<Wall> walls);
        bool TryThrowBox(Player player, Box box, Direction direction, List<Box> boxes, List<Wall> walls);
        bool TrySkateboardMove(Player player, Direction direction, List<Box> boxes, List<Wall> walls);
    }
}
