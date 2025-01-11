using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain;

namespace PromoCodeFactory.DataAccess.Repositories;

/// <summary>
/// Мок БД
/// </summary>
/// <remarks>
/// Аналог БД, не является ни сервисом; ни репозиторием (также использование репозитория в архитектуре ASP.NET спорно)
/// </remarks>
public class InMemoryRepository<T>: IRepository<T> where T: BaseEntity
{
    protected IList<T> Data { get; set; }

    public InMemoryRepository(IList<T> data)
    {
        Data = data;
    }

    public Task<IReadOnlyList<T>> GetAllAsync()
    {
        return Task.FromResult(Data as IReadOnlyList<T>);
    }

    public Task<T> GetByIdAsync(Guid id)
    {
        return Task.FromResult(Data.FirstOrDefault(x => x.Id == id));
    }

    public Task<T> Add(T entity)
    {
        entity.Id = Guid.NewGuid();
        Data.Add(entity);

        return Task.FromResult(entity);
    }

    public async Task<T> Update(T entity)
    {
        await Delete(entity.Id);
        Data.Add(entity);

        return entity;
    }

    public Task Delete(Guid id)
    {
        var removing = Data.FirstOrDefault(x => x.Id == id) ?? throw new Exception($"Не найдена сущность {id}");
        Data.Remove(removing);

        return Task.FromResult(0);
    }
}