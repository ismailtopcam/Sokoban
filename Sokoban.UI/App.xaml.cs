using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using Sokoban.Application.Interfaces;
using Sokoban.Application.Services;
using Sokoban.Infrastructure.Repositories;
using Sokoban.UI.ViewModels;

namespace Sokoban.UI
{
    public partial class App : System.Windows.Application
    {
        private IHost _host;

        public App()
        {
            Debug.WriteLine("App: Constructor started");
            try
            {
                _host = Host.CreateDefaultBuilder()
                    .ConfigureServices((context, services) =>
                    {
                        ConfigureServices(services);
                    })
                    .Build();

                Debug.WriteLine("App: Host built successfully");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"App: Error in constructor: {ex}");
                MessageBox.Show($"Initialization Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            Debug.WriteLine("App: OnStartup started");
            try
            {
                await _host.StartAsync();
                Debug.WriteLine("App: Host started");

                var mainWindow = _host.Services.GetRequiredService<MainWindow>();
                Debug.WriteLine("App: MainWindow created");

                MainWindow = mainWindow;
                mainWindow.Show();
                Debug.WriteLine("App: MainWindow shown");

                base.OnStartup(e);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"App: Error in OnStartup: {ex}");
                MessageBox.Show($"Startup Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            try
            {
                using (_host)
                {
                    await _host.StopAsync(TimeSpan.FromSeconds(5));
                }

                base.OnExit(e);
                Debug.WriteLine("App: Exited successfully");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"App: Error in OnExit: {ex}");
            }
        }

        private void ConfigureServices(IServiceCollection services)
        {
            try
            {
                Debug.WriteLine("App: ConfigureServices started");

                var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                var levelsPath = Path.Combine(baseDirectory, "Levels");
                var savesPath = Path.Combine(baseDirectory, "Saves");

                Debug.WriteLine($"App: Levels directory: {levelsPath}");
                Debug.WriteLine($"App: Saves directory: {savesPath}");

                Directory.CreateDirectory(levelsPath);
                Directory.CreateDirectory(savesPath);

                // Infrastructure Services
                services.AddSingleton<ILevelRepository>(_ =>
                {
                    var repo = new LevelRepository(levelsPath);
                    Debug.WriteLine("App: LevelRepository created");
                    return repo;
                });

                services.AddSingleton<IGameStateRepository>(_ =>
                {
                    var repo = new GameStateRepository(savesPath);
                    Debug.WriteLine("App: GameStateRepository created");
                    return repo;
                });

                // Application Services
                services.AddSingleton<IMovementService, MovementService>();
                services.AddSingleton<IPowerUpService, PowerUpService>();
                services.AddSingleton<IGameService, GameService>();

                // UI Services
                services.AddSingleton<MainViewModel>();
                services.AddSingleton<MainWindow>();

                Debug.WriteLine("App: ConfigureServices completed");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"App: Error in ConfigureServices: {ex}");
                throw;
            }
        }
    }
}