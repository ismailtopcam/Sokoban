using Sokoban.Core.Entities;

namespace Sokoban.Application.DTOs
{
    public class LevelDto
    {
        public Player Player { get; set; }
        public List<Box> Boxes { get; set; } = new();
        public List<Wall> Walls { get; set; } = new();
        public List<Target> Targets { get; set; } = new();
        public List<PowerUp> PowerUps { get; set; } = new();
    }
}
