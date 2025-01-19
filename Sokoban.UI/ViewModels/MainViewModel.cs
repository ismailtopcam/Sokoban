using Microsoft.Win32;
using Sokoban.Application.DTOs;
using Sokoban.Application.Interfaces;
using Sokoban.Core.Enums;
using Sokoban.UI.Commands;
using Sokoban.UI.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Sokoban.UI.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly IGameService _gameService;
        private readonly DispatcherTimer _gameTimer;

        private GameStateDto _currentGameState;
        private ObservableCollection<GameBoardCell> _gameBoard;
        private ObservableCollection<PowerUpViewModel> _activePowerUps;
        private int _boardWidth;
        private int _boardHeight;
        private TimeSpan _elapsedTime;

        public MainViewModel(IGameService gameService)
        {
            try
            {
                var testUri = new Uri("pack://application:,,,/Sokoban.UI;component/Images/player.png");
                var stream = System.Windows.Application.GetResourceStream(testUri);
                if (stream != null)
                {
                    Debug.WriteLine("PNG resource loaded successfully!");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading PNG resource: {ex}");
            }

            _gameService = gameService;
            _gameBoard = new ObservableCollection<GameBoardCell>();
            _activePowerUps = new ObservableCollection<PowerUpViewModel>();

            // Komutları başlat
            InitializeCommands();

            // Timer'ı başlat
            _gameTimer = new DispatcherTimer();
            _gameTimer.Interval = TimeSpan.FromSeconds(1);
            _gameTimer.Tick += GameTimer_Tick;

            // İlk seviyeyi yükle
            _gameTimer.Start();
            ExecuteNewGame();
        }

        private async void ExecuteNewGame()
        {
            try
            {
                Debug.WriteLine("Loading level1.sok...");
                if (await _gameService.InitializeGameAsync("level1.sok"))
                {
                    Debug.WriteLine("Level loaded successfully");
                    _currentGameState = _gameService.GetCurrentState();

                    Debug.WriteLine($"Player position: {_currentGameState.Player?.X}, {_currentGameState.Player?.Y}");
                    Debug.WriteLine($"Number of walls: {_currentGameState.Walls?.Count}");
                    Debug.WriteLine($"Number of boxes: {_currentGameState.Boxes?.Count}");
                    Debug.WriteLine($"Number of targets: {_currentGameState.Targets?.Count}");

                    UpdateGameBoard();
                    Debug.WriteLine($"Board size: {BoardWidth}x{BoardHeight}");
                    Debug.WriteLine($"GameBoard cells: {GameBoard?.Count}");
                }
                else
                {
                    Debug.WriteLine("Failed to load level");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in ExecuteNewGame: {ex}");
            }
        }

        private string GetImagePath(string imageName)
        {
            return $"/Images/{imageName}.png";
        }

        private void UpdateGameBoard()
        {
            try
            {
                if (_currentGameState == null)
                {
                    Debug.WriteLine("UpdateGameBoard: _currentGameState is null");
                    return;
                }

                BoardWidth = _currentGameState.BoardWidth;
                BoardHeight = _currentGameState.BoardHeight;

                Debug.WriteLine($"Updating board with size: {BoardWidth}x{BoardHeight}");

                var newBoard = new ObservableCollection<GameBoardCell>();

                for (int y = 0; y < BoardHeight; y++)
                {
                    for (int x = 0; x < BoardWidth; x++)
                    {
                        var cell = new GameBoardCell
                        {
                            ImageSource = GetImagePath("floor")
                        };

                        // Hedef noktalarını kontrol et
                        if (_currentGameState.Targets.Any(t => t.X == x && t.Y == y))
                        {
                            cell.ImageSource = GetImagePath("target");
                        }

                        // Duvarları kontrol et
                        if (_currentGameState.Walls.Any(w => w.X == x && w.Y == y))
                        {
                            cell.ImageSource = GetImagePath("wall");
                        }

                        // Kutuları kontrol et
                        if (_currentGameState.Boxes.Any(b => b.X == x && b.Y == y))
                        {
                            cell.ImageSource = GetImagePath("box");
                        }

                        // Oyuncuyu kontrol et
                        if (_currentGameState.Player.X == x && _currentGameState.Player.Y == y)
                        {
                            cell.ImageSource = GetImagePath("player");
                        }

                        newBoard.Add(cell);
                    }
                }

                GameBoard = newBoard;
                OnPropertyChanged(nameof(GameBoard));
                OnPropertyChanged(nameof(BoardWidth));
                OnPropertyChanged(nameof(BoardHeight));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in UpdateGameBoard: {ex}");
            }
        }

        #region Properties

        public ObservableCollection<GameBoardCell> GameBoard
        {
            get => _gameBoard;
            set
            {
                _gameBoard = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<PowerUpViewModel> ActivePowerUps
        {
            get => _activePowerUps;
            set
            {
                _activePowerUps = value;
                OnPropertyChanged();
            }
        }

        public int BoardWidth
        {
            get => _boardWidth;
            set
            {
                _boardWidth = value;
                OnPropertyChanged();
            }
        }

        public int BoardHeight
        {
            get => _boardHeight;
            set
            {
                _boardHeight = value;
                OnPropertyChanged();
            }
        }

        public TimeSpan ElapsedTime
        {
            get => _elapsedTime;
            set
            {
                _elapsedTime = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Commands

        public ICommand NewGameCommand { get; private set; }
        public ICommand LoadLevelCommand { get; private set; }
        public ICommand SaveGameCommand { get; private set; }
        public ICommand ExitCommand { get; private set; }
        public ICommand MoveCommand { get; private set; }

        private void InitializeCommands()
        {
            NewGameCommand = new RelayCommand(ExecuteNewGame);
            LoadLevelCommand = new RelayCommand(ExecuteLoadLevel);
            SaveGameCommand = new RelayCommand(ExecuteSaveGame);
            ExitCommand = new RelayCommand(ExecuteExit);
            MoveCommand = new RelayCommand<Direction>(ExecuteMove);
        }

        private async void ExecuteLoadLevel()
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Sokoban Levels (*.sok)|*.sok",
                InitialDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Levels")
            };

            if (dialog.ShowDialog() == true)
            {
                await StartNewGame(dialog.FileName);
            }
        }

        private async void ExecuteSaveGame()
        {
            await _gameService.SaveGameAsync();
        }

        private void ExecuteExit()
        {
            // Oyunu kaydet
            _gameService.SaveGameAsync().Wait();

            // Ana pencereyi bul ve kapat
            var mainWindow = System.Windows.Application.Current.MainWindow;
            mainWindow?.Close();
        }

        private async void ExecuteMove(Direction direction)
        {
            var newState = await _gameService.MovePlayerAsync(direction);
            UpdateGameState(newState);
        }

        #endregion

        #region Game Logic

        private async Task StartNewGame(string levelFile)
        {
            if (await _gameService.InitializeGameAsync(levelFile))
            {
                _currentGameState = _gameService.GetCurrentState();
                UpdateGameState(_currentGameState);
                _gameTimer.Start();
            }
        }

        private void UpdateGameState(GameStateDto gameState)
        {
            _currentGameState = gameState;
            UpdateGameBoard();
            UpdatePowerUps();

            if (gameState.GameState == GameState.Completed)
            {
                _gameTimer.Stop();
                ShowLevelCompleteDialog();
            }
        }

        private void UpdatePowerUps()
        {
            ActivePowerUps.Clear();
            foreach (var powerUp in _currentGameState.Player.ActivePowerUps)
            {
                ActivePowerUps.Add(new PowerUpViewModel(powerUp));
            }
        }

        private int CalculateBoardWidth()
        {
            return Math.Max(
                Math.Max(
                    _currentGameState.Walls.Max(w => w.X),
                    _currentGameState.Boxes.Max(b => b.X)
                ),
                Math.Max(
                    _currentGameState.Targets.Max(t => t.X),
                    _currentGameState.PowerUps.Max(p => p.X)
                )
            ) + 1;
        }

        private int CalculateBoardHeight()
        {
            return Math.Max(
                Math.Max(
                    _currentGameState.Walls.Max(w => w.Y),
                    _currentGameState.Boxes.Max(b => b.Y)
                ),
                Math.Max(
                    _currentGameState.Targets.Max(t => t.Y),
                    _currentGameState.PowerUps.Max(p => p.Y)
                )
            ) + 1;
        }

        private void ShowLevelCompleteDialog()
        {
            MessageBox.Show(
                $"Tebrikler! Seviyeyi tamamladınız!\nToplam hamle: {_currentGameState.MovesCount}\nGeçen süre: {_elapsedTime}",
                "Seviye Tamamlandı",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            ElapsedTime = ElapsedTime.Add(TimeSpan.FromSeconds(1));
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
