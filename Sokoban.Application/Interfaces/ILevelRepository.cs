using Sokoban.Application.DTOs;

namespace Sokoban.Application.Interfaces
{
    public interface ILevelRepository
    {
        Task<LevelDto> LoadLevelAsync(string levelFile);
        Task<bool> SaveLevelAsync(LevelDto level, string levelFile);
        Task<IEnumerable<string>> GetAvailableLevelsAsync();
    }
}
