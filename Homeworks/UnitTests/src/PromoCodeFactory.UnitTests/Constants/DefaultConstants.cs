using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PromoCodeFactory.UnitTests.Constants;

public static class DefaultConstants
{

    public static readonly PartnerPromoCodeLimit PartnerPromoCodeLimit = new()
    {
        Id = Guid.Parse("e00633a5-978a-420e-a7d6-3e1dab116393"),
        CreateDate = new DateTime(2020, 07, 9),
        EndDate = new DateTime(2020, 10, 9),
        Limit = 100,
        CancelDate = null,
    };

    public static readonly Partner Partner = new Partner()
    {
        Id = Guid.Parse("7d994823-8226-4273-b063-1a95f3cc1df8"),
        Name = "Суперигрушки",
        IsActive = true,
        PartnerLimits = new List<PartnerPromoCodeLimit>()
        {
            DefaultConstants.PartnerPromoCodeLimit
        }
    };


}
