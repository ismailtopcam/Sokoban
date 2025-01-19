using Sokoban.Core.Enums;
using Sokoban.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sokoban.Core.Entities
{
    public class Box : IMovable
    {
        public int X { get; set; }
        public int Y { get; set; }
        public bool IsOnTarget { get; set; }

        public Box(int x, int y)
        {
            X = x;
            Y = y;
            IsOnTarget = false;
        }

        public bool CanMove(Direction direction)
        {
            return true;
        }

        public void Move(Direction direction)
        {
            switch (direction)
            {
                case Direction.Up:
                    Y--;
                    break;
                case Direction.Down:
                    Y++;
                    break;
                case Direction.Left:
                    X--;
                    break;
                case Direction.Right:
                    X++;
                    break;
            }
        }

        public void SetPosition(int x, int y)
        {
            X = x;
            Y = y;
            IsOnTarget = false;
        }
    }
}
