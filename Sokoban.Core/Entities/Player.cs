using Sokoban.Core.Enums;
using Sokoban.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sokoban.Core.Entities
{
    public class Player : IMovable
    {
        public int X { get; set; }
        public int Y { get; set; }
        public List<PowerUpType> ActivePowerUps { get; private set; }
        public bool CanPull { get; private set; }
        public bool CanStrongPush { get; private set; }
        public bool CanSprint { get; private set; }
        public bool CanThrow { get; private set; }
        public bool CanSkateboard { get; private set; }
        public bool CanPunch { get; private set; }

        public Player(int x, int y)
        {
            X = x;
            Y = y;
            ActivePowerUps = new List<PowerUpType>();
        }

        public void AddPowerUp(PowerUpType powerUp)
        {
            if (!ActivePowerUps.Contains(powerUp))
            {
                ActivePowerUps.Add(powerUp);
                UpdatePowerUpStates();
            }
        }

        private void UpdatePowerUpStates()
        {
            CanPull = ActivePowerUps.Contains(PowerUpType.Pull);
            CanStrongPush = ActivePowerUps.Contains(PowerUpType.Push);
            CanSprint = ActivePowerUps.Contains(PowerUpType.Sprint);
            CanThrow = ActivePowerUps.Contains(PowerUpType.Throw);
            CanSkateboard = ActivePowerUps.Contains(PowerUpType.Skateboard);
            CanPunch = ActivePowerUps.Contains(PowerUpType.Punch);
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
        }
    }
}
