using MediatR;
using Wordle.Application.Scores.DTOs;
using Wordle.Domain.Common.Enums;

namespace Wordle.Application.Scores.Queries.Leaderboard;

public class LeaderboardQuery : IRequest<LeaderboardResponseDto>
{
    public LeaderboardRange Range { get; set; }
}
