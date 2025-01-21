using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PromoCodeFactory.WebHost.Controllers
{
    /// <summary>
    /// Клиенты
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CustomersController
        : ControllerBase
    {
        private readonly IRepository<Customer> customerRepository;
        private readonly IRepository<Preference> preferenceRepository;

        public CustomersController(IRepository<Customer> customerRepository,
            IRepository<Preference> preferenceRepository)
        {
            this.customerRepository = customerRepository;
            this.preferenceRepository = preferenceRepository;
        }

        /// <summary>
        /// Получить клиентов
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<CustomerShortResponse>>> GetCustomersAsync()
        {
            var customers = await customerRepository.GetAllAsync();

            var response = customers.Select(x => new CustomerShortResponse()
            {
                Id = x.Id,
                Email = x.Email,
                FirstName = x.FirstName,
                LastName = x.LastName
            }).ToList();

            return Ok(response);
        }

        /// <summary>
        /// Получить клиента
        /// </summary>
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<CustomerResponse>> GetCustomerAsync(Guid id)
        {
            var customer = await customerRepository.GetByIdAsync(id);

            if (customer == null)
                return NotFound();

            var response = new CustomerResponse()
            {
                Id = customer.Id,
                Email = customer.Email,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Preferences = customer.CustomerPreferences.Select(x => new PreferenceResponse()
                {
                    Id = x.PreferenceId,
                    Name = x.Preference.Name
                }).ToList()
            };

            return Ok(response);
        }

        /// <summary>
        /// Создать клиента
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<CustomerResponse>> CreateCustomerAsync(CreateOrEditCustomerRequest request)
        {
            var preferences = await preferenceRepository.GetByIdsAsync(request.PreferenceIds);

            var customer = new Customer()
            {
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
            };
            customer.CustomerPreferences = preferences.Select(x => new CustomerPreference()
            {
                Customer = customer,
                Preference = x
            }).ToList();

            await customerRepository.AddAsync(customer);

            return Ok(new CustomerResponse() { 
                Id = customer.Id, Email = customer.Email, FirstName = customer.FirstName, LastName = customer.LastName,
                Preferences = customer.CustomerPreferences?.Select(x => new PreferenceResponse()
                {
                    Id = x.PreferenceId,
                    Name = x.Preference.Name
                }).ToList(),
                PromoCodes = customer.PromoCodes?.Select(x => new PromoCodeShortResponse()
                {
                    Id = x.Id,
                    Code = x.Code,
                    ServiceInfo = x.ServiceInfo,
                    PartnerName = x.PartnerName,
                }).ToList(),
            });
        }

        /// <summary>
        /// Обновить клиента
        /// </summary>
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> EditCustomersAsync(Guid id, CreateOrEditCustomerRequest request)
        {
            var customer = await customerRepository.GetByIdAsync(id);

            if (customer == null)
                return NotFound();

            var preferences = await preferenceRepository.GetByIdsAsync(request.PreferenceIds);

            customer.Email = request.Email;
            customer.FirstName = request.FirstName;
            customer.LastName = request.LastName;
            customer.CustomerPreferences.Clear();
            customer.CustomerPreferences = preferences.Select(x => new CustomerPreference()
            {
                Customer = customer,
                Preference = x
            }).ToList();

            await customerRepository.UpdateAsync(customer);

            return NoContent();
        }

        /// <summary>
        /// Удалить клиента
        /// </summary>
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteCustomerAsync(Guid id)
        {
            var customer = await customerRepository.GetByIdAsync(id);

            if (customer == null)
                return NotFound();

            await customerRepository.DeleteAsync(customer);

            return NoContent();
        }
    }
}