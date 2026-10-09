using exam_system.Domain.Entities.Quizzes;
using exam_system.Dtos.Quizes;
using exam_system.Features.Quizzes.AdminQuizPublishCheck.Queries;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace exam_system.Features.Quizzes.AdminQuizPublishCheck.Handlers
{
    public class GetQuizInfoQueryHandler: IRequestHandler<GetQuizInfoQuery, RequestResponse<QuizInfoDto>>
    {
        private readonly IGenericRepository<Quiz> _quizRepository;

        public GetQuizInfoQueryHandler(IGenericRepository<Quiz> quizRepository)
        {
            _quizRepository = quizRepository;
        }

        public async Task<RequestResponse<QuizInfoDto>> Handle(GetQuizInfoQuery request, CancellationToken cancellationToken)
        {
            var quizDto = await _quizRepository.Get(q=>q.Id ==request.QuizId && !q.IsDeleted)
                .Select(q=> new QuizInfoDto
                {
                    durationminutes = q.DurationMinutes,
                    passScore = q.PassScore
                }).FirstOrDefaultAsync(cancellationToken);

            if(quizDto is null)
                return RequestResponse<QuizInfoDto>.Fail("Quiz not found");

            return RequestResponse<QuizInfoDto>.Ok(quizDto);
        }
    }
}
