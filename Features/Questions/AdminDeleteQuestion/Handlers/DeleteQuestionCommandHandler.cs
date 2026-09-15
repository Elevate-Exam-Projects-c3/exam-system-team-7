using exam_system.Domain.Entities.Quizzes;
using exam_system.Features.Questions.AdminDeleteQuestion.Commands;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace exam_system.Features.Questions.AdminDeleteQuestion.Handlers {
    public class DeleteQuestionCommandHandler : IRequestHandler<DeleteQuestionCommand, RequestResponse<bool>> {

        private readonly IGenericRepository<Question> questionRepository;
        private readonly IUnitOfWork unitOfWork;

        public DeleteQuestionCommandHandler(IGenericRepository<Question> questionRepository, IUnitOfWork unitOfWork) {
            this.questionRepository = questionRepository;
            this.unitOfWork = unitOfWork;
        }
        public async Task<RequestResponse<bool>> Handle(DeleteQuestionCommand request, CancellationToken cancellationToken) {
            var question = await questionRepository.GetAll()
                .FirstOrDefaultAsync(q => q.Id == request.questionId, cancellationToken);

            if (question == null) 
                return RequestResponse<bool>.Fail("Question not found" , StatusCodes.Status404NotFound);
 

            question.IsDeleted = true;
            question.DeletedAt = DateTime.UtcNow;

            await questionRepository.UpdateAsync(question);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return RequestResponse<bool>.Ok(true);
        }
    }
}
