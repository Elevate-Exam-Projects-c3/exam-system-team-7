using exam_system.Domain.Entities.Quizzes;
using exam_system.Features.Questions.AdminAddOptions.Commands;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;
using MediatR;

namespace exam_system.Features.Questions.AdminCreateOptions.Handlers {
    public class CreateOptionsCommandHandler : IRequestHandler<CreateOptionsCommand, RequestResponse<Guid>> {

        private readonly IGenericRepository<QuestionOption> optionsRepository;
        private readonly IUnitOfWork unitOfWork;

        public CreateOptionsCommandHandler(IGenericRepository<QuestionOption> optionsRepository , 
            IUnitOfWork unitOfWork) {
            this.optionsRepository = optionsRepository;
            this.unitOfWork = unitOfWork;
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

            var result = await unitOfWork.SaveChangesAsync();


            if (result <= 0)
                return RequestResponse<Guid>.Fail("Options failed to Create.");


            return RequestResponse<Guid>.Ok(newOption.Id, "Option created successfully.");
        }
    }
}
