﻿using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PromoCodeFactory.WebHost.Controllers;

/// <summary>
/// Предпочтения клиентов
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class PreferencesController : ControllerBase
{
    private readonly IRepository<Preference> preferencesRepository;

    public PreferencesController(IRepository<Preference> preferencesRepository)
    {
        this.preferencesRepository = preferencesRepository;
    }

    /// <summary>
    /// Предпочтения клиентов
    /// </summary>
    /// <returns>Список предпочтений всех клиентов</returns>
    [HttpGet]
    public async Task<ActionResult<List<PreferenceResponse>>> GetPreferencesAsync()
    {
        var preferences = await preferencesRepository.GetAllAsync();

        var response = preferences.Select(x => new PreferenceResponse()
        {
            Id = x.Id,
            Name = x.Name
        }).ToList();

        return Ok(response);
    }
}
