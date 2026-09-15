using exam_system.Domain.Entities.Quizzes;
using exam_system.Features.Questions.AdminUpdateOptions.Commands;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace exam_system.Features.Questions.AdminUpdateOptions.Handlers {
    public class UpdateOptionsCommandHandler: IRequestHandler<UpdateOptionsCommand, RequestResponse<Guid>> {

        private readonly IGenericRepository<QuestionOption> optionsRepository;

        public UpdateOptionsCommandHandler(IGenericRepository<QuestionOption> optionsRepository) {

            this.optionsRepository = optionsRepository;
        }

        public async Task<RequestResponse<Guid>> Handle(UpdateOptionsCommand request,CancellationToken cancellationToken) {

            if (request.optionId == Guid.Empty) {
                var newOption = new QuestionOption {
                    QuestionId = request.questionId,
                    OptionText = request.OptionText.Trim(),
                    IsCorrect = request.IsCorrect,
                };

                await optionsRepository.AddAsync(newOption);

                return RequestResponse<Guid>.Ok(newOption.Id,"Option created successfully.");
            }

            var existingOption = await optionsRepository.GetAll().FirstOrDefaultAsync(o =>
                        o.Id == request.optionId &&
                        o.QuestionId == request.questionId &&
                        !o.IsDeleted,cancellationToken);

            if (existingOption == null) {
                return RequestResponse<Guid>.Fail("Option not found.",StatusCodes.Status404NotFound);
            }

            existingOption.OptionText =request.OptionText.Trim();
            existingOption.IsCorrect =request.IsCorrect;

            optionsRepository.Update(existingOption);

            return RequestResponse<Guid>.Ok(existingOption.Id,"Option updated successfully.");
        }
    }
}