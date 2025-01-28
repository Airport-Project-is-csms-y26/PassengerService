namespace PassengerService.Application.Contracts.Passengers.Operations;

public static class BanPassenger
{
    public readonly record struct Request(long Id);

    public abstract record Result
    {
        private Result() { }

        public sealed record Success : Result;

        public sealed record PassengerNotFound : Result;
    }
}