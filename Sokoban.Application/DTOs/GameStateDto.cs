using Sokoban.Core.Entities;
using Sokoban.Core.Enums;

namespace Sokoban.Application.DTOs
{
    public class GameStateDto
    {
        public Player Player { get; set; }
        public List<Box> Boxes { get; set; } = new();
        public List<Wall> Walls { get; set; } = new();
        public List<Target> Targets { get; set; } = new();
        public List<PowerUp> PowerUps { get; set; } = new();
        public GameState GameState { get; set; }
        public int CurrentLevel { get; set; }
        public int MovesCount { get; set; }
        public TimeSpan ElapsedTime { get; set; }
        public int BoardWidth { get; set; }
        public int BoardHeight { get; set; }
    }
}
