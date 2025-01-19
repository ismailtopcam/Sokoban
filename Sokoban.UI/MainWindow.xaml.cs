using Sokoban.Core.Enums;
using Sokoban.UI.ViewModels;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
            case Key.R:
                _viewModel.NewGameCommand.Execute(null);
                break;
        }
    }
}