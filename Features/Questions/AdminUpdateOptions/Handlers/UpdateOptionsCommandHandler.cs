using exam_system.Domain.Entities.Quizzes;
using exam_system.Features.Questions.AdminUpdateOptions.Commands;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;
using MediatR;

namespace exam_system.Features.Questions.AdminUpdateOptions.Handlers {
    public class UpdateOptionsCommandHandler: IRequestHandler<UpdateOptionsCommand, RequestResponse<Guid>> {

        private readonly IGenericRepository<QuestionOption> optionsRepository;

        public UpdateOptionsCommandHandler(IGenericRepository<QuestionOption> optionsRepository) {

            this.optionsRepository = optionsRepository;
        }

        public async Task<RequestResponse<Guid>> Handle(UpdateOptionsCommand request,CancellationToken cancellationToken) {

            var existingOption = optionsRepository
                .GetAll()
                .FirstOrDefault(o =>
                    o.Id == request.optionId &&
                    o.QuestionId == request.questionId &&
                    !o.IsDeleted);

            if (existingOption == null) 
                return RequestResponse<Guid>.Fail("Option not found.",StatusCodes.Status404NotFound);
            

            existingOption.OptionText = request.OptionText.Trim();

            existingOption.IsCorrect = request.IsCorrect;

            optionsRepository.Update(existingOption);

            return RequestResponse<Guid>.Ok(existingOption.Id,"Option updated successfully.");
        }
    }
}