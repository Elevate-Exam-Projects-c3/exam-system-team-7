using exam_system.Common.Enums;
using exam_system.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace exam_system.Infrastructure.BackgroundJobs {
    public class QuizAttemptTimeoutBackgroundService : BackgroundService {

        private readonly IServiceProvider serviceProvider;
        private readonly ILogger<QuizAttemptTimeoutBackgroundService> logger;

        public QuizAttemptTimeoutBackgroundService(IServiceProvider serviceProvider, ILogger<QuizAttemptTimeoutBackgroundService> logger) {

            this.serviceProvider = serviceProvider;
            this.logger = logger;

        }

       protected override async Task ExecuteAsync( CancellationToken stoppingToken) {

            while (!stoppingToken.IsCancellationRequested) {
                try {
                    using var scope = serviceProvider.CreateScope();

                    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();


                    var expiredAttempts = await context.QuizAttempts.
                            Where(x =>
                            x.Status == AttemptStatus.InProgress &&
                            x.Deadline < DateTime.UtcNow)
                        .ToListAsync(stoppingToken);

                    foreach (var attempt in expiredAttempts) {
                        attempt.Status = AttemptStatus.TimedOut;
                        attempt.UpdatedAt = DateTime.UtcNow;
                    }


                    if (expiredAttempts.Any()) 
                        await context.SaveChangesAsync( stoppingToken);

                     
                    
                } catch (Exception ex) {
                    logger.LogError(ex,"Error while checking expired quiz attempts");
                }

                await Task.Delay(TimeSpan.FromMinutes(1),stoppingToken);
            }

        }
    

    }
}
