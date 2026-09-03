using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using exam_system.Domain.Entities.Identity;
using exam_system.Domain.Entities.Diplomas;
using exam_system.Domain.Entities.Quizzes;
using exam_system.Domain.Entities.Attempts;

namespace exam_system.Persistence.Configurations;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.ToTable("AspNetUsers");
        builder.HasKey(u => u.Id);

        builder.Property(u => u.FullName).HasMaxLength(100).IsRequired();
        builder.Property(u => u.Email).HasMaxLength(256).IsRequired();
        builder.Property(u => u.PasswordHash).IsRequired();

        builder.HasIndex(u => u.Email).IsUnique();

        builder.HasOne(u => u.Student)
            .WithOne(s => s.User)
            .HasForeignKey<Student>(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.ToTable("Students");
        builder.HasKey(s => s.Id);

        builder.Property(s => s.StudentCode).HasMaxLength(50);
        builder.Property(s => s.PhoneNumber).HasMaxLength(20);
    }
}

public class EmailVerificationOtpConfiguration : IEntityTypeConfiguration<EmailVerificationOtp>
{
    public void Configure(EntityTypeBuilder<EmailVerificationOtp> builder)
    {
        builder.ToTable("OtpCodes");
        builder.HasKey(o => o.Id);

        builder.Property(o => o.Email).HasMaxLength(256).IsRequired();
        builder.Property(o => o.OtpHash).IsRequired();

        builder.HasOne(o => o.User)
            .WithMany(u => u.EmailVerificationOtps)
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(o => o.Email);
    }
}

public class PasswordResetOtpConfiguration : IEntityTypeConfiguration<PasswordResetOtp>
{
    public void Configure(EntityTypeBuilder<PasswordResetOtp> builder)
    {
        builder.ToTable("PasswordResetOtps");
        builder.HasKey(o => o.Id);

        builder.Property(o => o.Email).HasMaxLength(256).IsRequired();
        builder.Property(o => o.OtpHash).IsRequired();

        builder.HasOne(o => o.User)
            .WithMany(u => u.PasswordResetOtps)
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(o => o.Email);
    }
}

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshTokens");
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Token).IsRequired();

        builder.HasOne(r => r.User)
            .WithMany(u => u.RefreshTokens)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(r => r.Token).IsUnique();
    }
}

public class DiplomaConfiguration : IEntityTypeConfiguration<Diploma>
{
    public void Configure(EntityTypeBuilder<Diploma> builder)
    {
        builder.ToTable("Diplomas");
        builder.HasKey(d => d.Id);

        builder.Property(d => d.Title).HasMaxLength(200).IsRequired();
        builder.Property(d => d.Description).HasMaxLength(1000);
    }
}

public class StudentEnrollmentConfiguration : IEntityTypeConfiguration<StudentEnrollment>
{
    public void Configure(EntityTypeBuilder<StudentEnrollment> builder)
    {
        builder.ToTable("Enrollments");
        builder.HasKey(e => e.Id);

        builder.HasIndex(e => new { e.StudentId, e.DiplomaId }).IsUnique();

        builder.HasOne(e => e.Student)
            .WithMany(s => s.Enrollments)
            .HasForeignKey(e => e.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Diploma)
            .WithMany(d => d.Enrollments)
            .HasForeignKey(e => e.DiplomaId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class QuizConfiguration : IEntityTypeConfiguration<Quiz>
{
    public void Configure(EntityTypeBuilder<Quiz> builder)
    {
        builder.ToTable("Quizzes");
        builder.HasKey(q => q.Id);

        builder.Property(q => q.Title).HasMaxLength(200).IsRequired();
        builder.Property(q => q.Instructions).HasMaxLength(2000);

        builder.HasOne(q => q.Diploma)
            .WithMany(d => d.Quizzes)
            .HasForeignKey(q => q.DiplomaId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class QuestionConfiguration : IEntityTypeConfiguration<Question>
{
    public void Configure(EntityTypeBuilder<Question> builder)
    {
        builder.ToTable("Questions");
        builder.HasKey(q => q.Id);

        builder.Property(q => q.Text).IsRequired();
        builder.Property(q => q.Explanation).HasMaxLength(1000);

        builder.HasOne(q => q.Quiz)
            .WithMany(qz => qz.Questions)
            .HasForeignKey(q => q.QuizId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class QuestionOptionConfiguration : IEntityTypeConfiguration<QuestionOption>
{
    public void Configure(EntityTypeBuilder<QuestionOption> builder)
    {
        builder.ToTable("QuestionOptions");
        builder.HasKey(o => o.Id);

        builder.Property(o => o.OptionText).HasMaxLength(500).IsRequired();

        builder.HasOne(o => o.Question)
            .WithMany(q => q.Options)
            .HasForeignKey(o => o.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class QuizAttemptConfiguration : IEntityTypeConfiguration<QuizAttempt>
{
    public void Configure(EntityTypeBuilder<QuizAttempt> builder)
    {
        builder.ToTable("QuizAttempts");
        builder.HasKey(a => a.Id);

        builder.HasOne(a => a.Student)
            .WithMany(s => s.QuizAttempts)
            .HasForeignKey(a => a.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.Quiz)
            .WithMany(q => q.Attempts)
            .HasForeignKey(a => a.QuizId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(a => new { a.StudentId, a.QuizId });
    }
}

public class StudentQuestionAnswerConfiguration : IEntityTypeConfiguration<StudentQuestionAnswer>
{
    public void Configure(EntityTypeBuilder<StudentQuestionAnswer> builder)
    {
        builder.ToTable("StudentQuestionAnswers");
        builder.HasKey(a => a.Id);

        builder.HasOne(a => a.Attempt)
            .WithMany(att => att.Answers)
            .HasForeignKey(a => a.AttemptId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(a => a.Question)
            .WithMany(q => q.Answers)
            .HasForeignKey(a => a.QuestionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.SelectedOption)
            .WithMany(o => o.SelectedInAnswers)
            .HasForeignKey(a => a.SelectedOptionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(a => new { a.AttemptId, a.QuestionId }).IsUnique();
    }
}
