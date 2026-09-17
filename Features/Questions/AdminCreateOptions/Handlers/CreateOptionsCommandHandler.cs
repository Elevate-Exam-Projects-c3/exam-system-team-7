using exam_system.Domain.Entities.Quizzes;
using exam_system.Features.Questions.AdminAddOptions.Commands;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;
using MediatR;

namespace exam_system.Features.Questions.AdminCreateOptions.Handlers {
    public class CreateOptionsCommandHandler : IRequestHandler<CreateOptionsCommand, RequestResponse<bool>> {

        private readonly IGenericRepository<QuestionOption> optionsRepository;

        public CreateOptionsCommandHandler(IGenericRepository<QuestionOption> optionsRepository) { 
            this.optionsRepository = optionsRepository;
        }


        public async Task<RequestResponse<bool>> Handle(CreateOptionsCommand request, CancellationToken cancellationToken) {


            if (request.Options == null || request.Options.Count < 2) 
                return RequestResponse<bool>.Fail("At least two options are required.",StatusCodes.Status400BadRequest);

             var options = request.Options.Select(optionDto =>
                new QuestionOption {
                    QuestionId = request.questionId,
                    OptionText = optionDto.OptionText.Trim(),
                    IsCorrect = optionDto.IsCorrect,
                }).ToList();


            await optionsRepository.AddRangeAsync(options);


            return RequestResponse<bool>.Created(true, "Option created successfully.");
        }
    }
}
