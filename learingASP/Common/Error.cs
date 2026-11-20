namespace learingASP.Common;

public enum ErrorKind
{
    Validation,   // bad input        → 400
    NotFound,     // missing record   → 404
    Forbidden,    // not allowed      → 403
    Conflict      // overlap, balance → 409
}

public sealed record Error(ErrorKind Kind, string Message)
{
    public static Error Validation(string message) => new(ErrorKind.Validation, message);
    public static Error NotFound(string message) => new(ErrorKind.NotFound, message);
    public static Error Forbidden(string message) => new(ErrorKind.Forbidden, message);
    public static Error Conflict(string message) => new(ErrorKind.Conflict, message);
}
