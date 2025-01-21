using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.DataAccess;
using PromoCodeFactory.WebHost.Models;

namespace PromoCodeFactory.WebHost.Controllers
{
    /// <summary>
    /// Промокоды
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PromocodesController
        : ControllerBase
    {
        private readonly StudentContext dataContext;
        private readonly IRepository<PromoCode> promoCodesRepository;

        public PromocodesController(IRepository<PromoCode> promoCodesRepository, StudentContext dataContext)
        {
            this.promoCodesRepository = promoCodesRepository;
            this.dataContext = dataContext;
        }

        /// <summary>
        /// Получить все промокоды
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<PromoCodeShortResponse>>> GetPromocodesAsync()
        {
            var preferences = await promoCodesRepository.GetAllAsync();

            var response = preferences.Select(x => new PromoCodeShortResponse()
            {
                Id = x.Id,
                Code = x.Code,
                BeginDate = x.BeginDate.ToString("yyyy-MM-dd"),
                EndDate = x.EndDate.ToString("yyyy-MM-dd"),
                PartnerName = x.PartnerName,
                ServiceInfo = x.ServiceInfo
            }).ToList();

            return Ok(response);
        }

        /// <summary>
        /// Создать промокод и выдать его клиентам с указанным предпочтением
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> GivePromoCodesToCustomersWithPreferenceAsync(GivePromoCodeRequest request)
        {
            var promoCode = new PromoCode()
            {
                Code = request.PromoCode,
                BeginDate = DateTime.Now,
                EndDate = DateTime.Now.AddMonths(1),
                PartnerName = request.PartnerName,
                ServiceInfo = request.ServiceInfo
            };

            await promoCodesRepository.AddAsync(promoCode);

            var customers = await dataContext.Customers
                                             .Include(x => x.CustomerPreferences)
                                                .ThenInclude(x => x.Preference)
                                             .Where(x => x.CustomerPreferences
                                                          .Select(x => x.Preference.Name)
                                                          .Contains(request.Preference))
                                             .AsSplitQuery()
                                             .ToListAsync();

            foreach (var customer in customers)
                customer.PromoCodes.Add(promoCode);

            await dataContext.SaveChangesAsync();

            return NoContent();
        }
    }
}