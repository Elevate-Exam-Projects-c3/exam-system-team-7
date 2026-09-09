using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using exam_system.Common.Middleware;
using exam_system.Domain.Entities.Diplomas;
using exam_system.Features.Identity.Shared;
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

// Password hashing: one Singleton instance is enough — the hasher keeps no
// state and BCrypt is thread-safe, so the same instance can serve all requests.
builder.Services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();

// EXAM-103 services: OTP generation (crypto-random) and email delivery.
// Both are stateless, so Singleton is enough. Feature code only knows the
// IEmailSender interface — the console sender (DevEmailSender) can be
// swapped back for offline development without touching any handler.
builder.Services.AddSingleton<IOtpGenerator, RandomOtpGenerator>();

// Real SMTP delivery (MailKit). Settings live in the "Smtp" section of
// appsettings.Development.json (Gmail: smtp.gmail.com:587 + app password).
builder.Services.Configure<SmtpOptions>(builder.Configuration.GetSection(SmtpOptions.SectionName));
builder.Services.AddSingleton<IEmailSender, SmtpEmailSender>();

var app = builder.Build();

// Global exception handling: ValidationException -> 400 with field-mapped
// errors, anything else -> 500 without leaking details. Replaces the inline
// catch that used to live in the temporary test endpoint.
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

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
