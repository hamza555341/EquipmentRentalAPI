using EquipmentRental.Application.Common.Results.Errors;
using EquipmentRental.Application.Features.Loan.Commands.ReturnLoan;
using EquipmentRental.UnitTests.Helpers;
using EquipmentEntity = EquipmentRental.Domain.Entites.Equipment;
using LoanEntity = EquipmentRental.Domain.Entites.Loan;

namespace EquipmentRental.UnitTests.Features.LoanTests;

public class ReturnLoanCommandHandlerTests
{
    private readonly FakeUnitOfWork _uow = new();

    private ReturnLoanCommandHandler CreateHandler(string? userId, bool isAdmin = false)
        => new(_uow, TestData.CurrentUser(userId, isAdmin).Object);

    private LoanEntity SeedActiveLoan(string ownerId)
    {
        var equipment = _uow.Set<EquipmentEntity>().Seed(TestData.NewEquipment());
        return _uow.Set<LoanEntity>().Seed(
            TestData.NewLoan(equipment.Id, ownerId, DateTimeOffset.UtcNow.AddDays(3)));
    }

    [Fact]
    public async Task Handle_ShouldReturnUnauthorized_WhenUserIsNotAuthenticated()
    {
        // Arrange
        var handler = CreateHandler(userId: null);

        // Act
        var result = await handler.Handle(new ReturnLoanCommand(1), CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.Unauthorized, result.FirstError().Type);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenLoanDoesNotExist()
    {
        // Arrange
        var handler = CreateHandler("user-1");

        // Act
        var result = await handler.Handle(new ReturnLoanCommand(999), CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.NotFound, result.FirstError().Type);
        Assert.Equal(0, _uow.SaveChangesCallCount);
    }

    [Fact]
    public async Task Handle_ShouldReturnForbidden_WhenMemberReturnsAnotherUsersLoan()
    {
        // Arrange
        var loan = SeedActiveLoan(ownerId: "owner");
        var handler = CreateHandler("intruder", isAdmin: false);

        // Act
        var result = await handler.Handle(new ReturnLoanCommand(loan.Id), CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.Forbidden, result.FirstError().Type);
        Assert.Null(loan.ReturnedAt);                 // القرض لسه زي ما هو
        Assert.Equal(0, _uow.SaveChangesCallCount);
    }

    [Fact]
    public async Task Handle_ShouldReturnConflict_WhenLoanIsAlreadyReturned()
    {
        // Arrange
        var equipment = _uow.Set<EquipmentEntity>().Seed(TestData.NewEquipment());
        var loan = _uow.Set<LoanEntity>().Seed(TestData.NewLoan(
            equipment.Id, "user-1",
            dueDate: DateTimeOffset.UtcNow.AddDays(1),
            returnedAt: DateTimeOffset.UtcNow.AddHours(-2)));
        var handler = CreateHandler("user-1");

        // Act
        var result = await handler.Handle(new ReturnLoanCommand(loan.Id), CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.Conflict, result.FirstError().Type);
        Assert.Equal("Loan.AlreadyReturned", result.FirstError().Code);
        Assert.Equal(0, _uow.SaveChangesCallCount);
    }

    [Fact]
    public async Task Handle_ShouldSetReturnedAt_WhenOwnerReturnsOwnLoan()
    {
        // Arrange
        var loan = SeedActiveLoan(ownerId: "user-1");
        var handler = CreateHandler("user-1");
        var before = DateTimeOffset.UtcNow;

        // Act
        var result = await handler.Handle(new ReturnLoanCommand(loan.Id), CancellationToken.None);

        // Assert
        var after = DateTimeOffset.UtcNow;

        Assert.True(result.IsSuccess);
        Assert.NotNull(loan.ReturnedAt);
        Assert.InRange(loan.ReturnedAt!.Value, before, after);
        Assert.Equal(1, _uow.SaveChangesCallCount);
    }

    [Fact]
    public async Task Handle_ShouldAllowAdminToReturnAnotherUsersLoan()
    {
        // Arrange
        var loan = SeedActiveLoan(ownerId: "member-1");
        var handler = CreateHandler("admin-1", isAdmin: true);

        // Act
        var result = await handler.Handle(new ReturnLoanCommand(loan.Id), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(loan.ReturnedAt);
        Assert.Equal(1, _uow.SaveChangesCallCount);
    }
}
