using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Wordle.Application.Common.Interfaces;
using Wordle.Application.Scores.DTOs;
using Wordle.Application.Scores.Queries.GetMyScore;
using Wordle.Application.Scores.Queries.Leaderboard;

namespace Wordle.Application.Scores.Queries.GetMyScore;

public class GetMyScoreQueryHandler : IRequestHandler<GetMyScoreQuery, MyScoreDto>
{
    private readonly IScoreRepository _scoreRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetMyScoreQueryHandler(IScoreRepository scoreRepository, IHttpContextAccessor httpContextAccessor)
    {
        _scoreRepository = scoreRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<MyScoreDto> Handle(GetMyScoreQuery request, CancellationToken cancellationToken)
    {
        var userIdStr = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrWhiteSpace(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
            throw new UnauthorizedAccessException("Kullanıcı kimliği doğrulanamadı.");

        var scores = await _scoreRepository.GetAllByUserAsync(userId);
        var totalPoint = scores.Sum(s => s.Point);
        var gameCount = scores.Count;
        var averagePoint = gameCount > 0 ? (double)totalPoint / gameCount : 0;

        var nickname = scores.FirstOrDefault()?.User?.Nickname ?? "Unknown";

        return new MyScoreDto
        {
            Nickname = nickname,
            TotalPoint = totalPoint,
            GameCount = gameCount,
            AveragePoint = Math.Round(averagePoint, 2)
        };
    }
}
