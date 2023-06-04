namespace Catalog.Api.DTO;

public record PaginatedList<T>(
    List<T> Items,
    int CurrentPage,
    int TotalPages,
    int PageSize,
    int TotalCount);