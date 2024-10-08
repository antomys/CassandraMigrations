namespace Cassandra.Migrations;

public sealed class MigrationsRetryPolicy(int readAttempts, int writeAttempts, int unavailableAttempts) : IRetryPolicy
{
    public RetryDecision OnReadTimeout(
        IStatement query,
        ConsistencyLevel cl,
        int requiredResponses,
        int receivedResponses,
        bool dataRetrieved,
        int nbRetry)
    {
        if (dataRetrieved)
        {
            return RetryDecision.Ignore();
        }

        return nbRetry < readAttempts
            ? RetryDecision.Retry(cl)
            : RetryDecision.Rethrow();
    }

    public RetryDecision OnUnavailable(
        IStatement query,
        ConsistencyLevel cl,
        int requiredReplica, 
        int aliveReplica,
        int nbRetry) =>
        nbRetry < unavailableAttempts
            ? RetryDecision.Retry(ConsistencyLevel.One)
            : RetryDecision.Rethrow();

    public RetryDecision OnWriteTimeout(
        IStatement query,
        ConsistencyLevel cl,
        string writeType,
        int requiredAcks,
        int receivedAcks,
        int nbRetry) =>
        nbRetry < writeAttempts
            ? RetryDecision.Retry(cl)
            : RetryDecision.Rethrow();
}