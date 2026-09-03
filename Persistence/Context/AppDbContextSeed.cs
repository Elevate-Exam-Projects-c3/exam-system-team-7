using Microsoft.EntityFrameworkCore;
using exam_system.Common.Enums;
using exam_system.Domain.Entities.Identity;
using exam_system.Domain.Entities.Diplomas;
using exam_system.Domain.Entities.Quizzes;
using exam_system.Domain.Entities.Attempts;

namespace exam_system.Persistence.Context;

public static class AppDbContextSeed
{
    public static async Task SeedAsync(AppDbContext context, ILogger logger)
    {
        try
        {
            // 1. Seed Users and Students
            if (!await context.Users.AnyAsync())
            {
                logger.LogInformation("Seeding Users and Students...");

                var adminUser = new ApplicationUser
                {
                    Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    FullName = "System Administrator",
                    Email = "admin@examsystem.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123456", workFactor: 12),
                    Role = UserRole.Admin,
                    AccountStatus = AccountStatus.Active,
                    EmailConfirmed = true,
                    CreatedAt = DateTime.UtcNow.AddMonths(-6)
                };

                var studentUser1 = new ApplicationUser
                {
                    Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    FullName = "John Doe",
                    Email = "john.doe@student.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Student@123456", workFactor: 12),
                    Role = UserRole.Student,
                    AccountStatus = AccountStatus.Active,
                    EmailConfirmed = true,
                    CreatedAt = DateTime.UtcNow.AddMonths(-3)
                };

                var studentUser2 = new ApplicationUser
                {
                    Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                    FullName = "Sarah Connor",
                    Email = "sarah.connor@student.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Student@123456", workFactor: 12),
                    Role = UserRole.Student,
                    AccountStatus = AccountStatus.Active,
                    EmailConfirmed = true,
                    CreatedAt = DateTime.UtcNow.AddMonths(-2)
                };

