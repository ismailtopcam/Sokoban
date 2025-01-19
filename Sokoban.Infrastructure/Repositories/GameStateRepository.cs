using Sokoban.Application.DTOs;
using Sokoban.Application.Interfaces;

namespace Sokoban.Infrastructure.Repositories
{
    public class GameStateRepository : IGameStateRepository
    {
        private readonly string _saveDirectory;
        private const string SaveFileName = "currentGame.save";

        public GameStateRepository(string saveDirectory)
        {
            _saveDirectory = saveDirectory;
            Directory.CreateDirectory(_saveDirectory);
        }

        public async Task<GameStateDto> LoadGameStateAsync()
        {
            var filePath = Path.Combine(_saveDirectory, SaveFileName);
            if (!File.Exists(filePath))
                return null;

            try
            {
                var json = await File.ReadAllTextAsync(filePath);
                return System.Text.Json.JsonSerializer.Deserialize<GameStateDto>(json);
            }
            catch (Exception ex)
            {
                // Loglama yapılabilir
                return null;
            }
        }

        public async Task<bool> SaveGameStateAsync(GameStateDto gameState)
        {
            try
            {
                var filePath = Path.Combine(_saveDirectory, SaveFileName);
                var json = System.Text.Json.JsonSerializer.Serialize(gameState);
                await File.WriteAllTextAsync(filePath, json);
                return true;
            }
            catch (Exception ex)
            {
                // Loglama yapılabilir
                return false;
            }
        }
    }
}
