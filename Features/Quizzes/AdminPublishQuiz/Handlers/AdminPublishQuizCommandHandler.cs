using exam_system.Common.Enums;
using exam_system.Domain.Entities.Quizzes;
using exam_system.Features.Quizzes.AdminPublishQuiz.Commands;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;
using MediatR;

namespace exam_system.Features.Quizzes.AdminPublishQuiz.Handlers
{
    public class AdminPublishQuizCommandHandler : IRequestHandler<AdminPublishQuizCommand, RequestResponse<Guid>>
    {
        private readonly Persistence.DataAccess.IGenericRepository<Quiz> _quizRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AdminPublishQuizCommandHandler(IGenericRepository<Quiz> quizRepository , IUnitOfWork unitOfWork)
        {
            _quizRepository = quizRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<RequestResponse<Guid>> Handle(AdminPublishQuizCommand request, CancellationToken cancellationToken)
        {
            var quiz = await _quizRepository.GetByIdAsync(request.QuizId);
            if (quiz == null)
            {
                return RequestResponse<Guid>.Fail("Quiz not found");
            }
            quiz.Status = QuizStatus.Published;
            quiz.PublishedAt = DateTime.Now;

            _quizRepository.SaveInclude(quiz,nameof(quiz.Status),nameof(quiz.PublishedAt));
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return RequestResponse<Guid>.Ok(quiz.Id,"Quiz published successfully");
        }
    }
}
