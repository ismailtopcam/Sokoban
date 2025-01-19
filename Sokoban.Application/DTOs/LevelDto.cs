using Sokoban.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sokoban.Application.DTOs
{
    public class LevelDto
    {
        public Player Player { get; set; }
        public List<Box> Boxes { get; set; } = new();
        public List<Wall> Walls { get; set; } = new();
        public List<Target> Targets { get; set; } = new();
        public List<PowerUp> PowerUps { get; set; } = new();
        public int LevelNumber { get; set; }
        public string LevelName { get; set; }
    }
}
