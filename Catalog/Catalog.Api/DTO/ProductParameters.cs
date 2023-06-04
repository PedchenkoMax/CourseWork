namespace Catalog.Api.DTO;

public record ProductParameters(
    int PageNumber = 1,
    int PageSize = 10,
    string OrderBy = "Name",
    bool IsAscending = true);