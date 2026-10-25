using EquipmentRental.Application.Common.Results.Errors;
using EquipmentRental.Application.Features.Loan.Queries.GetLoans;
using EquipmentRental.UnitTests.Helpers;
using EquipmentEntity = EquipmentRental.Domain.Entites.Equipment;
using LoanEntity = EquipmentRental.Domain.Entites.Loan;

namespace EquipmentRental.UnitTests.Features.LoanTests;

public class GetLoansQueryHandlerTests
{
    private readonly FakeUnitOfWork _uow = new();

    private GetLoansQueryHandler CreateHandler(string? userId, bool isAdmin = false)
        => new(_uow, TestData.CurrentUser(userId, isAdmin).Object);

    private void SeedThreeLoans()
    {
        var e1 = _uow.Set<EquipmentEntity>().Seed(TestData.NewEquipment("Camera"));
        var e2 = _uow.Set<EquipmentEntity>().Seed(TestData.NewEquipment("Drone"));
        var e3 = _uow.Set<EquipmentEntity>().Seed(TestData.NewEquipment("Laptop"));
        var now = DateTimeOffset.UtcNow;

        _uow.Set<LoanEntity>().Seed(TestData.NewLoan(
            e1.Id, "user-1", dueDate: now.AddDays(3), borrowedAt: now.AddDays(-1)));

        _uow.Set<LoanEntity>().Seed(TestData.NewLoan(
            e2.Id, "user-1", dueDate: now.AddDays(-5),
            returnedAt: now.AddDays(-6), borrowedAt: now.AddDays(-10)));

        _uow.Set<LoanEntity>().Seed(TestData.NewLoan(
            e3.Id, "user-2", dueDate: now.AddDays(2), borrowedAt: now.AddDays(-2)));
    }

    [Fact]
    public async Task Handle_ShouldReturnUnauthorized_WhenUserIsNotAuthenticated()
    {
        // Arrange
        var handler = CreateHandler(userId: null);

        // Act
        var result = await handler.Handle(new GetLoansQuery(), CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.Unauthorized, result.Errors[0].Type);
    }

    [Fact]
    public async Task Handle_ShouldReturnOnlyOwnLoans_WhenUserIsMember()
    {
        // Arrange
        SeedThreeLoans();
        var handler = CreateHandler("user-1", isAdmin: false);

        // Act
        var result = await handler.Handle(new GetLoansQuery(), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Count);
        Assert.All(result.Value, l => Assert.Equal("user-1", l.UserId));
    }

    [Fact]
    public async Task Handle_ShouldReturnAllLoans_WhenUserIsAdmin()
    {
        // Arrange
        SeedThreeLoans();
        var handler = CreateHandler("admin-1", isAdmin: true);

        // Act
        var result = await handler.Handle(new GetLoansQuery(), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(3, result.Value.Count);
    }

    [Fact]
    public async Task Handle_ShouldOrderByBorrowedAtDescending()
    {
        // Arrange
        SeedThreeLoans();
        var handler = CreateHandler("admin-1", isAdmin: true);

        // Act
        var result = await handler.Handle(new GetLoansQuery(), CancellationToken.None);

        // Assert
        var dates = result.Value.Select(l => l.BorrowedAt).ToList();
        Assert.Equal(dates.OrderByDescending(d => d).ToList(), dates);
    }

    [Fact]
    public async Task Handle_ShouldNotMarkReturnedLoanAsOverdue()
    {
        // Arrange
        SeedThreeLoans();
        var handler = CreateHandler("user-1");

        // Act
        var result = await handler.Handle(new GetLoansQuery(), CancellationToken.None);

        // Assert
        var returnedLoan = result.Value.Single(l => l.EquipmentName == "Drone");
        Assert.NotNull(returnedLoan.ReturnedAt);
        Assert.False(returnedLoan.IsOverdue);
    }

    [Fact]
    public async Task Handle_ShouldMarkUnreturnedPastDueLoanAsOverdue()
    {
        // Arrange
        var equipment = _uow.Set<EquipmentEntity>().Seed(TestData.NewEquipment("Camera"));
        _uow.Set<LoanEntity>().Seed(TestData.NewLoan(
            equipment.Id, "user-1", dueDate: DateTimeOffset.UtcNow.AddDays(-1)));
        var handler = CreateHandler("user-1");

        // Act
        var result = await handler.Handle(new GetLoansQuery(), CancellationToken.None);

        // Assert
        var loan = Assert.Single(result.Value);
        Assert.True(loan.IsOverdue);
        Assert.Equal("Camera", loan.EquipmentName);
    }
}
