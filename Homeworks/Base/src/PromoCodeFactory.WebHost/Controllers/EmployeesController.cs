using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Dtos.Employee;
using PromoCodeFactory.Core.Views;
using PromoCodeFactory.WebHost.Models;

namespace PromoCodeFactory.WebHost.Controllers;

/// <summary>
/// Сотрудники
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly IRepository<Employee> employeeRepository;

    public EmployeesController(IRepository<Employee> employeeRepository)
    {
        this.employeeRepository = employeeRepository;
    }

    /// <summary>
    /// Получить данные всех сотрудников
    /// </summary>
    [HttpGet]
    public async Task<IReadOnlyList<EmployeeShortResponse>> GetEmployeesAsync()
    {
        var employees = await employeeRepository.GetAllAsync();

        var employeesModelList = employees.Select(x =>
            new EmployeeShortResponse()
            {
                Id = x.Id,
                Email = x.Email,
                FullName = x.FullName,
            }).ToList();

        return employeesModelList;
    }

    /// <summary>
    /// Получить данные сотрудника по Id
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<EmployeeResponse>> GetEmployeeByIdAsync(Guid id)
    {
        var employee = await employeeRepository.GetByIdAsync(id);

        if (employee == null)
            return NotFound();

        var employeeModel = new EmployeeResponse()
        {
            Id = employee.Id,
            Email = employee.Email,
            Roles = employee.Roles.Select(x => new RoleItemResponse()
            {
                Name = x.Name,
                Description = x.Description
            }).ToList(),
            FullName = employee.FullName,
            AppliedPromocodesCount = employee.AppliedPromocodesCount
        };

        return employeeModel;
    }

    /// <summary>
    /// Создать сотрудника
    /// </summary>
    [HttpPost]
    public async Task<EmployeeView> CreateEmployee(EmployeeCreateDto dto)
    {
        var employee = dto.Adapt<Employee>();

        return (await employeeRepository.Add(employee)).Adapt<EmployeeView>();
    }

    /// <summary>
    /// Обновить сотрудника
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<EmployeeView> UpdateEmployee(Guid id, EmployeeUpdateDto dto)
    {
        var employee = await employeeRepository.GetByIdAsync(id) ?? throw new Exception($"Не найден пользователь {id}");

        dto.Adapt(employee);

        return (await employeeRepository.Update(employee)).Adapt<EmployeeView>();
    }

    /// <summary>
    /// Удалить сотрудника
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task DeleteEmployee(Guid id)
        => await employeeRepository.Delete(id);
}