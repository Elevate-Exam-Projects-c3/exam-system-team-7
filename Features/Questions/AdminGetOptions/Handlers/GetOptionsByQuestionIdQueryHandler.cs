using exam_system.Domain.Entities.Quizzes;
using exam_system.Dtos.Options;
using exam_system.Features.Questions.AdminGetOptions.Queries;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace exam_system.Features.Questions.AdminGetOptions.Handlers {
    public class GetOptionsByQuestionIdQueryHandler: IRequestHandler<GetOptionsByQuestionIdQuery,RequestResponse<List<OptionDto>>> {

        private readonly IGenericRepository<QuestionOption> optionsRepository;

        public GetOptionsByQuestionIdQueryHandler(IGenericRepository<QuestionOption> optionsRepository) {

            this.optionsRepository = optionsRepository;
        }

        public async Task<RequestResponse<List<OptionDto>>> Handle(
            GetOptionsByQuestionIdQuery request,
            CancellationToken cancellationToken) {
            var options = await optionsRepository
                .GetAll()
                .Where(o =>
                    o.QuestionId == request.questionId &&
                    !o.IsDeleted)
                .Select(o => new OptionDto {
                    Id = o.Id,
                    OptionText = o.OptionText,
                    IsCorrect = o.IsCorrect
                })
                .ToListAsync(cancellationToken);

            if (options.Count == 0) 
                return RequestResponse<List<OptionDto>>.Ok(options,"Options list is empty.");
            

            return RequestResponse<List<OptionDto>>.Ok(options,"Options retrieved successfully.");
        }
    }
}