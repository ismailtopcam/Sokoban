using Sokoban.Application.Interfaces;
using Sokoban.Core.Entities;
using Sokoban.Core.Enums;

namespace Sokoban.Application.Services
{
    public class MovementService : IMovementService
    {
        public bool TryMovePlayer(Player player, Direction direction, List<Box> boxes, List<Wall> walls)
        {
            int newX = player.X;
            int newY = player.Y;

            // Yeni pozisyonu hesapla
            UpdatePosition(ref newX, ref newY, direction);

            // Duvar kontrolü
            if (walls.Any(w => w.X == newX && w.Y == newY))
                return false;

            // Kutu kontrolü ve itme
            var box = boxes.FirstOrDefault(b => b.X == newX && b.Y == newY);
            if (box != null)
            {
                if (!CanMoveBox(box, direction, boxes, walls))
                    return false;

                box.Move(direction);
            }

            player.Move(direction);
            return true;
        }

        public bool CanMoveBox(Box box, Direction direction, List<Box> boxes, List<Wall> walls)
        {
            int newX = box.X;
            int newY = box.Y;

            UpdatePosition(ref newX, ref newY, direction);

            // Duvar veya diğer kutu kontrolü
            return !walls.Any(w => w.X == newX && w.Y == newY) &&
                   !boxes.Any(b => b.X == newX && b.Y == newY);
        }

        public bool TryPullBox(Player player, Box box, Direction direction, List<Box> boxes, List<Wall> walls)
        {
            if (!player.CanPull)
                return false;

            // Oyuncunun arkasındaki kutuyu kontrol et
            int behindX = player.X;
            int behindY = player.Y;
            UpdatePosition(ref behindX, ref behindY, GetOppositeDirection(direction));

            var boxToPull = boxes.FirstOrDefault(b => b.X == behindX && b.Y == behindY);
            if (boxToPull == null)
                return false;

            // Oyuncunun yeni pozisyonunu kontrol et
            int newPlayerX = player.X;
            int newPlayerY = player.Y;
            UpdatePosition(ref newPlayerX, ref newPlayerY, direction);

            if (walls.Any(w => w.X == newPlayerX && w.Y == newPlayerY) ||
                boxes.Any(b => b.X == newPlayerX && b.Y == newPlayerY && b != boxToPull))
                return false;

            // Hareketi gerçekleştir
            player.Move(direction);
            boxToPull.Move(direction);
            return true;
        }

        public bool TryStrongPush(Player player, List<Box> boxes, Direction direction, List<Wall> walls)
        {
            if (!player.CanStrongPush)
                return false;

            int frontX = player.X;
            int frontY = player.Y;
            UpdatePosition(ref frontX, ref frontY, direction);

            // İlk kutuyu bul
            var firstBox = boxes.FirstOrDefault(b => b.X == frontX && b.Y == frontY);
            if (firstBox == null)
                return false;

            // İkinci kutu pozisyonunu kontrol et
            int secondX = frontX;
            int secondY = frontY;
            UpdatePosition(ref secondX, ref secondY, direction);

            var secondBox = boxes.FirstOrDefault(b => b.X == secondX && b.Y == secondY);
            if (secondBox == null)
                return false;

            // İkinci kutudan sonraki pozisyonu kontrol et
            int finalX = secondX;
            int finalY = secondY;
            UpdatePosition(ref finalX, ref finalY, direction);

            if (walls.Any(w => w.X == finalX && w.Y == finalY) ||
                boxes.Any(b => b.X == finalX && b.Y == finalY && b != firstBox && b != secondBox))
                return false;

            // Kutuları hareket ettir
            secondBox.Move(direction);
            firstBox.Move(direction);
            player.Move(direction);
            return true;
        }

        public bool TrySprintMove(Player player, Direction direction, List<Box> boxes, List<Wall> walls)
        {
            if (!player.CanSprint)
                return false;

            int newX = player.X;
            int newY = player.Y;
            UpdatePosition(ref newX, ref newY, direction, 2); // 2 kare hareket

            // Yol üzerinde engel kontrolü
            int intermediateX = player.X;
            int intermediateY = player.Y;
            UpdatePosition(ref intermediateX, ref intermediateY, direction);

            if (walls.Any(w => (w.X == intermediateX && w.Y == intermediateY) || (w.X == newX && w.Y == newY)) ||
                boxes.Any(b => (b.X == intermediateX && b.Y == intermediateY) || (b.X == newX && b.Y == newY)))
                return false;

            player.X = newX;
            player.Y = newY;
            return true;
        }

        public bool TryThrowBox(Player player, Box box, Direction direction, List<Box> boxes, List<Wall> walls)
        {
            if (!player.CanThrow)
                return false;

            // Fırlatılacak kutunun oyuncunun yanında olduğunu kontrol et
            if (!IsAdjacent(player, box))
                return false;

            // Hedef pozisyonu hesapla (2 kare öte)
            int targetX = box.X;
            int targetY = box.Y;
            UpdatePosition(ref targetX, ref targetY, direction, 2);

            // Hedef pozisyonda engel kontrolü
            if (walls.Any(w => w.X == targetX && w.Y == targetY) ||
                boxes.Any(b => b.X == targetX && b.Y == targetY && b != box))
                return false;

            // Kutuyu fırlat
            box.X = targetX;
            box.Y = targetY;
            return true;
        }

        public bool TrySkateboardMove(Player player, Direction direction, List<Box> boxes, List<Wall> walls)
        {
            if (!player.CanSkateboard)
                return false;

            int currentX = player.X;
            int currentY = player.Y;

            while (true)
            {
                int nextX = currentX;
                int nextY = currentY;
                UpdatePosition(ref nextX, ref nextY, direction);

                // Engel kontrolü
                if (walls.Any(w => w.X == nextX && w.Y == nextY) ||
                    boxes.Any(b => b.X == nextX && b.Y == nextY))
                    break;

                currentX = nextX;
                currentY = nextY;
            }

            if (currentX == player.X && currentY == player.Y)
                return false;

            player.X = currentX;
            player.Y = currentY;
            return true;
        }

        private void UpdatePosition(ref int x, ref int y, Direction direction, int steps = 1)
        {
            switch (direction)
            {
                case Direction.Up:
                    y -= steps;
                    break;
                case Direction.Down:
                    y += steps;
                    break;
                case Direction.Left:
                    x -= steps;
                    break;
                case Direction.Right:
                    x += steps;
                    break;
            }
        }

        private Direction GetOppositeDirection(Direction direction)
        {
            return direction switch
            {
                Direction.Up => Direction.Down,
                Direction.Down => Direction.Up,
                Direction.Left => Direction.Right,
                Direction.Right => Direction.Left,
                _ => throw new ArgumentException("Invalid direction")
            };
        }

        private bool IsAdjacent(Player player, Box box)
        {
            return (Math.Abs(player.X - box.X) == 1 && player.Y == box.Y) ||
                   (Math.Abs(player.Y - box.Y) == 1 && player.X == box.X);
        }
    }
}
