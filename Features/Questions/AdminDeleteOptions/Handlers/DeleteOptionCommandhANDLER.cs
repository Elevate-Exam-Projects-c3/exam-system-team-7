using exam_system.Domain.Entities.Quizzes;
using exam_system.Features.Questions.AdminDeleteOptions.Commands;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace exam_system.Features.Questions.AdminDeleteOptions.Handlers {
    public class DeleteOptionCommandHandler: IRequestHandler<DeleteOptionCommand, RequestResponse<bool>> {

        private readonly IGenericRepository<QuestionOption> optionsRepository;

        public DeleteOptionCommandHandler(IGenericRepository<QuestionOption> optionsRepository) {
            this.optionsRepository = optionsRepository;
        }

        public async Task<RequestResponse<bool>> Handle(DeleteOptionCommand request,CancellationToken cancellationToken) {

            var option = await optionsRepository
                .GetAll()
                .FirstOrDefaultAsync(
                    x =>
                        x.Id == request.optionId &&
                        x.QuestionId == request.questionId &&
                        !x.IsDeleted,
                    cancellationToken);

            if (option == null) 
                return RequestResponse<bool>.Fail("Option not found.",StatusCodes.Status404NotFound);
            

            option.IsDeleted = true;
            option.DeletedAt = DateTime.UtcNow;

            optionsRepository.Update(option);

            return RequestResponse<bool>.Ok(true,"Option deleted successfully.");
        }
    }
}