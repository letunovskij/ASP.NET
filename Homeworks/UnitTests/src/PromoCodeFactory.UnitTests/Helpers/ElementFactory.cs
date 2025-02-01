using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.UnitTests.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PromoCodeFactory.UnitTests.Helpers;

internal static class ElementFactory
{
    static ElementFactory() { }

    private static readonly JsonSerializerOptions jsonOptions =
        new(JsonSerializerDefaults.Web) { PropertyNameCaseInsensitive = true };

    internal static Partner? CreatePartner(Partner partner)
        => Clone(partner);

    internal static Partner? CreateDefaultPartner()
        => Clone(DefaultConstants.Partner);

    internal static T? Clone<T>(T? source)
    {
        var serialized = JsonSerializer.Serialize(source, jsonOptions);
        return JsonSerializer.Deserialize<T>(serialized, jsonOptions);
    }

    internal static PartnerPromoCodeLimit CreateDefaultPromocode()
        => Clone(DefaultConstants.PartnerPromoCodeLimit);
}
