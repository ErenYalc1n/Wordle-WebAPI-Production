using MediatR;
using Wordle.Application.DailyWords.DTOs;

namespace Wordle.Application.DailyWords.Queries.Search;

public record SearchDailyWordQuery(string SearchInput) : IRequest<SearchDailyWordDto?>;
