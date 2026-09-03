using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using exam_system.Domain.Entities.Diplomas;
using exam_system.Persistence;
using exam_system.Persistence.Context;
using exam_system.Persistence.DataAccess;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddPersistenceServices(builder.Configuration);

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
});

builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

var app = builder.Build();

// Seed Database automatically on startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        await AppDbContextSeed.SeedAsync(context, logger);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred during database migration/seeding.");
    }
}

// Enable Swagger UI in Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Examination System API v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();

// Test Minimal API Endpoint to verify database access and generic repository
app.MapGet("/api/test/diplomas", async (IGenericRepository<Diploma> diplomaRepo, CancellationToken ct) =>
{
    var diplomas = await diplomaRepo.GetAll()
        .Select(d => new
        {
            d.Id,
            d.Title,
            d.Description,
            QuizzesCount = d.Quizzes.Count,
            EnrollmentsCount = d.Enrollments.Count,
            d.CreatedAt
        })
        .ToListAsync(ct);

    return Results.Ok(new
    {
        Success = true,
        Count = diplomas.Count,
        Data = diplomas
    });
})
.WithName("GetTestDiplomas")
.WithTags("Test");

app.MapControllers();

app.Run();
