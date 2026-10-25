using EquipmentRental.Application.Common.Results.Errors;
using EquipmentRental.Application.Features.Equipment.Queries.GetEquipmentById;
using EquipmentRental.UnitTests.Helpers;
using EquipmentEntity = EquipmentRental.Domain.Entites.Equipment;
using EquipmentStatus = EquipmentRental.Domain.Entites.EquipmentStatus;
using LoanEntity = EquipmentRental.Domain.Entites.Loan;

namespace EquipmentRental.UnitTests.Features.EquipmentTests;

public class GetEquipmentByIdQueryHandlerTests
{
    private readonly FakeUnitOfWork _uow = new();

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenEquipmentDoesNotExist()
    {
        // Arrange
        var handler = new GetEquipmentByIdQueryHandler(_uow);

        // Act
        var result = await handler.Handle(new GetEquipmentByIdQuery(999), CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.NotFound, result.Errors[0].Type);
    }

    [Fact]
    public async Task Handle_ShouldReturnAvailable_WhenActiveAndNotBorrowed()
    {
        // Arrange
        var equipment = _uow.Set<EquipmentEntity>().Seed(TestData.NewEquipment("Camera"));
        var handler = new GetEquipmentByIdQueryHandler(_uow);

        // Act
        var result = await handler.Handle(new GetEquipmentByIdQuery(equipment.Id), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Camera", result.Value.Name);
        Assert.True(result.Value.IsAvaliable);   
    }

    [Fact]
    public async Task Handle_ShouldReturnNotAvailable_WhenEquipmentHasActiveLoan()
    {
        // Arrange
        var equipment = _uow.Set<EquipmentEntity>().Seed(TestData.NewEquipment());
        _uow.Set<LoanEntity>().Seed(
            TestData.NewLoan(equipment.Id, "u1", DateTimeOffset.UtcNow.AddDays(2)));
        var handler = new GetEquipmentByIdQueryHandler(_uow);

        // Act
        var result = await handler.Handle(new GetEquipmentByIdQuery(equipment.Id), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.False(result.Value.IsAvaliable);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotAvailable_WhenEquipmentIsUnderMaintenance()
    {
        // Arrange
        var equipment = _uow.Set<EquipmentEntity>()
            .Seed(TestData.NewEquipment(status: EquipmentStatus.Maintenance));
        var handler = new GetEquipmentByIdQueryHandler(_uow);

        // Act
        var result = await handler.Handle(new GetEquipmentByIdQuery(equipment.Id), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.False(result.Value.IsAvaliable);
        Assert.Equal(EquipmentStatus.Maintenance, result.Value.Status);
    }
}
