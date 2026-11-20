namespace learingASP.Common
{
    public class Result
    {
        private readonly Error? _error;

        protected Result(bool isSuccess, Error? error)
        {
            if (isSuccess && error is not null)
                throw new InvalidOperationException("A successful result cannot have an error.");
            if (!isSuccess && error is null)
                throw new InvalidOperationException("A failed result must have an error.");

            IsSuccess = isSuccess;
            _error = error;
        }

        public bool IsSuccess { get; }

        public Error Error => _error
            ?? throw new InvalidOperationException("A successful result has no error.");

        public static Result Success() => new(true, null);
        public static Result Failure(Error error) => new(false, error);

        public static implicit operator Result(Error error) => Failure(error);
    }

    public class Result<T> : Result
    {
        private readonly T? _value;

        private Result(T? value, bool isSuccess, Error? error) : base(isSuccess, error)
            => _value = value;

        public T Value => IsSuccess
            ? _value!
            : throw new InvalidOperationException("Cannot read the value of a failed result.");

        public static Result<T> Success(T value) => new(value, true, null);
        public static new Result<T> Failure(Error error) => new(default, false, error);

        public static implicit operator Result<T>(T value) => Success(value);
        public static implicit operator Result<T>(Error error) => Failure(error);
    }
}
