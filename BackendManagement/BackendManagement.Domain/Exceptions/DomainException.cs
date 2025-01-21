namespace BackendManagement.Domain.Exceptions;

/// <summary>
/// 領域例外基礎類別
/// </summary>
public abstract class DomainException : Exception
{
    protected DomainException(string message) : base(message)
    {
    }

    protected DomainException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
} 