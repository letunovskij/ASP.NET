using AutoFixture.AutoMoq;
using AutoFixture;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Controllers;
using System;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using PromoCodeFactory.WebHost.Models;
using PromoCodeFactory.UnitTests.Helpers;
using PromoCodeFactory.UnitTests.Constants.Partners;
using System.Linq;
using PromoCodeFactory.UnitTests.Constants;

namespace PromoCodeFactory.UnitTests.WebHost.Controllers.Partners;

public class SetPartnerPromoCodeLimitAsyncTests
{
    //TODO: Add Unit Tests
    private readonly Mock<IRepository<Partner>> _partnersRepositoryMock;
    private readonly PartnersController _partnersController;

    public SetPartnerPromoCodeLimitAsyncTests()
    {
        var fixture = new Fixture().Customize(new AutoMoqCustomization());
        _partnersRepositoryMock = fixture.Freeze<Mock<IRepository<Partner>>>();
        _partnersController = fixture.Build<PartnersController>().OmitAutoProperties().Create();
    }

    /// <summary>
    /// Проверяется, что если партнер не найден, то нужно выдать ошибку 404;
    /// </summary>
    [Fact]
    public async Task SetPartnerPromoCodeLimitAsync_PartnerNotFound_Return404() 
    {
        // Arrange
        SetPartnerPromoCodeLimitRequest request = new() { EndDate = new DateTime(2025, 10, 10), Limit = 10 };

        var partnerId = Guid.Parse("def47943-7aaf-44a1-ae21-05aa4948b165");
        Partner partner = null;
        _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partnerId))
            .ReturnsAsync(partner);

        // Act
        var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partnerId, request);

        // Assert
        result.Should().BeAssignableTo<NotFoundResult>();
    }

    /// <summary>
    /// Проверяется, при установке лимита нужно отключить предыдущий лимит;
    /// </summary>
    [Fact]
    public async Task SetPartnerPromoCodeLimitAsync_SetLimit_PreviousLimitTurnOff()
    {
        // Arrange
        SetPartnerPromoCodeLimitRequest request = new() { EndDate = new DateTime(2025, 10, 10), Limit = 10 };

        var partnerId = Guid.Parse("def47943-7aaf-44a1-ae21-05aa4948b165");
        PartnerBuilder builder = new PartnerBuilder();
        Partner partner = builder.CreateDefault().Build();

        _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partnerId))
            .ReturnsAsync(partner);

        // Act
        var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partnerId, request);

        // Assert
        partner.PartnerLimits.First(x => x.Id == DefaultConstants.PartnerPromoCodeLimit.Id).CancelDate.Should().NotBe(null);
    }

    /// <summary>
    /// Проверяется, что если партнер заблокирован, то нужно выдать ошибку 400;
    /// </summary>
    [Fact]
    public async Task SetPartnerPromoCodeLimitAsync_PartnerIsBlocked_Return400()
    {
        // Arrange
        SetPartnerPromoCodeLimitRequest request = new() { EndDate = new DateTime(2025, 10, 10), Limit = 10 };

        var partnerId = Guid.Parse("def47943-7aaf-44a1-ae21-05aa4948b165");
        Partner partner = ElementFactory.CreateDefaultPartner();
        partner.IsActive = false;

        _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partnerId))
            .ReturnsAsync(partner);

        // Act
        var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partnerId, request);

        result.ToString().Should().Be("Microsoft.AspNetCore.Mvc.BadRequestObjectResult");
    }

    /// <summary>
    /// Тест проверяет, что сохранили новый лимит 
    /// </summary>
    [Fact]
    public async Task SetPartnerPromoCodeLimitAsync_SetLimit_PromocodesMustBeSaved()
    {
        // Arrange
        SetPartnerPromoCodeLimitRequest request = new() { EndDate = new DateTime(2025, 10, 10), Limit = 1 };

        var partnerId = Guid.Parse("def47943-7aaf-44a1-ae21-05aa4948b165");
        PartnerBuilder builder = new PartnerBuilder();
        Partner partner = builder.CreateDefault().AddLimit(ElementFactory.CreateDefaultPromocode()).Build();
        partner.PartnerLimits.First().CancelDate = null;

        _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partnerId))
            .ReturnsAsync(partner);

        // Act
        var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partnerId, request);

        // Assert
        partner.PartnerLimits.Count.Should().Be(3);
    }

    /// <summary>
    /// Если партнеру выставляется лимит, то мы должны обнулить количество промокодов
    /// </summary>
    [Fact]
    public async Task SetPartnerPromoCodeLimitAsync_SetLimit_PromocodesMustBe0()
    {
        // Arrange
        SetPartnerPromoCodeLimitRequest request = new() { EndDate = new DateTime(2025, 10, 10), Limit = 10 };

        var partnerId = Guid.Parse("def47943-7aaf-44a1-ae21-05aa4948b165");
        PartnerBuilder builder = new PartnerBuilder();
        Partner partner = builder.CreateDefault().AddLimit(ElementFactory.CreateDefaultPromocode()).Build();
        partner.NumberIssuedPromoCodes = 3;

        _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partnerId))
            .ReturnsAsync(partner);

        // Act
        var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partnerId, request);

        // Assert
        partner.NumberIssuedPromoCodes.Should().Be(0);
    }

    /// <summary>
    /// Проверяется, если лимит закончился, то количество NumberIssuedPromoCodes не обнуляется;
    /// </summary>
    [Fact]
    public async Task SetPartnerPromoCodeLimitAsync_SetLimitButLimitIsClosed_PromocodesMustBeTheSame()
    {
        // Arrange
        SetPartnerPromoCodeLimitRequest request = new() { EndDate = new DateTime(2025, 10, 10), Limit = 10 };

        var partnerId = Guid.Parse("def47943-7aaf-44a1-ae21-05aa4948b165");
        PartnerBuilder builder = new PartnerBuilder();
        Partner partner = builder.CreateDefault().Build();
        partner.PartnerLimits.First().CancelDate = new DateTime(2025, 10, 10);
        partner.NumberIssuedPromoCodes = 3;

        _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partnerId))
            .ReturnsAsync(partner);

        // Act
        var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partnerId, request);

        // Assert
        partner.NumberIssuedPromoCodes.Should().Be(3);
    }

    /// <summary>
    /// Лимит должен быть больше 0;
    /// </summary>
    [Fact]
    public async Task SetPartnerPromoCodeLimitAsync_SetNegativeLimit_Return400()
    {
        // Arrange
        SetPartnerPromoCodeLimitRequest request = new() { EndDate = new DateTime(2025, 10, 10), Limit = -10 };

        var partnerId = Guid.Parse("def47943-7aaf-44a1-ae21-05aa4948b165");
        Partner partner = null;

        _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partnerId))
            .ReturnsAsync(partner);

        // Act
        var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partnerId, request);

        // Assert
        result.Should().BeAssignableTo<NotFoundResult>();
    }
}