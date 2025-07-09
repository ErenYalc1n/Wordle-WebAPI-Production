using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Wordle.Application.Common.Interfaces;
using Wordle.Application.Scores.DTOs;
using Wordle.Domain.Common.Enums;

namespace Wordle.Application.Scores.Queries.Leaderboard;

public class LeaderboardQueryHandler : IRequestHandler<LeaderboardQuery, LeaderboardResponseDto>
{
    private readonly IScoreRepository _scoreRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public LeaderboardQueryHandler(IScoreRepository scoreRepository, IHttpContextAccessor httpContextAccessor)
    {
        _scoreRepository = scoreRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<LeaderboardResponseDto> Handle(LeaderboardQuery request, CancellationToken cancellationToken)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        DateOnly fromDate = request.Range switch
        {
            LeaderboardRange.Weekly => today.AddDays(-7),
            LeaderboardRange.Monthly => today.AddMonths(-1),
            LeaderboardRange.Yearly => today.AddYears(-1),
            _ => throw new ArgumentOutOfRangeException()
        };

        var toDate = today;

        var topScores = await _scoreRepository.GetTopScoresAsync(fromDate, toDate, 10);

        var userIdStr = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        Guid.TryParse(userIdStr, out var userId);

        var myScore = userId != Guid.Empty
            ? await _scoreRepository.GetUserScoreAsync(userId, fromDate, toDate)
            : null;

        return new LeaderboardResponseDto
        {
            Top10 = topScores
                .Select((s, index) => new LeaderboardEntryDto
                {
                    Rank = index + 1,
                    Nickname = s.User.Nickname,
                    Point = s.Point
                })
                .ToList(),
            CurrentUser = myScore is null
                ? null
                : new LeaderboardEntryDto
                {
                    Nickname = myScore.User.Nickname,
                    Point = myScore.Point,
                    Rank = -1 
                }
        };
    }
}
