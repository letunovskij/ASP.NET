using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.UnitTests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PromoCodeFactory.UnitTests.Constants.Partners;

public class PartnerBuilder
{
    public Partner? Partner { get; set; }

    public PartnerBuilder CreateDefault()
    {
        Partner = ElementFactory.CreateDefaultPartner();
        return this;
    }

    public PartnerBuilder Create(Partner partner)
    {
        Partner = ElementFactory.CreatePartner(partner);
        return this;
    }

    public PartnerBuilder AddLimit(PartnerPromoCodeLimit limit)
    {
        Partner.PartnerLimits ??= [];
        Partner.PartnerLimits.Add(limit);

        return this;
    }

    public Partner Build()
    {
        return Partner;
    }
}
