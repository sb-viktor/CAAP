﻿using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using CAAP.Services;
using CAAP.ViewModels;
using CAAP.Views;
using Microsoft.Extensions.DependencyInjection;

namespace CAAP;

public partial class App : Application
{
    /// <summary>
    /// The current application instance.
    /// This property is used to access the application instance from anywhere in the code.
    /// </summary>
    public new static App? Current => Application.Current as App;

    /// <summary>
    /// The service provider for dependency injection.
    /// This property is used to access the service provider from anywhere in the code.
    /// It is set in the App.xaml.cs file when the application is initialized.
    /// </summary>
    public ServiceProvider? Services { get; private set; }
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
            // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
            DisableAvaloniaDataAnnotationValidation();
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel(),
            };

            SetupServices(desktop);
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void SetupServices(IClassicDesktopStyleApplicationLifetime desktop)
    {
        Services = new ServiceCollection()
            .AddSingleton<IFilesService>(x => new FilesService(desktop.MainWindow!))
            .BuildServiceProvider();
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            _ = BindingPlugins.DataValidators.Remove(plugin);
        }
    }
}
