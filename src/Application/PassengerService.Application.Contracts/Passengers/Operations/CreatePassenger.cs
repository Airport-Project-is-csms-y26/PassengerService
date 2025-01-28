namespace PassengerService.Application.Contracts.Passengers.Operations;

public static class CreatePassenger
{
    public readonly record struct Request(string Name, long Passport, string Email, DateTimeOffset Birthday);

    public abstract record Result
    {
        private Result() { }

        public sealed record Success : Result;

        public sealed record InvalidData : Result;
    }
}