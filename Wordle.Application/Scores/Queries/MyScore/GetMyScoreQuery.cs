using MediatR;
using Wordle.Application.DTOs;
using Wordle.Application.Scores.DTOs;

namespace Wordle.Application.Scores.Queries.Leaderboard;

public record GetMyScoreQuery() : IRequest<MyScoreDto>;
