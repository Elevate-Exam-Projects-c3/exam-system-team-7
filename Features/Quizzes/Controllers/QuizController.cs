using exam_system.Common.Enums;
using exam_system.Dtos.Quizes;
using exam_system.Features.Quizzes.AdminCreateQuiz.Commands;
using exam_system.Features.Quizzes.AdminUpdateQuiz.Commands;
using exam_system.Features.Shared;
using MediatR;
using Microsoft.AspNetCore.Authorization;
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
        //[Authorize(Roles = nameof(UserRole.Admin))]
        public async Task<ActionResult<EndpointResponse<Guid>>> CreateQuiz([FromBody] CreateQuizDto createQuiz, CancellationToken cancellationToken) {
            try {
                return Ok(new EndpointResponse<Guid>(
                    success: true,
                    statusCode: StatusCodes.Status201Created,
                    message: "Quiz created successfully.",
                    data: await mediator.Send(new CreateQuizCommand(createQuiz), cancellationToken)
                ));
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
        //[Authorize(Roles = nameof(UserRole.Admin))]
        public async Task<ActionResult<EndpointResponse<Guid>>> UpdateQuiz(Guid quizId, [FromBody] UpdateQuizDto updateQuiz, CancellationToken cancellationToken) {

            try {
                if (updateQuiz is null) {
                    var errorResponse = new EndpointResponse<Guid>(
                        success: false,
                        statusCode: StatusCodes.Status400BadRequest,
                        message: "Quiz data is required."
                    );
                    return BadRequest(errorResponse);
                }
                return Ok(new EndpointResponse<Guid>(
                            success: true,
                            statusCode: StatusCodes.Status200OK,
                            message: "Quiz updated successfully.",
                            data: await mediator.Send(new UpdateQuizCommand(quizId, updateQuiz), cancellationToken)));
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
    }
}