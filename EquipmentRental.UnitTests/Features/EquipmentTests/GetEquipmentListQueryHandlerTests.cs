using EquipmentRental.Application.Features.Equipment.Queries.GetEquipmentList;
using EquipmentRental.UnitTests.Helpers;
using EquipmentEntity = EquipmentRental.Domain.Entites.Equipment;
using EquipmentStatus = EquipmentRental.Domain.Entites.EquipmentStatus;
using LoanEntity = EquipmentRental.Domain.Entites.Loan;

namespace EquipmentRental.UnitTests.Features.EquipmentTests;

public class GetEquipmentListQueryHandlerTests
{
    private readonly FakeUnitOfWork _uow = new();

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenThereIsNoEquipment()
    {
        // Arrange
        var handler = new GetEquipmentListQueryHandler(_uow);

        // Act
        var result = await handler.Handle(new GetEquipmentListQuery(), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value);
    }

    [Fact]
    public async Task Handle_ShouldComputeAvailabilityCorrectly()
    {
        // Arrange
        var free = _uow.Set<EquipmentEntity>().Seed(TestData.NewEquipment("Free"));
        var borrowed = _uow.Set<EquipmentEntity>().Seed(TestData.NewEquipment("Borrowed"));
        var inMaintenance = _uow.Set<EquipmentEntity>()
            .Seed(TestData.NewEquipment("Broken", EquipmentStatus.Maintenance));
        var returnedBefore = _uow.Set<EquipmentEntity>().Seed(TestData.NewEquipment("ReturnedBefore"));

        // Borrowed: عليه Loan نشط
        _uow.Set<LoanEntity>().Seed(
            TestData.NewLoan(borrowed.Id, "u1", DateTimeOffset.UtcNow.AddDays(2)));

        // ReturnedBefore: كان مستعار واترجع، يعني متاح تاني
        _uow.Set<LoanEntity>().Seed(TestData.NewLoan(
            returnedBefore.Id, "u2",
            dueDate: DateTimeOffset.UtcNow.AddDays(-2),
            returnedAt: DateTimeOffset.UtcNow.AddDays(-3)));

        var handler = new GetEquipmentListQueryHandler(_uow);

        // Act
        var result = await handler.Handle(new GetEquipmentListQuery(), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(4, result.Value.Count);   // بيرجّع كل المعدات حتى الـ Maintenance

        // الخاصية اسمها IsAvaliable عندك (فيها typo). لو صلّحتها غيّر الاسم هنا.
        Assert.True(result.Value.Single(x => x.Id == free.Id).IsAvaliable);
        Assert.False(result.Value.Single(x => x.Id == borrowed.Id).IsAvaliable);
        Assert.False(result.Value.Single(x => x.Id == inMaintenance.Id).IsAvaliable);
        Assert.True(result.Value.Single(x => x.Id == returnedBefore.Id).IsAvaliable);
    }
}
