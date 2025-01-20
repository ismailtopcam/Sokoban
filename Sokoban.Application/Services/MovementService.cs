using Sokoban.Core.Entities;
using Sokoban.Core.Enums;
using System.Diagnostics;

namespace Sokoban.Application.Services
{
    public class MovementService : IMovementService
    {
        public bool TryMovePlayer(Player player, Direction direction, List<Box> boxes, List<Wall> walls)
        {
            int newX = player.X;
            int newY = player.Y;

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

        public bool TryPullBox(Player player, Direction direction, List<Box> boxes, List<Wall> walls)
        {
            Debug.WriteLine($"Attempting to pull box. Player at ({player.X}, {player.Y})");

            // Önce oyuncunun etrafındaki en yakın kutuyu bul
            var nearestBox = FindNearestBox(player, boxes);
            if (nearestBox == null)
            {
                Debug.WriteLine("No box found near player");
                return false;
            }

            // Kutunun oyuncuya göre yönünü belirle
            Direction pullDirection = DeterminePullDirection(player, nearestBox);
            Debug.WriteLine($"Determined pull direction: {pullDirection}");

            // Oyuncunun gideceği pozisyonu hesapla (kutunun tersi yönünde)
            int newPlayerX = player.X;
            int newPlayerY = player.Y;

            switch (pullDirection)
            {
                case Direction.Up:
                    newPlayerY--;
                    break;
                case Direction.Down:
                    newPlayerY++;
                    break;
                case Direction.Left:
                    newPlayerX--;
                    break;
                case Direction.Right:
                    newPlayerX++;
                    break;
            }

            Debug.WriteLine($"Checking if player can move to ({newPlayerX}, {newPlayerY})");

            // Hareket edilecek yerde engel var mı kontrol et
            if (walls.Any(w => w.X == newPlayerX && w.Y == newPlayerY) ||
                boxes.Any(b => b.X == newPlayerX && b.Y == newPlayerY))
            {
                Debug.WriteLine("Path is blocked");
                return false;
            }

            // Hareketi gerçekleştir
            int newBoxX = player.X;
            int newBoxY = player.Y;

            Debug.WriteLine($"Moving player to ({newPlayerX}, {newPlayerY})");
            player.X = newPlayerX;
            player.Y = newPlayerY;

            Debug.WriteLine($"Moving box to ({newBoxX}, {newBoxY})");
            nearestBox.X = newBoxX;
            nearestBox.Y = newBoxY;

            Debug.WriteLine("Pull successful");
            return true;
        }
        private Box FindNearestBox(Player player, List<Box> boxes)
        {
            // Sadece oyuncunun yanındaki kutuları bul (1 kare mesafede)
            return boxes.FirstOrDefault(b =>
                (Math.Abs(b.X - player.X) == 1 && b.Y == player.Y) ||
                (Math.Abs(b.Y - player.Y) == 1 && b.X == player.X));
        }
        private Direction DeterminePullDirection(Player player, Box box)
        {
            // Kutunun oyuncuya göre yönünü belirle
            if (box.X == player.X)
            {
                if (box.Y < player.Y)
                    return Direction.Down;  // Kutu oyuncunun altındaysa aşağı çek
                else
                    return Direction.Up;    // Kutu oyuncunun üstündeyse yukarı çek
            }
            else
            {
                if (box.X < player.X)
                    return Direction.Right; // Kutu oyuncunun sağındaysa sağa çek
                else
                    return Direction.Left;  // Kutu oyuncunun solundaysa sola çek
            }
        }


        public bool TryStrongPush(Player player, List<Box> boxes, Direction direction, List<Wall> walls)
        {
            // Önündeki iki kutuyu kontrol et
            int firstX = player.X;
            int firstY = player.Y;
            UpdatePosition(ref firstX, ref firstY, direction);

            int secondX = firstX;
            int secondY = firstY;
            UpdatePosition(ref secondX, ref secondY, direction);

            // İki kutu peş peşe mi kontrol et
            var firstBox = boxes.FirstOrDefault(b => b.X == firstX && b.Y == firstY);
            var secondBox = boxes.FirstOrDefault(b => b.X == secondX && b.Y == secondY);

            if (firstBox == null || secondBox == null)
                return false;

            // İkinci kutunun gideceği yeri kontrol et
            int thirdX = secondX;
            int thirdY = secondY;
            UpdatePosition(ref thirdX, ref thirdY, direction);

            if (walls.Any(w => w.X == thirdX && w.Y == thirdY) ||
                boxes.Any(b => b.X == thirdX && b.Y == thirdY && b != firstBox && b != secondBox))
                return false;

            // Kutuları it
            secondBox.Move(direction);
            firstBox.Move(direction);
            return true;
        }

        public bool TrySprintMove(Player player, Direction direction, List<Box> boxes, List<Wall> walls)
        {
            int newX = player.X;
            int newY = player.Y;

            // İki adım ilerlet
            for (int i = 0; i < 2; i++)
            {
                UpdatePosition(ref newX, ref newY, direction);

                if (walls.Any(w => w.X == newX && w.Y == newY) ||
                    boxes.Any(b => b.X == newX && b.Y == newY))
                    return false;
            }

            player.X = newX;
            player.Y = newY;
            return true;
        }

        public bool TryThrowBox(Player player, Box box, Direction direction, List<Box> boxes, List<Wall> walls)
        {
            if (!IsAdjacent(player, box))
                return false;

            int targetX = box.X;
            int targetY = box.Y;

            // İki kare ilerlet
            for (int i = 0; i < 2; i++)
            {
                UpdatePosition(ref targetX, ref targetY, direction);
            }

            // Hedef noktada engel var mı kontrol et
            if (walls.Any(w => w.X == targetX && w.Y == targetY) ||
                boxes.Any(b => b.X == targetX && b.Y == targetY && b != box))
                return false;

            box.X = targetX;
            box.Y = targetY;
            return true;
        }

        public bool TrySkateboardMove(Player player, Direction direction, List<Box> boxes, List<Wall> walls)
        {
            int newX = player.X;
            int newY = player.Y;

            while (true)
            {
                int nextX = newX;
                int nextY = newY;
                UpdatePosition(ref nextX, ref nextY, direction);

                if (walls.Any(w => w.X == nextX && w.Y == nextY) ||
                    boxes.Any(b => b.X == nextX && b.Y == nextY))
                    break;

                newX = nextX;
                newY = nextY;
            }

            if (newX == player.X && newY == player.Y)
                return false;

            player.X = newX;
            player.Y = newY;
            return true;
        }

        private void UpdatePosition(ref int x, ref int y, Direction direction)
        {
            switch (direction)
            {
                case Direction.Up:
                    y--;
                    break;
                case Direction.Down:
                    y++;
                    break;
                case Direction.Left:
                    x--;
                    break;
                case Direction.Right:
                    x++;
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
                _ => direction
            };
        }

        private bool IsAdjacent(Player player, Box box)
        {
            return (Math.Abs(player.X - box.X) == 1 && player.Y == box.Y) ||
                   (Math.Abs(player.Y - box.Y) == 1 && player.X == box.X);
        }

        private bool CanMoveBox(Box box, Direction direction, List<Box> boxes, List<Wall> walls)
        {
            int newX = box.X;
            int newY = box.Y;
            UpdatePosition(ref newX, ref newY, direction);

            return !walls.Any(w => w.X == newX && w.Y == newY) &&
                   !boxes.Any(b => b.X == newX && b.Y == newY && b != box);
        }
    }
}
