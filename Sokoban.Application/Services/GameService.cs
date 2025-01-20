using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Sokoban.Application.DTOs;
using Sokoban.Application.Interfaces;
using Sokoban.Core.Entities;
using Sokoban.Core.Enums;

namespace Sokoban.Application.Services
{
    public class GameService : IGameService
    {
        private readonly IMovementService _movementService;
        private readonly IPowerUpService _powerUpService;
        private readonly ILevelRepository _levelRepository;
        private readonly IGameStateRepository _gameStateRepository;

        private GameState _currentState;
        private Player _player;
        private List<Box> _boxes;
        private List<Wall> _walls;
        private List<Target> _targets;
        private List<PowerUp> _powerUps;
        private int _movesCount;

        public GameService(
            IMovementService movementService,
            IPowerUpService powerUpService,
            ILevelRepository levelRepository,
            IGameStateRepository gameStateRepository)
        {
            _movementService = movementService;
            _powerUpService = powerUpService;
            _levelRepository = levelRepository;
            _gameStateRepository = gameStateRepository;

            _boxes = new List<Box>();
            _walls = new List<Wall>();
            _targets = new List<Target>();
            _powerUps = new List<PowerUp>();
            _movesCount = 0;
        }

        public async Task<bool> InitializeGameAsync(string levelFile)
        {
            var level = await _levelRepository.LoadLevelAsync(levelFile);
            if (level == null) return false;

            // Level verilerini yükle
            _player = level.Player;
            _boxes = level.Boxes;
            _walls = level.Walls;
            _targets = level.Targets;
            _powerUps = level.PowerUps;
            _currentState = GameState.Playing;
            _movesCount = 0;

            return true;
        }

        public async Task<GameStateDto> MovePlayerAsync(Direction direction)
        {
            if (_currentState != GameState.Playing)
                return GetCurrentState();

            var moveResult = _movementService.TryMovePlayer(_player, direction, _boxes, _walls);
            if (moveResult)
            {
                _movesCount++;

                // Güç geliştirmeleri kontrol et
                CheckPowerUps();

                // Oyun durumunu kontrol et
                CheckGameState();

                await SaveGameAsync();
            }

            return GetCurrentState();
        }

        //public async Task<GameStateDto> UsePowerUpAsync(PowerUpType powerUp)
        //{
        //    if (!_player.ActivePowerUps.Contains(powerUp))
        //        return GetCurrentState();

        //    bool powerUpUsed = _powerUpService.UsePowerUp(_player, powerUp, _boxes, _walls);
        //    if (powerUpUsed)
        //    {
        //        _movesCount++;

        //        CheckGameState();
        //        await SaveGameAsync();
        //    }

        //    return GetCurrentState();
        //}
        public async Task<GameStateDto> UsePowerUpAsync(PowerUpType powerUp, Direction direction)
        {
            Debug.WriteLine($"Attempting to use power {powerUp} in direction {direction}");
            bool powerUpUsed = false;

            switch (powerUp)
            {
                case PowerUpType.Pull:
                    powerUpUsed = _movementService.TryPullBox(_player, direction, _boxes, _walls);
                    Debug.WriteLine($"Pull power used: {powerUpUsed}");
                    break;

                case PowerUpType.Push:
                    powerUpUsed = _movementService.TryStrongPush(_player, _boxes, direction, _walls);
                    Debug.WriteLine($"StrongPush power used: {powerUpUsed}");
                    break;

                case PowerUpType.Sprint:
                    powerUpUsed = _movementService.TrySprintMove(_player, direction, _boxes, _walls);
                    Debug.WriteLine($"Sprint power used: {powerUpUsed}");
                    break;

                case PowerUpType.Throw:
                    // Oyuncunun yanındaki kutuyu bul
                    var nearestBox = _boxes.OrderBy(b =>
                        Math.Abs(b.X - _player.X) + Math.Abs(b.Y - _player.Y))
                        .FirstOrDefault();
                    if (nearestBox != null)
                    {
                        powerUpUsed = _movementService.TryThrowBox(_player, nearestBox, direction, _boxes, _walls);
                        Debug.WriteLine($"Throw power used: {powerUpUsed}");
                    }
                    break;

                case PowerUpType.Skateboard:
                    powerUpUsed = _movementService.TrySkateboardMove(_player, direction, _boxes, _walls);
                    Debug.WriteLine($"Skateboard power used: {powerUpUsed}");
                    break;

                case PowerUpType.Punch:
                    // Yıkılabilir duvarı bul
                    var wallToBreak = _walls.FirstOrDefault(w =>
                        w.IsBreakable &&
                        w.X == _player.X + GetDirectionOffset(direction).dx &&
                        w.Y == _player.Y + GetDirectionOffset(direction).dy);

                    if (wallToBreak != null)
                    {
                        _walls.Remove(wallToBreak);
                        powerUpUsed = true;
                        Debug.WriteLine($"StrongPunch power used: Wall broken at {wallToBreak.X},{wallToBreak.Y}");
                    }
                    break;
            }

            if (powerUpUsed)
            {
                CheckGameState();
                await SaveGameAsync();
            }

            return GetCurrentState();
        }
        private (int dx, int dy) GetDirectionOffset(Direction direction)
        {
            return direction switch
            {
                Direction.Up => (0, -1),
                Direction.Down => (0, 1),
                Direction.Left => (-1, 0),
                Direction.Right => (1, 0),
                _ => (0, 0)
            };
        }

