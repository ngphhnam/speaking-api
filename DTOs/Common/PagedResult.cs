namespace SpeakingPractice.Api.DTOs.Common;

public record PagedResult<T>(IReadOnlyCollection<T> Items, int Page, int PageSize, int TotalCount);

