using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using exam_system.Common.Middleware;
using exam_system.Domain.Entities.Diplomas;
using exam_system.Features.Identity.Register.Commands;
using exam_system.Features.Shared;
using exam_system.Persistence;
using exam_system.Persistence.Context;
using exam_system.Persistence.DataAccess;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddPersistenceServices(builder.Configuration);

builder.Services.AddMediatR(typeof(Program).Assembly);

// FluentValidation: find every AbstractValidator<> in this assembly and register
// it as IValidator<TRequest> so the ValidationBehavior can inject them.
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

// MediatR pipeline: run ValidationBehavior before every Handler (fail fast).
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

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

// TEMPORARY test endpoint — proves the MediatR validation pipeline end-to-end.
// Send bad input -> 400 with field errors (validator blocked the Handler).
// Send valid input  -> 200 "Validation passed - Handler reached."
// EXAM-103 replaces this with the real POST /api/auth/register controller.
app.MapPost("/api/test/register-validation", async (RegisterUserCommand command, IMediator mediator) =>
{
    try
    {
        var response = await mediator.Send(command);
        return Results.Json(response, statusCode: response.StatusCode);
    }
    catch (ValidationException ex)
    {
        var errors = ex.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

        var body = RequestResponse<object>.Fail("One or more validation errors occurred.", 400, errors);
        return Results.Json(body, statusCode: 400);
    }
})
.WithName("TestRegisterValidation")
.WithTags("Test");

app.MapControllers();

app.Run();
