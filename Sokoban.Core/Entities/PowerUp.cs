using Sokoban.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sokoban.Core.Entities
{
    public class PowerUp
    {
        public int X { get; }
        public int Y { get; }
        public PowerUpType Type { get; }
        public bool IsCollected { get; set; }

        public PowerUp(int x, int y, PowerUpType type)
        {
            X = x;
            Y = y;
            Type = type;
            IsCollected = false;
        }
    }
}
