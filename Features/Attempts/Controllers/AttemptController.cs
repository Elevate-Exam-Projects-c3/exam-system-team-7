using exam_system.Common.Enums;
using exam_system.Dtos.Quizes;
using exam_system.Features.Attempts.StartAttempt.Orchestrators;
using exam_system.Features.Shared;
using exam_system.ViewModels.Options;
using exam_system.ViewModels.Questions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace exam_system.Features.Attempts.Controllers {

    [ApiController]
    [Route("api/[controller]")]
    public class AttemptController : ControllerBase {

        private readonly IMediator mediator;
        public AttemptController(IMediator mediator) {
            this.mediator = mediator;
        }


        [HttpPost("{quizId:guid}/start-quiz")]
        //[Authorize(Roles = nameof(UserRole.Student))]

        public async Task<ActionResult<EndpointResponse<StartQuizViewModel>>> StartQuiz(Guid quizId,CancellationToken cancellationToken) {
            var studentId = Guid.Parse("AAAAAAAA-1111-1111-1111-AAAAAAAAAAAA");

            var result = await mediator.Send(new StartQuizOrchestrator(studentId,quizId),cancellationToken);

            if (!result.Success || result.Data == null) 
                return StatusCode(result.StatusCode,EndpointResponse<StartQuizViewModel>
                    .FromResult(RequestResponse<StartQuizViewModel>.Fail(result.Message,result.StatusCode)));
            

            var viewModel = new StartQuizViewModel {
                AttemptId = result.Data.AttemptId,
                StartTime = result.Data.StartTime.Value,
                Deadline = result.Data.Deadline,

                Questions = result.Data.Questions
                    .Select(q => new QuestionViewModel {
                        Id = q.Id,
                        Text = q.Text,

                        Options = q.Options
                            .Select(o => new QuizOptionViewModel {
                                Id = o.Id,
                                OptionText = o.OptionText
                            })
                            .ToList()
                    })
                    .ToList()
            };

            return StatusCode(result.StatusCode,EndpointResponse<StartQuizViewModel>.
                FromResult(RequestResponse<StartQuizViewModel>.Ok(viewModel,result.Message)));
        }
    }
}
