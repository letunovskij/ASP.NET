using Mapster;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Dtos.Employee;
using PromoCodeFactory.Core.Views;

namespace PromoCodeFactory.WebHost.Config;

internal sealed class EmployeeMappingProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Employee, EmployeeView>();

        config.NewConfig<EmployeeBaseDto, Employee>()
              .Ignore(x => x.Roles);

        config.NewConfig<Role, RoleView>();
    }
}