        private void CheckPowerUps()
        {
            var powerUp = _powerUps.FirstOrDefault(p =>
                p.X == _player.X &&
                p.Y == _player.Y &&
                !p.IsCollected);

            if (powerUp != null)
            {
                powerUp.IsCollected = true;
                _player.AddPowerUp(powerUp.Type);
            }
        }

        private void CheckGameState()
        {
            bool allBoxesOnTarget = _boxes.All(b =>
                _targets.Any(t => t.X == b.X && t.Y == b.Y));

            if (allBoxesOnTarget)
            {
                _currentState = GameState.Completed;
            }
        }

        public GameStateDto GetCurrentState()
        {
            var state = new GameStateDto
            {
                Player = _player,
                Boxes = _boxes.ToList(),
                Walls = _walls.ToList(),
                Targets = _targets.ToList(),
                PowerUps = _powerUps.ToList(),
                GameState = _currentState,
                MovesCount = _movesCount,
                BoardWidth = CalculateBoardWidth(),
                BoardHeight = CalculateBoardHeight()
            };

            Debug.WriteLine($"Current game state - Board size: {state.BoardWidth}x{state.BoardHeight}");
            return state;
        }

        public async Task<bool> SaveGameAsync()
        {
            return await _gameStateRepository.SaveGameStateAsync(GetCurrentState());
        }

        public async Task<bool> LoadGameAsync()
        {
            var gameState = await _gameStateRepository.LoadGameStateAsync();
            if (gameState == null) return false;

            _player = gameState.Player;
            _boxes = gameState.Boxes;
            _walls = gameState.Walls;
            _targets = gameState.Targets;
            _powerUps = gameState.PowerUps;
            _currentState = gameState.GameState;

            return true;
        }

        private int CalculateBoardWidth()
        {
            if (_player == null) return 0;

            int maxX = _player.X;

            if (_walls.Any())
                maxX = Math.Max(maxX, _walls.Max(w => w.X));

            if (_boxes.Any())
                maxX = Math.Max(maxX, _boxes.Max(b => b.X));

            if (_targets.Any())
                maxX = Math.Max(maxX, _targets.Max(t => t.X));

            if (_powerUps.Any())
                maxX = Math.Max(maxX, _powerUps.Max(p => p.X));

            Debug.WriteLine($"Calculated board width: {maxX + 1}");
            return maxX + 1; // +1 because coordinates are 0-based
        }

        private int CalculateBoardHeight()
        {
            if (_player == null) return 0;

            int maxY = _player.Y;

            if (_walls.Any())
                maxY = Math.Max(maxY, _walls.Max(w => w.Y));

            if (_boxes.Any())
                maxY = Math.Max(maxY, _boxes.Max(b => b.Y));

            if (_targets.Any())
                maxY = Math.Max(maxY, _targets.Max(t => t.Y));

            if (_powerUps.Any())
                maxY = Math.Max(maxY, _powerUps.Max(p => p.Y));

            Debug.WriteLine($"Calculated board height: {maxY + 1}");
            return maxY + 1; // +1 because coordinates are 0-based
        }
    }
}
