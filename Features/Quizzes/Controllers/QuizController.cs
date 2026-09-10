using exam_system.Features.Quizzes.AdminCreateQuiz.Commands;
using exam_system.Features.Quizzes.AdminDeleteQuiz.Commands;
using exam_system.Features.Quizzes.AdminUpdateQuiz.Commands;
using exam_system.Features.Shared;
using exam_system.ViewModels.Quizes;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace exam_system.Features.Quizzes.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class QuizController : ControllerBase {

        private readonly IMediator mediator;
        public QuizController(IMediator mediator) {
            this.mediator = mediator;
        }


        [HttpPost]
        // [Authorize(Roles = nameof(UserRole.Admin))]
        public async Task<ActionResult<EndpointResponse<Guid>>> CreateQuiz([FromBody] CreateQuizViewModel createQuiz,CancellationToken cancellationToken) {
            try {
                var result = await mediator.Send(
                    new CreateQuizCommand(
                        createQuiz.DiplomaId,
                        createQuiz.Title,
                        createQuiz.Instructions,
                        createQuiz.DurationMinutes,
                        createQuiz.PassScore,
                        createQuiz.MaxAttempts,
                        createQuiz.StartDate,
                        createQuiz.EndDate
                    ),
                    cancellationToken
                );

                return StatusCode(
                    result.StatusCode,
                    EndpointResponse<Guid>.FromResult(result)
                );
            } catch (FluentValidation.ValidationException ex) {
                var errors = ex.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray()
                    );

                return BadRequest(new EndpointResponse<Guid>(
                    success: false,
                    statusCode: StatusCodes.Status400BadRequest,
                    message: "Validation failed.",
                    data: default,
                    errors: errors
                ));
            }
        }

    
        [HttpPut("{quizId:guid}")]
        // [Authorize(Roles = nameof(UserRole.Admin))]
        public async Task<ActionResult<EndpointResponse<Guid>>> UpdateQuiz(Guid quizId,[FromBody] UpdateQuizViewModel updateQuiz,CancellationToken cancellationToken) {
            try {
                var command = new UpdateQuizCommand(
                    quizId,
                    updateQuiz.Title,
                    updateQuiz.Instructions,
                    updateQuiz.DurationMinutes,
                    updateQuiz.PassScore,
                    updateQuiz.MaxAttempts,
                    updateQuiz.StartDate,
                    updateQuiz.EndDate
                );

                var result = await mediator.Send(
                    command,
                    cancellationToken
                );

                return StatusCode(
                    result.StatusCode,
                    EndpointResponse<Guid>.FromResult(result)
                );
            } catch (FluentValidation.ValidationException ex) {
                var errors = ex.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray()
                    );

                return BadRequest(
                    new EndpointResponse<Guid>(
                        success: false,
                        statusCode: StatusCodes.Status400BadRequest,
                        message: "Validation failed.",
                        data: default,
                        errors: errors
                    )
                );
            }
        }

        [HttpDelete("{quizId:guid}")]
        // [Authorize(Roles = nameof(UserRole.Admin))]
        public async Task<ActionResult<EndpointResponse<bool>>> DeleteQuiz(Guid quizId, CancellationToken cancellationToken) {
            var result = await mediator.Send(new DeleteQuizCommand(quizId),cancellationToken);

            return StatusCode(result.StatusCode,EndpointResponse<bool>.FromResult(result));
        }
    }
}