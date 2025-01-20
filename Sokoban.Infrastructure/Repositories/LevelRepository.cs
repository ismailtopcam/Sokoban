using Sokoban.Application.DTOs;
using Sokoban.Application.Interfaces;
using Sokoban.Core.Entities;
using Sokoban.Core.Enums;
using System.Diagnostics;

namespace Sokoban.Infrastructure.Repositories
{
    public class LevelRepository : ILevelRepository
    {
        private readonly string _levelsDirectory;
        private const string DEFAULT_LEVEL = @"##########
                                               #        #
                                               # @   $  #
                                               #    #   #
                                               #  P   . #
                                               #   B    #
                                               #    S   #
                                               #     K  #
                                               #        #
                                               ##########";

        public LevelRepository(string levelsDirectory)
        {
            _levelsDirectory = levelsDirectory;
            Directory.CreateDirectory(_levelsDirectory);

            // Varsayılan seviyeyi oluştur
            var defaultLevelPath = Path.Combine(_levelsDirectory, "level1.sok");
            if (!File.Exists(defaultLevelPath))
            {
                File.WriteAllText(defaultLevelPath, DEFAULT_LEVEL);
            }
        }

        public async Task<LevelDto> LoadLevelAsync(string levelFile)
        {
            try
            {
                var filePath = Path.Combine(_levelsDirectory, levelFile);

                if (!File.Exists(filePath))
                {
                    return null;
                }

                var level = new LevelDto
                {
                    Boxes = new List<Box>(),
                    Walls = new List<Wall>(),
                    Targets = new List<Target>(),
                    PowerUps = new List<PowerUp>()
                };

                var lines = await File.ReadAllLinesAsync(filePath);

                for (int y = 0; y < lines.Length; y++)
                {
                    for (int x = 0; x < lines[y].Length; x++)
                    {
                        switch (lines[y][x])
                        {
                            case '@': // Oyuncu
                                level.Player = new Player(x, y);
                                Debug.WriteLine($"Player added at: {x},{y}");
                                break;
                            case '#': // Duvar
                                level.Walls.Add(new Wall(x, y));
                                Debug.WriteLine($"Wall added at: {x},{y}");
                                break;
                            case 'B': // Yıkılabilir duvar
                                level.Walls.Add(new Wall(x, y, true));
                                Debug.WriteLine($"Breakable wall added at: {x},{y}");
                                break;
                            case '$': // Kutu
                                level.Boxes.Add(new Box(x, y));
                                Debug.WriteLine($"Box added at: {x},{y}");
                                break;
                            case '.': // Hedef
                                level.Targets.Add(new Target(x, y));
                                Debug.WriteLine($"Target added at: {x},{y}");
                                break;
                            case 'P': // Çekme güç geliştirmesi
                                level.PowerUps.Add(new PowerUp(x, y, PowerUpType.Pull));
                                Debug.WriteLine($"Pull powerup added at: {x},{y}");
                                break;
                            case 'S': // Güçlü itme güç geliştirmesi
                                level.PowerUps.Add(new PowerUp(x, y, PowerUpType.Push));
                                Debug.WriteLine($"Strong push powerup added at: {x},{y}");
                                break;
                            case 'R': // Koşma güç geliştirmesi
                                level.PowerUps.Add(new PowerUp(x, y, PowerUpType.Sprint));
                                Debug.WriteLine($"Sprint powerup added at: {x},{y}");
                                break;
                            case 'T': // Fırlatma güç geliştirmesi
                                level.PowerUps.Add(new PowerUp(x, y, PowerUpType.Throw));
                                Debug.WriteLine($"Throw powerup added at: {x},{y}");
                                break;
                            case 'K': // Kaykay güç geliştirmesi
                                level.PowerUps.Add(new PowerUp(x, y, PowerUpType.Skateboard));
                                Debug.WriteLine($"Skateboard powerup added at: {x},{y}");
                                break;
                            case 'F': // Güçlü yumruk güç geliştirmesi
                                level.PowerUps.Add(new PowerUp(x, y, PowerUpType.Punch));
                                Debug.WriteLine($"Strong punch powerup added at: {x},{y}");
                                break;
                            case ' ': // Boş alan
                                break;
                            default:
                                Debug.WriteLine($"Unknown character '{lines[y][x]}' at: {x},{y}");
                                break;
                        }
                    }
                }

                return level;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading level: {ex}");
                return null;
            }
        }

