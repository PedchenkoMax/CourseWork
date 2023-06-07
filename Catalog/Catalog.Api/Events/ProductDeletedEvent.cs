namespace Catalog.Api.Events;

public interface IProductDeletedEvent
{
    Guid ProductId { get; }
}

public record ProductDeletedEvent(Guid ProductId);
