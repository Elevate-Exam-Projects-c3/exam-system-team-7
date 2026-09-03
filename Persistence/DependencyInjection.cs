using Microsoft.EntityFrameworkCore;
using exam_system.Persistence.Context;
using exam_system.Persistence.DataAccess;

namespace exam_system.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? "Server=(localdb)\\mssqllocaldb;Database=ExaminationSystemDb;Trusted_Connection=True;MultipleActiveResultSets=true";

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
