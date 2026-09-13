using exam_system.Domain.Entities.Quizzes;
using exam_system.Features.Questions.AdminGetQuestion.Queries;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace exam_system.Features.Questions.AdminGetQuestion.Handlers {
    public class CheckIfQuestionExistQueryHandler : IRequestHandler<CheckIfQuestionExistQuery, RequestResponse<bool>> {

        private readonly IGenericRepository<Question> questionRepository;

        public CheckIfQuestionExistQueryHandler(IGenericRepository<Question> questionRepository) {
            
            this.questionRepository = questionRepository;
        }


        async Task<RequestResponse<bool>> IRequestHandler<CheckIfQuestionExistQuery, RequestResponse<bool>>.Handle(CheckIfQuestionExistQuery request, CancellationToken cancellationToken) {
            return RequestResponse<bool>.Ok(await questionRepository.GetAll().AnyAsync(q => q.Id == request.QuestionId &&
                                                !q.IsDeleted, cancellationToken),
                            "Question existence checked successfully.");
        }
        
    }
}
