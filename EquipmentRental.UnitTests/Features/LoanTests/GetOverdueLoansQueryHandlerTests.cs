using EquipmentRental.Application.Features.Loan.Queries.GetOverdueLoans;
using EquipmentRental.UnitTests.Helpers;
using EquipmentEntity = EquipmentRental.Domain.Entites.Equipment;
using LoanEntity = EquipmentRental.Domain.Entites.Loan;

namespace EquipmentRental.UnitTests.Features.LoanTests;

public class GetOverdueLoansQueryHandlerTests
{
    private readonly FakeUnitOfWork _uow = new();

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenThereAreNoLoans()
    {
        // Arrange
        var handler = new GetOverdueLoansQueryHandler(_uow);

        // Act
        var result = await handler.Handle(new GetOverdueLoansQuery(), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value);
    }

    [Fact]
    public async Task Handle_ShouldExcludeReturnedAndNotYetDueLoans()
    {
        // Arrange
        var camera = _uow.Set<EquipmentEntity>().Seed(TestData.NewEquipment("Camera"));
        var drone = _uow.Set<EquipmentEntity>().Seed(TestData.NewEquipment("Drone"));
        var laptop = _uow.Set<EquipmentEntity>().Seed(TestData.NewEquipment("Laptop"));
        var now = DateTimeOffset.UtcNow;

        // متأخر فعلًا (لسه مترجعش وتاريخه فات)
        var overdue = _uow.Set<LoanEntity>().Seed(
            TestData.NewLoan(camera.Id, "u1", dueDate: now.AddDays(-2)));

        // اترجع متأخر: مش المفروض يظهر (القاعدة: المتأخر = لم يُرجَع)
        _uow.Set<LoanEntity>().Seed(TestData.NewLoan(
            drone.Id, "u2", dueDate: now.AddDays(-10), returnedAt: now.AddDays(-9)));

        // لسه معاده مجاش
        _uow.Set<LoanEntity>().Seed(
            TestData.NewLoan(laptop.Id, "u3", dueDate: now.AddDays(5)));

        var handler = new GetOverdueLoansQueryHandler(_uow);

        // Act
        var result = await handler.Handle(new GetOverdueLoansQuery(), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        var item = Assert.Single(result.Value);
        Assert.Equal(overdue.Id, item.Id);
        Assert.Equal("Camera", item.EquipmentName);
        Assert.True(item.IsOverdue);
    }

    [Fact]
    public async Task Handle_ShouldOrderByDueDateAscending()
    {
        // Arrange
        var a = _uow.Set<EquipmentEntity>().Seed(TestData.NewEquipment("A"));
        var b = _uow.Set<EquipmentEntity>().Seed(TestData.NewEquipment("B"));
        var now = DateTimeOffset.UtcNow;

        var recentlyOverdue = _uow.Set<LoanEntity>().Seed(
            TestData.NewLoan(a.Id, "u1", dueDate: now.AddDays(-1)));
        var longOverdue = _uow.Set<LoanEntity>().Seed(
            TestData.NewLoan(b.Id, "u2", dueDate: now.AddDays(-9)));

        var handler = new GetOverdueLoansQueryHandler(_uow);

        // Act
        var result = await handler.Handle(new GetOverdueLoansQuery(), CancellationToken.None);

        // Assert: الأقدم استحقاقًا الأول
        Assert.Equal(
            new[] { longOverdue.Id, recentlyOverdue.Id },
            result.Value.Select(x => x.Id).ToArray());
    }
}
