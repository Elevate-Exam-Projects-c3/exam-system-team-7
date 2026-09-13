using exam_system.Domain.Entities.Quizzes;
using exam_system.Features.Questions.AdminCreateQuestion.Commands;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;
using MediatR;

namespace exam_system.Features.Questions.AdminCreateQuestion.Handlers {
    public class CreateQuestionCommandHandler : IRequestHandler<CreateQuestionCommand, RequestResponse<Guid>> {

        private readonly IGenericRepository<Question> questionsRepository;

        private readonly IUnitOfWork unitOfWork;

        public CreateQuestionCommandHandler(IGenericRepository<Question> questionsRepository , IUnitOfWork unitOfWork) {
            this.questionsRepository = questionsRepository;
            this.unitOfWork = unitOfWork;
        }
        public async Task<RequestResponse<Guid>> Handle(CreateQuestionCommand request, CancellationToken cancellationToken) {

            var question = new Question {
                Id = Guid.NewGuid(),
                QuizId = request.QuizId,
                Text = request.Text.Trim(),
                Explanation = request.Explanation?.Trim(),
                OrderIndex = request.OrderIndex,

                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            await questionsRepository.AddAsync(question);

            var result = await unitOfWork.SaveChangesAsync(cancellationToken);

            if (result <= 0)
                return RequestResponse<Guid>.Fail("Question failed to Carete.");


            return RequestResponse<Guid>.Created(question.Id, "Question created successfully.");
        }
    }
}
