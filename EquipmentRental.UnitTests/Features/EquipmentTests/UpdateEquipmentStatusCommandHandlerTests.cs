using EquipmentRental.Application.Common.Results.Errors;
using EquipmentRental.Application.Features.Equipment.Commands.UpdateEquipmentStatus;
using EquipmentRental.UnitTests.Helpers;
using EquipmentEntity = EquipmentRental.Domain.Entites.Equipment;
using EquipmentStatus = EquipmentRental.Domain.Entites.EquipmentStatus;
using LoanEntity = EquipmentRental.Domain.Entites.Loan;

namespace EquipmentRental.UnitTests.Features.EquipmentTests;

public class UpdateEquipmentStatusCommandHandlerTests
{
    private readonly FakeUnitOfWork _uow = new();

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenEquipmentDoesNotExist()
    {
        // Arrange
        var handler = new UpdateEquipmentStatusCommandHandler(_uow);

        // Act
        var result = await handler.Handle(
            new UpdateEquipmentStatusCommand(999, EquipmentStatus.Maintenance),
            CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.NotFound, result.FirstError().Type);
        Assert.Equal(0, _uow.SaveChangesCallCount);
    }

    [Fact]
    public async Task Handle_ShouldChangeStatusAndSave_WhenEquipmentExists()
    {
        // Arrange
        var equipment = _uow.Set<EquipmentEntity>().Seed(TestData.NewEquipment());
        var handler = new UpdateEquipmentStatusCommandHandler(_uow);

        // Act
        var result = await handler.Handle(
            new UpdateEquipmentStatusCommand(equipment.Id, EquipmentStatus.Maintenance),
            CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(EquipmentStatus.Maintenance, equipment.Status);
        Assert.Equal(1, _uow.SaveChangesCallCount);
    }

 
    [Fact(Skip = "Handler doesn't enforce this rule yet - add the active-loan check then remove Skip")]
    public async Task Handle_ShouldReturnConflict_WhenMovingBorrowedEquipmentToMaintenance()
    {
        // Arrange
        var equipment = _uow.Set<EquipmentEntity>().Seed(TestData.NewEquipment());
        _uow.Set<LoanEntity>().Seed(
            TestData.NewLoan(equipment.Id, "user-1", DateTimeOffset.UtcNow.AddDays(3)));
        var handler = new UpdateEquipmentStatusCommandHandler(_uow);

        // Act
        var result = await handler.Handle(
            new UpdateEquipmentStatusCommand(equipment.Id, EquipmentStatus.Maintenance),
            CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.Conflict, result.FirstError().Type);
        Assert.Equal(EquipmentStatus.active, equipment.Status);
    }
}