        public async Task<bool> SaveLevelAsync(LevelDto level, string levelFile)
        {
            try
            {
                Debug.WriteLine($"Attempting to save level: {levelFile}");
                var filePath = Path.Combine(_levelsDirectory, levelFile);
                var width = Math.Max(
                    level.Walls.Any() ? level.Walls.Max(w => w.X) : 0,
                    Math.Max(
                        level.Boxes.Any() ? level.Boxes.Max(b => b.X) : 0,
                        Math.Max(
                            level.Targets.Any() ? level.Targets.Max(t => t.X) : 0,
                            level.PowerUps.Any() ? level.PowerUps.Max(p => p.X) : 0
                        )
                    )
                ) + 1;

                var height = Math.Max(
                    level.Walls.Any() ? level.Walls.Max(w => w.Y) : 0,
                    Math.Max(
                        level.Boxes.Any() ? level.Boxes.Max(b => b.Y) : 0,
                        Math.Max(
                            level.Targets.Any() ? level.Targets.Max(t => t.Y) : 0,
                            level.PowerUps.Any() ? level.PowerUps.Max(p => p.Y) : 0
                        )
                    )
                ) + 1;

                Debug.WriteLine($"Calculated board size: {width}x{height}");

                var map = new char[height, width];

                // Boş alanları doldur
                for (int y = 0; y < height; y++)
                    for (int x = 0; x < width; x++)
                        map[y, x] = ' ';

                // Oyuncu
                if (level.Player != null)
                {
                    map[level.Player.Y, level.Player.X] = '@';
                    Debug.WriteLine($"Placed player at: {level.Player.X},{level.Player.Y}");
                }

                // Duvarlar
                foreach (var wall in level.Walls)
                {
                    map[wall.Y, wall.X] = wall.IsBreakable ? 'B' : '#';
                    Debug.WriteLine($"Placed {(wall.IsBreakable ? "breakable " : "")}wall at: {wall.X},{wall.Y}");
                }

                // Kutular
                foreach (var box in level.Boxes)
                {
                    map[box.Y, box.X] = '$';
                    Debug.WriteLine($"Placed box at: {box.X},{box.Y}");
                }

                // Hedefler
                foreach (var target in level.Targets)
                {
                    map[target.Y, target.X] = '.';
                    Debug.WriteLine($"Placed target at: {target.X},{target.Y}");
                }

                // Güç geliştirmeler
                foreach (var powerUp in level.PowerUps)
                {
                    char symbol = powerUp.Type switch
                    {
                        PowerUpType.Pull => 'P',
                        PowerUpType.Push => 'S',
                        PowerUpType.Sprint => 'R',
                        PowerUpType.Throw => 'T',
                        PowerUpType.Skateboard => 'K',
                        PowerUpType.Punch => 'F',
                        _ => ' '
                    };
                    map[powerUp.Y, powerUp.X] = symbol;
                    Debug.WriteLine($"Placed {powerUp.Type} powerup at: {powerUp.X},{powerUp.Y}");
                }

                // Dosyaya yaz
                using (var writer = new StreamWriter(filePath))
                {
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            await writer.WriteAsync(map[y, x]);
                        }
                        await writer.WriteLineAsync();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error saving level: {ex}");
                return false;
            }
        }

        public async Task<IEnumerable<string>> GetAvailableLevelsAsync()
        {
            try
            {
                var levels = Directory.GetFiles(_levelsDirectory, "*.sok")
                    .Select(Path.GetFileName)
                    .ToList();
                return levels;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting available levels: {ex}");
                return Enumerable.Empty<string>();
            }
        }
    }
}