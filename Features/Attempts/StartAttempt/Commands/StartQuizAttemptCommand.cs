using exam_system.Features.Shared;
using MediatR;

namespace exam_system.Features.Attempts.StartAttempt.Commands {
    public record StartQuizAttemptCommand  (Guid QuizId,Guid StudentId  , int DurationMinutes) : IRequest<RequestResponse<Guid>> { 
    }
    
}
