
namespace EquipmentRental.Domain.Exceptions;

public abstract class NotFoundException(string message)
    : Exception(message)
{
}

public sealed class EquipmentNotFoundException(int id)
    : NotFoundException($"Equipment with ID {id} was not found.")
{
}

public sealed class LoanNotFoundException(int id)
    : NotFoundException($"Loan with ID {id} was not found.")
{
}
