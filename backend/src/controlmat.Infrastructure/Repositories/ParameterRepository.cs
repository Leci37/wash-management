using Microsoft.EntityFrameworkCore;
using Controlmat.Domain.Repositories;
using Controlmat.Infrastructure.Persistence;

namespace Controlmat.Infrastructure.Repositories;

public class ParameterRepository : IParameterRepository
{
    private readonly SumisanDbContext _context;

    public ParameterRepository(SumisanDbContext context)
    {
        _context = context;
    }

    public async Task<string> GetImagePathAsync()
    {
        var parameter = await _context.Parameters
            .FirstOrDefaultAsync(p => p.Name == "ImagePath");
        return parameter?.Value ?? "/shared/photos";
    }

    public async Task<int> GetMaxPhotosPerWashAsync()
    {
        var parameter = await _context.Parameters
            .FirstOrDefaultAsync(p => p.Name == "MaxPhotosPerWash");
        return int.TryParse(parameter?.Value, out var max) ? max : 99;
    }

    public async Task<string[]> GetSupportedFileTypesAsync()
    {
        var parameter = await _context.Parameters
            .FirstOrDefaultAsync(p => p.Name == "SupportedFileTypes");
        return parameter?.Value?.Split(',') ?? new[] { "jpg", "png" };
    }
}
