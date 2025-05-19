using Avalonia.Platform.Storage;
using CAAP.Services;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using LibVLCSharp.Shared;
using System.IO;

namespace CAAP.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private LibVLC? _libVLC;
    private MediaPlayer? _mediaPlayer;

    [RelayCommand]
    private async Task OpenFile(CancellationToken token)
    {
        var filesService = App.Current?.Services?.GetService<IFilesService>()
            ?? throw new NullReferenceException("Missing File Service instance.");

        var file = await filesService.OpenFileAsync();
        if (file is null) return;

        await ProcessFile(file, token);
    }

    [RelayCommand]
    private void Pause()
    {
        _mediaPlayer?.Pause();
    }

    [RelayCommand]
    private void Stop()
    {
        _mediaPlayer?.Stop();
    }

    [RelayCommand]
    private void PlayPause()
    {
        if (_mediaPlayer == null)
            return;
        if (_mediaPlayer.IsPlaying)
            _mediaPlayer.Pause();
        else
            _mediaPlayer.Play();
    }

    private async Task ProcessFile(IStorageFile file, CancellationToken token)
    {
        // Get the file temporary path
        var localPath = await GetLocalPathAsync(file, token);
        if (localPath is null)
            return;

        // Initialize LibVLC
        if (_libVLC == null)
            _libVLC = new LibVLC();

        // Dispose the previous MediaPlayer if it exists
        _mediaPlayer?.Dispose();
        _mediaPlayer = new MediaPlayer(_libVLC);

        // Play the media
        using var media = new Media(_libVLC, localPath, FromType.FromPath);
        _mediaPlayer.Play(media);
    }

    private async Task<string?> GetLocalPathAsync(IStorageFile file, CancellationToken token)
    {
        // Check if the file is null
        var tempPath = Path.Combine(Path.GetTempPath(), file.Name);
        await using (var source = await file.OpenReadAsync())
        await using (var dest = File.OpenWrite(tempPath))
            await source.CopyToAsync(dest, token);
        return tempPath;
    }
}
