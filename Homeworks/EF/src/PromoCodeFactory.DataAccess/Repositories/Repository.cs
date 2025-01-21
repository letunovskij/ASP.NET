﻿using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

using System.Threading.Tasks;

namespace PromoCodeFactory.DataAccess.Repositories;

public class Repository<T>
        : IRepository<T>
        where T : BaseEntity
{
    private readonly StudentContext _dataContext;

    public Repository(StudentContext dataContext)
    {
        _dataContext = dataContext;
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        var entities = await _dataContext.Set<T>().ToListAsync();

        return entities;
    }

    public async Task<T> GetByIdAsync(Guid id)
    {
        var entity = await _dataContext.Set<T>().FirstOrDefaultAsync(x => x.Id == id);

        return entity;
    }

    public async Task AddAsync(T entity)
    {
        await _dataContext.Set<T>().AddAsync(entity);

        await _dataContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(T entity)
    {
        await _dataContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(T entity)
    {
        _dataContext.Set<T>().Remove(entity);
        await _dataContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<T>> GetByIdsAsync(List<Guid> ids)
    {
        var entities = await _dataContext.Set<T>().Where(x => ids.Contains(x.Id)).ToListAsync();
        return entities;
    }
}