                var studentUser3 = new ApplicationUser
                {
                    Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                    FullName = "Alex Mercer",
                    Email = "alex.mercer@student.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Student@123456", workFactor: 12),
                    Role = UserRole.Student,
                    AccountStatus = AccountStatus.Pending,
                    EmailConfirmed = false,
                    CreatedAt = DateTime.UtcNow.AddDays(-5)
                };

                await context.Users.AddRangeAsync(adminUser, studentUser1, studentUser2, studentUser3);

                var student1 = new Student
                {
                    Id = Guid.Parse("aaaaaaaa-1111-1111-1111-aaaaaaaaaaaa"),
                    UserId = studentUser1.Id,
                    StudentCode = "STU-2026-0001",
                    PhoneNumber = "+1234567890",
                    CreatedAt = DateTime.UtcNow.AddMonths(-3)
                };

                var student2 = new Student
                {
                    Id = Guid.Parse("bbbbbbbb-2222-2222-2222-bbbbbbbbbbbb"),
                    UserId = studentUser2.Id,
                    StudentCode = "STU-2026-0002",
                    PhoneNumber = "+1987654321",
                    CreatedAt = DateTime.UtcNow.AddMonths(-2)
                };

                var student3 = new Student
                {
                    Id = Guid.Parse("cccccccc-3333-3333-3333-cccccccccccc"),
                    UserId = studentUser3.Id,
                    StudentCode = "STU-2026-0003",
                    PhoneNumber = "+1122334455",
                    CreatedAt = DateTime.UtcNow.AddDays(-5)
                };

                await context.Students.AddRangeAsync(student1, student2, student3);

                var otp = new EmailVerificationOtp
                {
                    Id = Guid.NewGuid(),
                    UserId = studentUser3.Id,
                    Email = studentUser3.Email,
                    OtpHash = BCrypt.Net.BCrypt.HashPassword("123456", workFactor: 12),
                    ExpiresAt = DateTime.UtcNow.AddMinutes(10),
                    AttemptCount = 0,
                    IsUsed = false,
                    CreatedAt = DateTime.UtcNow.AddMinutes(-2)
                };

                var resetOtp = new PasswordResetOtp
                {
                    Id = Guid.NewGuid(),
                    UserId = studentUser2.Id,
                    Email = studentUser2.Email,
                    OtpHash = BCrypt.Net.BCrypt.HashPassword("654321", workFactor: 12),
                    ExpiresAt = DateTime.UtcNow.AddMinutes(8),
                    AttemptCount = 0,
                    IsUsed = false,
                    CreatedAt = DateTime.UtcNow.AddMinutes(-2)
                };

                var refreshToken = new RefreshToken
                {
                    Id = Guid.NewGuid(),
                    UserId = studentUser1.Id,
                    Token = Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N"),
                    ExpiresAt = DateTime.UtcNow.AddDays(7),
                    IsUsed = false,
                    IsRevoked = false,
                    CreatedAt = DateTime.UtcNow
                };

                await context.EmailVerificationOtps.AddAsync(otp);
                await context.PasswordResetOtps.AddAsync(resetOtp);
                await context.RefreshTokens.AddAsync(refreshToken);

                await context.SaveChangesAsync();
            }

            // 2. Seed Diplomas
            if (!await context.Diplomas.AnyAsync())
            {
                logger.LogInformation("Seeding Diplomas, Quizzes, Questions, and Options...");

                var diplomaNet = new Diploma
                {
                    Id = Guid.Parse("d1111111-1111-1111-1111-111111111111"),
                    Title = "Full Stack .NET & Cloud Architecture Diploma",
                    Description = "Comprehensive diploma covering C#, ASP.NET Core, EF Core, Microservices, CQRS, Docker and Azure Cloud deployment.",
                    CreatedAt = DateTime.UtcNow.AddMonths(-4)
                };

                var diplomaFrontend = new Diploma
                {
                    Id = Guid.Parse("d2222222-2222-2222-2222-222222222222"),
                    Title = "Modern Frontend Development with Angular & React",
                    Description = "Master modern web user interfaces using TypeScript, Angular, React, TailwindCSS, State Management and RESTful APIs.",
                    CreatedAt = DateTime.UtcNow.AddMonths(-3)
                };

                var diplomaDevOps = new Diploma
                {
                    Id = Guid.Parse("d3333333-3333-3333-3333-333333333333"),
                    Title = "DevOps & CI/CD Engineering",
                    Description = "Industry-standard practices in GitOps, GitHub Actions, Docker, Kubernetes, Terraform and Infrastructure as Code.",
                    CreatedAt = DateTime.UtcNow.AddMonths(-1)
                };

                await context.Diplomas.AddRangeAsync(diplomaNet, diplomaFrontend, diplomaDevOps);

                // 3. Seed Enrollments
                var enrollment1 = new StudentEnrollment
                {
                    Id = Guid.NewGuid(),
                    StudentId = Guid.Parse("aaaaaaaa-1111-1111-1111-aaaaaaaaaaaa"),
                    DiplomaId = diplomaNet.Id,
                    EnrolledAt = DateTime.UtcNow.AddMonths(-2)
                };

                var enrollment2 = new StudentEnrollment
                {
                    Id = Guid.NewGuid(),
                    StudentId = Guid.Parse("bbbbbbbb-2222-2222-2222-bbbbbbbbbbbb"),
                    DiplomaId = diplomaNet.Id,
                    EnrolledAt = DateTime.UtcNow.AddMonths(-1)
                };

                var enrollment3 = new StudentEnrollment
                {
                    Id = Guid.NewGuid(),
                    StudentId = Guid.Parse("bbbbbbbb-2222-2222-2222-bbbbbbbbbbbb"),
                    DiplomaId = diplomaFrontend.Id,
                    EnrolledAt = DateTime.UtcNow.AddDays(-20)
                };

                await context.Enrollments.AddRangeAsync(enrollment1, enrollment2, enrollment3);

                // 4. Seed Quizzes for .NET Diploma
                var quizCsharp = new Quiz
                {
                    Id = Guid.Parse("01111111-1111-1111-1111-111111111111"),
                    DiplomaId = diplomaNet.Id,
                    Title = "C# Advanced OOP & Memory Management",
                    Instructions = "Answer all questions. You have 30 minutes to complete the test.",
                    DurationMinutes = 30,
                    PassScore = 70,
                    MaxAttempts = 3,
                    Status = QuizStatus.Published,
                    PublishedAt = DateTime.UtcNow.AddMonths(-2),
                    CreatedAt = DateTime.UtcNow.AddMonths(-2)
                };

                var quizEfCore = new Quiz
                {
                    Id = Guid.Parse("02222222-2222-2222-2222-222222222222"),
                    DiplomaId = diplomaNet.Id,
                    Title = "Entity Framework Core & Query Optimization",
                    Instructions = "Test covers change tracker, migrations, indexing, and compiled queries.",
                    DurationMinutes = 45,
                    PassScore = 65,
                    MaxAttempts = 2,
                    Status = QuizStatus.Published,
                    PublishedAt = DateTime.UtcNow.AddMonths(-1),
                    CreatedAt = DateTime.UtcNow.AddMonths(-1)
                };

                var quizArchitecture = new Quiz
                {
                    Id = Guid.Parse("03333333-3333-3333-3333-333333333333"),
                    DiplomaId = diplomaNet.Id,
                    Title = "Vertical Slice Architecture & CQRS Patterns",
                    Instructions = "Draft evaluation for architectural patterns in enterprise systems.",
                    DurationMinutes = 60,
                    PassScore = 75,
                    MaxAttempts = null,
                    Status = QuizStatus.Draft,
                    CreatedAt = DateTime.UtcNow.AddDays(-10)
                };

                var quizTypeScript = new Quiz
                {
                    Id = Guid.Parse("04444444-4444-4444-4444-444444444444"),
                    DiplomaId = diplomaFrontend.Id,
                    Title = "TypeScript Core Types & Generics",
                    Instructions = "30-minute assessment testing generic constraints, keyof, and mapped types.",
                    DurationMinutes = 30,
                    PassScore = 60,
                    MaxAttempts = 3,
                    Status = QuizStatus.Published,
                    PublishedAt = DateTime.UtcNow.AddDays(-15),
                    CreatedAt = DateTime.UtcNow.AddDays(-20)
                };

                await context.Quizzes.AddRangeAsync(quizCsharp, quizEfCore, quizArchitecture, quizTypeScript);

                // 5. Seed Questions & Options for C# Quiz
                var q1 = new Question
                {
                    Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                    QuizId = quizCsharp.Id,
                    Text = "What is the primary difference between a value type (struct) and a reference type (class) in .NET?",
                    Explanation = "Value types reside on the stack or inline in containing structures, whereas reference types allocate on the managed heap with garbage collection tracking.",
                    OrderIndex = 1
                };
                var opt1A = new QuestionOption { Id = Guid.NewGuid(), QuestionId = q1.Id, OptionText = "Value types are allocated on the stack (or inline), reference types on the managed heap.", IsCorrect = true };
                var opt1B = new QuestionOption { Id = Guid.NewGuid(), QuestionId = q1.Id, OptionText = "Value types can inherit from other structs, classes cannot.", IsCorrect = false };
                var opt1C = new QuestionOption { Id = Guid.NewGuid(), QuestionId = q1.Id, OptionText = "Reference types cannot be null, while value types can always be null by default.", IsCorrect = false };
                var opt1D = new QuestionOption { Id = Guid.NewGuid(), QuestionId = q1.Id, OptionText = "There is no performance difference between structs and classes in .NET.", IsCorrect = false };

                var q2 = new Question
                {
                    Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                    QuizId = quizCsharp.Id,
                    Text = "Which keyword in C# ensures that unmanaged resources are deterministically released by invoking Dispose()?",
                    Explanation = "The 'using' statement or declaration ensures Dispose() is called even if exceptions occur.",
                    OrderIndex = 2
                };
                var opt2A = new QuestionOption { Id = Guid.NewGuid(), QuestionId = q2.Id, OptionText = "finally", IsCorrect = false };
                var opt2B = new QuestionOption { Id = Guid.NewGuid(), QuestionId = q2.Id, OptionText = "using", IsCorrect = true };
                var opt2C = new QuestionOption { Id = Guid.NewGuid(), QuestionId = q2.Id, OptionText = "fixed", IsCorrect = false };
                var opt2D = new QuestionOption { Id = Guid.NewGuid(), QuestionId = q2.Id, OptionText = "lock", IsCorrect = false };

                var q3 = new Question
                {
                    Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                    QuizId = quizCsharp.Id,
                    Text = "What does the 'readonly ref struct' declaration guarantee in C#?",
                    Explanation = "It guarantees the struct is both immutable and stack-only, preventing heap boxing or escaping across async boundaries.",
                    OrderIndex = 3
                };
                var opt3A = new QuestionOption { Id = Guid.NewGuid(), QuestionId = q3.Id, OptionText = "It can only be stored in heap memory.", IsCorrect = false };
                var opt3B = new QuestionOption { Id = Guid.NewGuid(), QuestionId = q3.Id, OptionText = "It is allocated on the stack only and cannot be modified after construction.", IsCorrect = true };
                var opt3C = new QuestionOption { Id = Guid.NewGuid(), QuestionId = q3.Id, OptionText = "It allows multi-threaded race conditions safely.", IsCorrect = false };
                var opt3D = new QuestionOption { Id = Guid.NewGuid(), QuestionId = q3.Id, OptionText = "It implements IDisposable automatically.", IsCorrect = false };

                var q4 = new Question
                {
                    Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"),
                    QuizId = quizCsharp.Id,
                    Text = "Which garbage collection generation is optimized for short-lived objects like local variables in .NET?",
                    Explanation = "Generation 0 (Gen 0) is the ephemeral generation where newly allocated objects live.",
                    OrderIndex = 4
                };
                var opt4A = new QuestionOption { Id = Guid.NewGuid(), QuestionId = q4.Id, OptionText = "Generation 0", IsCorrect = true };
                var opt4B = new QuestionOption { Id = Guid.NewGuid(), QuestionId = q4.Id, OptionText = "Generation 1", IsCorrect = false };
                var opt4C = new QuestionOption { Id = Guid.NewGuid(), QuestionId = q4.Id, OptionText = "Generation 2", IsCorrect = false };
                var opt4D = new QuestionOption { Id = Guid.NewGuid(), QuestionId = q4.Id, OptionText = "Large Object Heap (LOH)", IsCorrect = false };

                await context.Questions.AddRangeAsync(q1, q2, q3, q4);
                await context.QuestionOptions.AddRangeAsync(
                    opt1A, opt1B, opt1C, opt1D,
                    opt2A, opt2B, opt2C, opt2D,
                    opt3A, opt3B, opt3C, opt3D,
                    opt4A, opt4B, opt4C, opt4D
                );

                var qEf1 = new Question
                {
                    Id = Guid.NewGuid(),
                    QuizId = quizEfCore.Id,
                    Text = "Which EF Core method disables change tracking to significantly optimize read-only query performance?",
                    Explanation = "AsNoTracking() instructs the context not to retain tracking snapshots for returned entities.",
                    OrderIndex = 1
                };
                var optEf1A = new QuestionOption { Id = Guid.NewGuid(), QuestionId = qEf1.Id, OptionText = "AsNoTracking()", IsCorrect = true };
                var optEf1B = new QuestionOption { Id = Guid.NewGuid(), QuestionId = qEf1.Id, OptionText = "IgnoreTracking()", IsCorrect = false };
                var optEf1C = new QuestionOption { Id = Guid.NewGuid(), QuestionId = qEf1.Id, OptionText = "DisableTracker()", IsCorrect = false };
                var optEf1D = new QuestionOption { Id = Guid.NewGuid(), QuestionId = qEf1.Id, OptionText = "WithoutSnapshot()", IsCorrect = false };

                await context.Questions.AddAsync(qEf1);
                await context.QuestionOptions.AddRangeAsync(optEf1A, optEf1B, optEf1C, optEf1D);

                // 6. Seed Attempts and Answers
                var attempt1 = new QuizAttempt
                {
                    Id = Guid.Parse("01111111-aaaa-1111-1111-111111111111"),
                    StudentId = Guid.Parse("aaaaaaaa-1111-1111-1111-aaaaaaaaaaaa"),
                    QuizId = quizCsharp.Id,
                    Status = AttemptStatus.Submitted,
                    StartTime = DateTime.UtcNow.AddDays(-10).AddMinutes(-30),
                    Deadline = DateTime.UtcNow.AddDays(-10),
                    SubmittedAt = DateTime.UtcNow.AddDays(-10).AddMinutes(-5),
                    Score = 100.0,
                    Passed = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-10)
                };

                var answer1A = new StudentQuestionAnswer
                {
                    Id = Guid.NewGuid(),
                    AttemptId = attempt1.Id,
                    QuestionId = q1.Id,
                    SelectedOptionId = opt1A.Id,
                    IsCorrect = true,
                    AnsweredAt = attempt1.StartTime.AddMinutes(5)
                };
                var answer1B = new StudentQuestionAnswer
                {
                    Id = Guid.NewGuid(),
                    AttemptId = attempt1.Id,
                    QuestionId = q2.Id,
                    SelectedOptionId = opt2B.Id,
                    IsCorrect = true,
                    AnsweredAt = attempt1.StartTime.AddMinutes(10)
                };
                var answer1C = new StudentQuestionAnswer
                {
                    Id = Guid.NewGuid(),
                    AttemptId = attempt1.Id,
                    QuestionId = q3.Id,
                    SelectedOptionId = opt3B.Id,
                    IsCorrect = true,
                    AnsweredAt = attempt1.StartTime.AddMinutes(15)
                };
                var answer1D = new StudentQuestionAnswer
                {
                    Id = Guid.NewGuid(),
                    AttemptId = attempt1.Id,
                    QuestionId = q4.Id,
                    SelectedOptionId = opt4A.Id,
                    IsCorrect = true,
                    AnsweredAt = attempt1.StartTime.AddMinutes(20)
                };

                var attempt2 = new QuizAttempt
                {
                    Id = Guid.Parse("02222222-bbbb-2222-2222-222222222222"),
                    StudentId = Guid.Parse("bbbbbbbb-2222-2222-2222-bbbbbbbbbbbb"),
                    QuizId = quizCsharp.Id,
                    Status = AttemptStatus.InProgress,
                    StartTime = DateTime.UtcNow.AddMinutes(-10),
                    Deadline = DateTime.UtcNow.AddMinutes(20),
                    Score = null,
                    Passed = null,
                    CreatedAt = DateTime.UtcNow.AddMinutes(-10)
                };

                var answer2A = new StudentQuestionAnswer
                {
                    Id = Guid.NewGuid(),
                    AttemptId = attempt2.Id,
                    QuestionId = q1.Id,
                    SelectedOptionId = opt1A.Id,
                    IsCorrect = true,
                    AnsweredAt = DateTime.UtcNow.AddMinutes(-5)
                };

                await context.QuizAttempts.AddRangeAsync(attempt1, attempt2);
                await context.StudentQuestionAnswers.AddRangeAsync(answer1A, answer1B, answer1C, answer1D, answer2A);

                await context.SaveChangesAsync();
                logger.LogInformation("Database seeded successfully with all domain entities!");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database: {Message}", ex.Message);
            throw;
        }
    }
}
