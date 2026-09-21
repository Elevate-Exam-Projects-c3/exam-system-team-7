using exam_system.Domain.Entities.Attempts;
using exam_system.Dtos.Attempts;
using exam_system.Features.Analytics.GetAdminAttemptDetail.Queries;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace exam_system.Features.Analytics.GetAdminAttemptDetail.Handlers
{
    public class GetAdminAttemptDetailQueryHandler : IRequestHandler<GetAdminAttemptDetailQuery, RequestResponse<AttemptDetailDto>>
    {
        private readonly IGenericRepository<QuizAttempt> _attemptRepository;

        public GetAdminAttemptDetailQueryHandler(IGenericRepository<QuizAttempt> attemptRepository)
        {
            _attemptRepository = attemptRepository;
        }
        public async Task<RequestResponse<AttemptDetailDto>> Handle(GetAdminAttemptDetailQuery request, CancellationToken cancellationToken)
        {
            var attempt = await _attemptRepository.GetByIdAsync(request.AttemptId);
            if (attempt is null)
                return RequestResponse<AttemptDetailDto>.Fail("Attempt not found");

            var detailDto = await _attemptRepository.GetAll()
                 .Where(a => a.Id == request.AttemptId && !a.IsDeleted)
                 .Select(a => new AttemptDetailDto
                 {
                     AttemptId = a.Id,
                     QuizTitle = a.Quiz.Title,
                     StudentName = a.Student.User.FullName,
                     Status = a.Status.ToString(),
                     Score = a.Score,
                     Passed = a.Passed,
                     StartedAt = a.StartTime,
                     SubmittedAt = a.SubmittedAt,
                     Questions = a.Answers.OrderBy(ans=>ans.Question.OrderIndex)
                                    .Select(ans => new QuestionBreakdownDto
                                    {
                                        QuestionText=ans.Question.Text,
                                        SelectedOptionText =ans.SelectedOption != null
                                        ?ans.SelectedOption.OptionText
                                        :null,
                                        IsCorrect=ans.IsCorrect.Value,
                                        Options=ans.Question.Options
                                                    .Select(o=> new OptionResultDto
                                                    {
                                                        OptionText=o.OptionText,
                                                        IsCorrect =o.IsCorrect
                                                    }).ToList()
                                    }).ToList()
                 }).FirstOrDefaultAsync(cancellationToken);

            
            return RequestResponse<AttemptDetailDto>.Ok(detailDto);

        }
    }
}
