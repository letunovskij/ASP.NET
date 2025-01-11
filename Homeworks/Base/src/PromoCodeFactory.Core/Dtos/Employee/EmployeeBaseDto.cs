using System.Collections.Generic;

namespace PromoCodeFactory.Core.Dtos.Employee;

public class EmployeeBaseDto
{
    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Email { get; set; }

    public int AppliedPromocodesCount { get; set; }
}
