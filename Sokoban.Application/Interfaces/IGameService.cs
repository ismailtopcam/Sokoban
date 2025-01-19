using Sokoban.Application.DTOs;
using Sokoban.Core.Enums;

namespace Sokoban.Application.Interfaces
{
    public interface IGameService
    {
        Task<bool> InitializeGameAsync(string levelFile);
        Task<GameStateDto> MovePlayerAsync(Direction direction);
        Task<GameStateDto> UsePowerUpAsync(PowerUpType powerUp);
        Task<bool> SaveGameAsync();
        Task<bool> LoadGameAsync();
        GameStateDto GetCurrentState();
    }
}
