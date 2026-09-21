using exam_system.Dtos.Attempts;
using exam_system.Dtos.Quizes;
using exam_system.Features.Attempts.CheckRemainingTime.Orchestrators;
using exam_system.Features.Attempts.GetAttemptHistory.Queries;
using exam_system.Features.Attempts.StartAttempt.Orchestrators;
using exam_system.Features.Shared;
using exam_system.ViewModels.Options;
using exam_system.ViewModels.Questions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace exam_system.Features.Attempts.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AttemptController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AttemptController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("{quizId:guid}/start-quiz")]
        //[Authorize(Roles = nameof(UserRole.Student))]
        public async Task<ActionResult<EndpointResponse<StartQuizViewModel>>> StartQuiz(Guid quizId, CancellationToken cancellationToken)
        {
            var studentId = Guid.Parse("AAAAAAAA-1111-1111-1111-AAAAAAAAAAAA");

            var result = await _mediator.Send(new StartQuizOrchestrator(studentId, quizId), cancellationToken);

            if (!result.Success || result.Data == null)
            {
                return StatusCode(result.StatusCode, EndpointResponse<StartQuizViewModel>
                    .FromResult(RequestResponse<StartQuizViewModel>.Fail(result.Message, result.StatusCode)));
            }

            var viewModel = new StartQuizViewModel
            {
                AttemptId = result.Data.AttemptId,
                StartTime = result.Data.StartTime.Value,
                Deadline = result.Data.Deadline,
                Questions = result.Data.Questions
                    .Select(q => new QuestionViewModel
                    {
                        Id = q.Id,
                        Text = q.Text,
                        Options = q.Options
                            .Select(o => new QuizOptionViewModel
                            {
                                Id = o.Id,
                                OptionText = o.OptionText
                            })
                            .ToList()
                    })
                    .ToList()
            };

            return StatusCode(result.StatusCode, EndpointResponse<StartQuizViewModel>
                .FromResult(RequestResponse<StartQuizViewModel>.Ok(viewModel, result.Message)));
        }

        [HttpGet("{attemptId:guid}/time-remaining")]
        public async Task<ActionResult<EndpointResponse<AttemptTimeRemainingDto>>> GetTimeRemaining(Guid attemptId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetAttemptTimeRemainingOrchestrator(attemptId), cancellationToken);

            return StatusCode(result.StatusCode, EndpointResponse<AttemptTimeRemainingDto>.FromResult(result));
        }

        [HttpGet("history")]
        public async Task<ActionResult<EndpointResponse<PaginatedResult<AttemptHistoryRecordsDto>>>> GetAttemptHistory(
            [FromQuery] int pageIndex, CancellationToken cancellationToken)
        {
            var studentId = Guid.Parse("AAAAAAAA-1111-1111-1111-AAAAAAAAAAAA");
            var result = await _mediator.Send(
                new GetAttemptHistoryQuery(studentId, new RequestPageDetailsDto { PageIndex = pageIndex }), cancellationToken);

            return StatusCode(result.StatusCode, EndpointResponse<PaginatedResult<AttemptHistoryRecordsDto>>.FromResult(result));
        }
    }
}