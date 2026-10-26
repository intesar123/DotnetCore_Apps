namespace CircuitBreakerTestApp.Infrastructure.Resilience;

public sealed class ResilienceSettings
{
    public const string SectionName = "Resilience";

    public OrdersSettings Orders { get; init;} = new();
    public DatabaseSettings Database {get; init;} = new();

    public class OrdersSettings
    {
         public int TotalTimeoutSeconds { get; init; } = 10;
        public int AttemptTimeoutSeconds { get; init; } = 2;
        public int MaxRetryAttempts { get; init; } = 3;
        public double FailureRatio { get; init; } = 0.5;
        public int MinimumThroughput { get; init; } = 20;
        public int SamplingDurationSeconds { get; init; } = 30;
        public int BreakDurationSeconds { get; init; } = 15;
    }

    public class DatabaseSettings
    {
        public double FailureRatio { get; init; } = 0.5;
        public int MinimumThroughput { get; init; } = 10;
        public int SamplingDurationSeconds { get; init; } = 30;
        public int BreakDurationSeconds { get; init; } = 20;
    }
}