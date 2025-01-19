using Sokoban.Application.Interfaces;
using Sokoban.Core.Entities;
using Sokoban.Core.Enums;

namespace Sokoban.Application.Services
{
    public class PowerUpService : IPowerUpService
    {
        private readonly IMovementService _movementService;

        public PowerUpService(IMovementService movementService)
        {
            _movementService = movementService;
        }

        public bool UsePowerUp(Player player, PowerUpType powerUp, List<Box> boxes, List<Wall> walls)
        {
            if (!player.ActivePowerUps.Contains(powerUp))
                return false;

            switch (powerUp)
            {
                case PowerUpType.Pull:
                    return HandlePullPowerUp(player, boxes, walls);
                case PowerUpType.Push:
                    return HandleStrongPushPowerUp(player, boxes, walls);
                case PowerUpType.Sprint:
                    return HandleSprintPowerUp(player, boxes, walls);
                case PowerUpType.Throw:
                    return HandleThrowPowerUp(player, boxes, walls);
                case PowerUpType.Skateboard:
                    return HandleSkateboardPowerUp(player, boxes, walls);
                case PowerUpType.Punch:
                    return HandleStrongPunchPowerUp(player, walls);
                default:
                    return false;
            }
        }

        public void CollectPowerUp(Player player, PowerUp powerUp)
        {
            if (!powerUp.IsCollected)
            {
                powerUp.IsCollected = true;
                player.AddPowerUp(powerUp.Type);
            }
        }

        public bool IsPowerUpActive(Player player, PowerUpType powerUp)
        {
            return player.ActivePowerUps.Contains(powerUp);
        }

        private bool HandlePullPowerUp(Player player, List<Box> boxes, List<Wall> walls)
        {
            // En yakın kutuyu bul
            var nearestBox = FindNearestBox(player, boxes);
            if (nearestBox == null)
                return false;

            // Kutuyu çekme yönünü belirle
            var direction = DetermineDirection(player, nearestBox);
            return _movementService.TryPullBox(player, nearestBox, direction, boxes, walls);
        }

        private bool HandleStrongPushPowerUp(Player player, List<Box> boxes, List<Wall> walls)
        {
            // Oyuncunun önündeki kutuları kontrol et
            var direction = GetPlayerFacingDirection(player);
            return _movementService.TryStrongPush(player, boxes, direction, walls);
        }

        private bool HandleSprintPowerUp(Player player, List<Box> boxes, List<Wall> walls)
        {
            var direction = GetPlayerFacingDirection(player);
            return _movementService.TrySprintMove(player, direction, boxes, walls);
        }

        private bool HandleThrowPowerUp(Player player, List<Box> boxes, List<Wall> walls)
        {
            // En yakın kutuyu bul
            var nearestBox = FindNearestBox(player, boxes);
            if (nearestBox == null)
                return false;

            var direction = GetPlayerFacingDirection(player);
            return _movementService.TryThrowBox(player, nearestBox, direction, boxes, walls);
        }

        private bool HandleSkateboardPowerUp(Player player, List<Box> boxes, List<Wall> walls)
        {
            var direction = GetPlayerFacingDirection(player);
            return _movementService.TrySkateboardMove(player, direction, boxes, walls);
        }

        private bool HandleStrongPunchPowerUp(Player player, List<Wall> walls)
        {
            // Oyuncunun önündeki yıkılabilir duvarı bul
            var direction = GetPlayerFacingDirection(player);
            int targetX = player.X;
            int targetY = player.Y;

            switch (direction)
            {
                case Direction.Up:
                    targetY--;
                    break;
                case Direction.Down:
                    targetY++;
                    break;
                case Direction.Left:
                    targetX--;
                    break;
                case Direction.Right:
                    targetX++;
                    break;
            }

            var wall = walls.FirstOrDefault(w => w.X == targetX && w.Y == targetY && w.IsBreakable);
            if (wall == null)
                return false;

            walls.Remove(wall);
            return true;
        }

        private Box FindNearestBox(Player player, List<Box> boxes)
        {
            return boxes.OrderBy(b =>
                Math.Abs(b.X - player.X) + Math.Abs(b.Y - player.Y))
                .FirstOrDefault();
        }

        private Direction DetermineDirection(Player player, Box box)
        {
            if (Math.Abs(player.X - box.X) > Math.Abs(player.Y - box.Y))
            {
                return player.X > box.X ? Direction.Left : Direction.Right;
            }
            return player.Y > box.Y ? Direction.Up : Direction.Down;
        }

        private Direction GetPlayerFacingDirection(Player player)
        {
            // Bu metod oyunun UI katmanından gelen son hareket yönünü kullanmalı
            // Şimdilik varsayılan olarak Right döndürüyoruz
            return Direction.Right;
        }
    }
}
