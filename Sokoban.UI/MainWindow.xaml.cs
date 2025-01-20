using Sokoban.Core.Enums;
using Sokoban.UI.ViewModels;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace Sokoban.UI;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly MainViewModel _viewModel;

    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = _viewModel;

        // Klavye olaylarını dinle
        KeyDown += MainWindow_KeyDown;
    }

    private void MainWindow_KeyDown(object sender, KeyEventArgs e)
    {
        switch (e.Key)
        {
            case Key.Left:
                _viewModel.MoveCommand.Execute(Direction.Left);
                break;
            case Key.Right:
                _viewModel.MoveCommand.Execute(Direction.Right);
                break;
            case Key.Up:
                _viewModel.MoveCommand.Execute(Direction.Up);
                break;
            case Key.Down:
                _viewModel.MoveCommand.Execute(Direction.Down);
                break;
            case Key.F5:
                _viewModel.NewGameCommand.Execute(null);
                break;

            // Güç geliştirmeleri
            case Key.P:
                _viewModel.UsePowerUpCommand.Execute(PowerUpType.Pull);
                Debug.WriteLine("Pull power key pressed");
                break;
            case Key.S:
                _viewModel.UsePowerUpCommand.Execute(PowerUpType.Push);
                Debug.WriteLine("StrongPush power key pressed");
                break;
            case Key.R:
                _viewModel.UsePowerUpCommand.Execute(PowerUpType.Sprint);
                Debug.WriteLine("Sprint power key pressed");
                break;
            case Key.T:
                _viewModel.UsePowerUpCommand.Execute(PowerUpType.Throw);
                Debug.WriteLine("Throw power key pressed");
                break;
            case Key.K:
                _viewModel.UsePowerUpCommand.Execute(PowerUpType.Skateboard);
                Debug.WriteLine("Skateboard power key pressed");
                break;
            case Key.F:
                _viewModel.UsePowerUpCommand.Execute(PowerUpType.Punch);
                Debug.WriteLine("StrongPunch power key pressed");
                break;
        }
    }
}