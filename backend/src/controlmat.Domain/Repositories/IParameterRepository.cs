namespace Controlmat.Domain.Repositories;

public interface IParameterRepository
{
    Task<string> GetImagePathAsync();
    Task<int> GetMaxPhotosPerWashAsync();
    Task<string[]> GetSupportedFileTypesAsync();
}
