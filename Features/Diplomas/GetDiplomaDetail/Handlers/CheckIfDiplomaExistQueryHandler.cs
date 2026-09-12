using exam_system.Domain.Entities.Diplomas;
using exam_system.Domain.Entities.Quizzes;
using exam_system.Features.Diplomas.GetDiplomaDetail.Queries;
using exam_system.Features.Quizzes.GetQuiz.Queries;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace exam_system.Features.Diplomas.GetDiplomaDetail.Handlers {
    public class CheckIfDiplomaExistQueryHandler: IRequestHandler<CheckIfDiplomaExistQuery, RequestResponse<bool>> {

        private readonly IGenericRepository<Diploma> diplomaRepository;

        public CheckIfDiplomaExistQueryHandler(IGenericRepository<Diploma> diplomaRepository) {
            this.diplomaRepository = diplomaRepository;
        }

     

        async Task<RequestResponse<bool>> IRequestHandler<CheckIfDiplomaExistQuery, RequestResponse<bool>>.Handle(CheckIfDiplomaExistQuery request, CancellationToken cancellationToken) {
            return RequestResponse<bool>.Ok( await diplomaRepository.GetAll().AnyAsync(q => q.Id == request.DiplomaId && !q.IsDeleted, cancellationToken),
                "Diploma existence checked successfully."
            );
        }
    }
}
