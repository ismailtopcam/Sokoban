using Sokoban.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sokoban.Application.Interfaces
{
    public interface IGameStateRepository
    {
        Task<GameStateDto> LoadGameStateAsync();
        Task<bool> SaveGameStateAsync(GameStateDto gameState);
    }
}
