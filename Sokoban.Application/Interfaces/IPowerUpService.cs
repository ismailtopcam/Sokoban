using Sokoban.Core.Entities;
using Sokoban.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sokoban.Application.Interfaces
{
    public interface IPowerUpService
    {
        bool UsePowerUp(Player player, PowerUpType powerUp, List<Box> boxes, List<Wall> walls);
        void CollectPowerUp(Player player, PowerUp powerUp);
        bool IsPowerUpActive(Player player, PowerUpType powerUp);
    }
}
