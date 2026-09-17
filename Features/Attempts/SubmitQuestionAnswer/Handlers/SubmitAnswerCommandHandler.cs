using exam_system.Domain.Entities.Attempts;
using exam_system.Features.Attempts.SubmitQuestionAnswer.Commands;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace exam_system.Features.Attempts.SubmitQuestionAnswer.Handlers {
    public class SubmitAnswerCommandHandler : IRequestHandler<SubmitAnswerCommand, RequestResponse<bool>> {

        private readonly IGenericRepository<StudentQuestionAnswer> studentQuestionAnswerRepository; 
        public SubmitAnswerCommandHandler(IGenericRepository<StudentQuestionAnswer> studentQuestionAnswerRepository) {
             this.studentQuestionAnswerRepository = studentQuestionAnswerRepository;
        }
        public async Task<RequestResponse<bool>> Handle(SubmitAnswerCommand request, CancellationToken cancellationToken) {

            var existingAnswer = await studentQuestionAnswerRepository.GetAll()
                           .FirstOrDefaultAsync(
                               x => x.AttemptId == request.AttemptId &&
                                    x.QuestionId == request.QuestionId &&
                                    !x.IsDeleted,
                               cancellationToken);


            if (existingAnswer != null) {
                existingAnswer.SelectedOptionId =request.SelectedOptionId;
                existingAnswer.AnsweredAt = DateTime.UtcNow;
                studentQuestionAnswerRepository.Update(existingAnswer);
                return RequestResponse<bool>.Ok(true,"Answer updated successfully.");
            }

            var newAnswer = new StudentQuestionAnswer {
                AttemptId = request.AttemptId,
                QuestionId = request.QuestionId,
                SelectedOptionId = request.SelectedOptionId,
                AnsweredAt = DateTime.UtcNow
            };

            studentQuestionAnswerRepository.Update(newAnswer);
            return RequestResponse<bool>.Ok(true, "Answer submitted successfully.");
        }
    }
}
