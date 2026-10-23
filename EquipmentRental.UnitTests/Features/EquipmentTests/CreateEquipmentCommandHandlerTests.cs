using EquipmentRental.Application.Features.Equipment.Commands.CreateEquipment;
using EquipmentRental.UnitTests.Helpers;
using EquipmentEntity = EquipmentRental.Domain.Entites.Equipment;
using EquipmentStatus = EquipmentRental.Domain.Entites.EquipmentStatus;

namespace EquipmentRental.UnitTests.Features.EquipmentTests;

public class CreateEquipmentCommandHandlerTests
{
    private readonly FakeUnitOfWork _uow = new();

    [Fact]
    public async Task Handle_ShouldCreateActiveEquipmentAndSave_WhenCommandIsValid()
    {
        // Arrange
        var handler = new CreateEquipmentCommandHandler(_uow);
        var command = new CreateEquipmentCommand("Camera", "4K camera");
        var before = DateTimeOffset.UtcNow;

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        var after = DateTimeOffset.UtcNow;

        Assert.True(result.IsSuccess);

        var equipment = Assert.Single(_uow.Set<EquipmentEntity>().Items);
        Assert.Equal("Camera", equipment.Name);
        Assert.Equal("4K camera", equipment.Description);
        Assert.Equal(EquipmentStatus.active, equipment.Status);   // الحالة الافتراضية Active
        Assert.InRange(equipment.CreatedAt, before, after);       // CreatedAt بوقت UTC
        Assert.Equal(1, _uow.SaveChangesCallCount);
    }
}
