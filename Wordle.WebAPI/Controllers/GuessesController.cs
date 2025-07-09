using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wordle.Application.Guesses.Commands.Create;
using Wordle.Application.Scores.DTOs;
using Wordle.Application.Scores.Queries.Leaderboard;
using Wordle.Domain.Common.Enums;

namespace Wordle.WebAPI.Controllers;

[Authorize(Roles = "Player")]
[ApiController]
[Route("api/[controller]")]
public class GuessesController : ControllerBase
{
    private readonly ISender _mediator;

    public GuessesController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize(Roles = "Player")]
    public async Task<IActionResult> Create([FromBody] CreateGuessCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [AllowAnonymous]
    [HttpGet("leaderboard")]
    public async Task<ActionResult<LeaderboardResponseDto>> GetLeaderboard([FromQuery] LeaderboardRange range)
    {
        var result = await _mediator.Send(new LeaderboardQuery { Range = range });
        return Ok(result);
    }


}
