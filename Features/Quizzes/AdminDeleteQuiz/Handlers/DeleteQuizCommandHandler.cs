using exam_system.Common.Enums;
using exam_system.Domain.Entities.Quizzes;
using exam_system.Features.Quizzes.AdminDeleteQuiz.Commands;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace exam_system.Features.Quizzes.AdminDeleteQuiz.Handlers {
    public class DeleteQuizCommandHandler : IRequestHandler<DeleteQuizCommand, RequestResponse<bool>> {

        private readonly IGenericRepository<Quiz> quizRepository;
        private readonly IUnitOfWork unitOfWork;

        public DeleteQuizCommandHandler(IGenericRepository<Quiz> quizRepository, IUnitOfWork unitOfWork) {
            this.quizRepository = quizRepository;
            this.unitOfWork = unitOfWork;
        }

     

       async Task<RequestResponse<bool>> IRequestHandler<DeleteQuizCommand, RequestResponse<bool>>.Handle(DeleteQuizCommand request, CancellationToken cancellationToken) {

            var quiz = await quizRepository.GetAll().FirstOrDefaultAsync(x => x.Id == request.QuizId &&
                                 !x.IsDeleted, cancellationToken);

            if (quiz is null) {
                return RequestResponse<bool>.Fail("Quiz not found.",StatusCodes.Status404NotFound);
            }

            if (quiz.Status == QuizStatus.Published) {
                return RequestResponse<bool>.Fail("Published quiz cannot be deleted. Unpublish it first.",StatusCodes.Status409Conflict);
            }

            quizRepository.Delete(quiz);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return RequestResponse<bool>.Ok(true,"Quiz deleted successfully.");
        }
    }
}
