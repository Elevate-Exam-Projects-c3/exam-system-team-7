using exam_system.Common.Enums;
using exam_system.Domain.Entities.Quizzes;
using exam_system.Features.Quizzes.AdminUnpublishQuiz.Commands;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;
using MediatR;

namespace exam_system.Features.Quizzes.AdminUnpublishQuiz.Handlers
{
    public class AdminUnpublishQuizCommandHandler : IRequestHandler<AdminUnpublishQuizCommand, RequestResponse<Guid>>
    {
        private readonly IGenericRepository<Quiz> _quizRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AdminUnpublishQuizCommandHandler(IGenericRepository<Quiz> quizRepository , IUnitOfWork unitOfWork)
        {
            _quizRepository = quizRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<RequestResponse<Guid>> Handle(AdminUnpublishQuizCommand request, CancellationToken cancellationToken)
        {
            var quiz=await _quizRepository.GetByIdAsync(request.QuizId);
            if (quiz is null)
                return RequestResponse<Guid>.Fail("Quiz not found");
            if (quiz.Status != QuizStatus.Published)
                return RequestResponse<Guid>.Fail("Quiz is not published yet");

            quiz.Status = QuizStatus.Draft;
            _quizRepository.SaveInclude(quiz, nameof(quiz.Status));
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return RequestResponse<Guid>.Ok(quiz.Id, "Quiz unpublished successfully");

        }
    }
}
