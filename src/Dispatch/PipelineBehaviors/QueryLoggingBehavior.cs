using Microsoft.Extensions.Logging;

namespace Vesia.Dispatch;

internal sealed class QueryLoggingBehavior<TQuery, TResult>(ILogger<QueryLoggingBehavior<TQuery, TResult>> logger)
    : IQueryPipelineBehavior<TQuery, TResult>
    where TQuery : IQuery<TResult>
{
    public async Task<TResult> Handle(TQuery query, Func<Task<TResult>> next, CancellationToken cancellationToken = default)
    {
        try
        {
            logger.LogInformation("Handling query {Query}", typeof(TQuery).Name);
            var result = await next();
            logger.LogInformation("Handled query {Query}", typeof(TQuery).Name);
            return result;
        }
        catch(Exception ex)
        {
            logger.LogError(ex, "Error handling query {Query}", typeof(TQuery).Name);
            throw;
        }
    }
}
