using exam_system.Common.Enums;
using exam_system.Domain.Entities.Attempts;
using exam_system.Features.Attempts.GetOldAttempt.Queries;
using exam_system.Features.Shared;
using exam_system.Persistence.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace exam_system.Features.Attempts.GetOldAttempt.Handlers {
    public class CheckExistingAttemptQueryHandler  :IRequestHandler<CheckExistingAttemptQuery, RequestResponse<Guid?>>{

        private readonly IGenericRepository<QuizAttempt> attemptRepository;

        public CheckExistingAttemptQueryHandler(IGenericRepository<QuizAttempt> attemptRepository) {
            this.attemptRepository = attemptRepository;
        }

        public async Task<RequestResponse<Guid?>> Handle(CheckExistingAttemptQuery request, CancellationToken cancellationToken){

            var oldAttempt = await attemptRepository.GetAll()
                .FirstOrDefaultAsync
                (a => 
                a.QuizId == request.QuizId &&
                a.StudentId == request.StudentId &&
                a.Status == AttemptStatus.InProgress && 
                a.Deadline > DateTime.UtcNow
                );

            if (oldAttempt == null) 
                return RequestResponse<Guid?>.Ok( null ,"No active attempt found.");
            

            return RequestResponse<Guid?>.Ok(oldAttempt.Id,"Existing attempt found.");
        }

       
    }
}
