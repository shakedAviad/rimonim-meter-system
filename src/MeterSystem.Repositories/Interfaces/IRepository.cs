namespace MeterSystem.Repositories.Interfaces;

public interface IRepository
{
    Task SaveAsync<T>(T entity, CancellationToken cancellationToken = default);
}