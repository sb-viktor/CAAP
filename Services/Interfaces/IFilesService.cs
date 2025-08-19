using Avalonia.Platform.Storage;

namespace CAAP.Services.Interfaces;

public interface IFilesService
{
    public Task<IStorageFile?> OpenFileAsync();

    public Task<IStorageFile?> SaveFileAsync();
}
