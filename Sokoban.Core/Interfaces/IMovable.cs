using Sokoban.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sokoban.Core.Interfaces
{
    public interface IMovable
    {
        bool CanMove(Direction direction);
        void Move(Direction direction);
    }
}
