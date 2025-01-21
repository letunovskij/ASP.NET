using PromoCodeFactory.Core.Abstractions.Repositories;

namespace PromoCodeFactory.DataAccess.Data;

public sealed class Initialize : IInitialize
{
    private readonly StudentContext dataContext;

    public Initialize(StudentContext dataContext)
    {
        this.dataContext = dataContext;
    }

    public void InitializeDb()
    {
        dataContext.Database.EnsureDeleted();
        dataContext.Database.EnsureCreated();

        dataContext.AddRange(FakeDataFactory.Customers);
        dataContext.SaveChanges();

        dataContext.AddRange(FakeDataFactory.Employees);
        dataContext.SaveChanges();

        dataContext.AddRange(FakeDataFactory.Preferences);
        dataContext.SaveChanges();
    }
}
