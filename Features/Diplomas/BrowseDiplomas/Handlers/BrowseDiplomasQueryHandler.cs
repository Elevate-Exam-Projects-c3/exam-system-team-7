using exam_system.Common.Enums;
using exam_system.Domain.Entities.Diplomas;
using exam_system.Dtos.Diploma.BrowseDiplomas;
using exam_system.Features.Diplomas.BrowseDiplomas.Queries;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace exam_system.Features.Diplomas.BrowseDiplomas.Handlers
{
    public class BrowseDiplomasQueryHandler : IRequestHandler<BrowseDiplomasQuery, RequestResponse<PaginatedResult<BrowseDiplomaItemDto>>>
    {
        private readonly IGenericRepository<Diploma> _diplomaRepository;
        private readonly ICurrentUserService _currentUserService;

        public BrowseDiplomasQueryHandler(IGenericRepository<Diploma> diplomaRepository)
        {
            _diplomaRepository = diplomaRepository;
        }

        public async Task<RequestResponse<PaginatedResult<BrowseDiplomaItemDto>>> Handle(BrowseDiplomasQuery request, CancellationToken cancellationToken)
        {
            var diplomas = _diplomaRepository.GetAll().AsNoTracking().Where(d => d.IsDeleted == false &&
                                                                                 d.Quizzes.Any(q => !q.IsDeleted && q.Status == Common.Enums.QuizStatus.Published));

            var items = await diplomas.OrderBy(d => d.Id)
                                   .Skip((request.RequestDto.PageIndex - 1) * request.RequestDto.PageSize)
                                   .Take(request.RequestDto.PageSize)
                                   .Select(d => new BrowseDiplomaItemDto
                                   {
                                       Id = d.Id,
                                       Title = d.Title,
                                       Description = d.Description,
                                       TotalQuizzes = d.Quizzes.Count(q => !q.IsDeleted && q.Status == QuizStatus.Published),
                                       // if this number related to current user i must edit this part to count only the quizzes that the current user has completed
                                       CompletedQuizzes = d.Quizzes.Count(q => !q.IsDeleted && q.Status == QuizStatus.Published 
                                                                                            && q.Attempts.Any(a => !a.IsDeleted && a.StudentId == request.CurrentUserId))
                                   }).ToListAsync(cancellationToken);


            var result = PaginatedResult<BrowseDiplomaItemDto>.Create(items, await diplomas.CountAsync(cancellationToken), request.RequestDto.PageIndex, request.RequestDto.PageSize);
            return RequestResponse<PaginatedResult<BrowseDiplomaItemDto>>.Ok(result, "Diplomas retrieved successfully");
        }
    }
}
