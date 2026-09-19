using exam_system.Domain.Entities.Quizzes;
using exam_system.Features.Questions.AdminAddOptions.Commands;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;
using MediatR;

namespace exam_system.Features.Questions.AdminCreateOptions.Handlers {
    public class CreateOptionsCommandHandler : IRequestHandler<CreateOptionsCommand, RequestResponse<Guid>> {

        private readonly IGenericRepository<QuestionOption> optionsRepository;

        public CreateOptionsCommandHandler(IGenericRepository<QuestionOption> optionsRepository) { 
            this.optionsRepository = optionsRepository;
        }


        public async Task<RequestResponse<Guid>> Handle(CreateOptionsCommand request, CancellationToken cancellationToken) {

            var newOption = new QuestionOption {

                QuestionId = request.QuestionId,
                OptionText = request.OptionText,
                IsCorrect = request.IsCorrect,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            await optionsRepository.AddAsync(newOption);


            return RequestResponse<Guid>.Created(newOption.Id, "Option created successfully.");
        }
    }
}
