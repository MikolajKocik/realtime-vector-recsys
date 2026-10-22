using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RecSys.Application.Abstractions;
using RecSys.Application.Features.Telemetry;
using RecSys.Domain.Entities;
using RecSys.Domain.Rules;
using RecSys.Infrastructure.Persistence;

namespace RecSys.Infrastructure.BackgroundJobs;

public sealed class TasteProfileWorker(
    ILogger<TasteProfileWorker> logger,
    IServiceScopeFactory scopeFactory,
    IInteractionEventQueue queue
) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("TasteProfileWorker started listening for interactions.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                UserInteractedEvent evt = await queue.DequeueAsync(stoppingToken);
                await ProcessInteractionAsync(evt, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing user interaction event");
            }
        }
    }

    private async Task ProcessInteractionAsync(UserInteractedEvent evt, CancellationToken ct)
    {
        using var scope = scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var vectorService = scope.ServiceProvider.GetRequiredService<IVectorMathService>();
        var cacheService = scope.ServiceProvider.GetRequiredService<IRecommendationCacheService>();

        CatalogItem? catalogItem = await dbContext.CatalogItems.FirstOrDefaultAsync(i => i.Id == evt.ItemId, ct);
        if (catalogItem is null)
        {
            logger.LogWarning("Catalog item {ItemId} not found for user interaction", evt.ItemId);
            return;
        }

        UserTasteProfile? profile = await dbContext.UserProfiles.FirstOrDefaultAsync(p => p.UserId == evt.UserId, ct);

        await using var transaction = await dbContext.Database.BeginTransactionAsync(ct);
        try
        {
            if (profile is null)
            {
                profile = new UserTasteProfile(evt.UserId, catalogItem.Embedding.ToArray());
                dbContext.UserProfiles.Add(profile);
            }
            else
            {
                float learningRate = InteractionWeightPolicy.GetLearningRate(evt.InteractionType);
                var tasteShiftResult = vectorService.CalculateTasteShift(
                    profile.TasteVector.ToArray(),
                    catalogItem.Embedding.ToArray(),
                    learningRate
                );

                if (tasteShiftResult.IsSuccess && tasteShiftResult.Value is not null)
                {
                    profile.UpdateTaste(tasteShiftResult.Value);
                }
            }

            await dbContext.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(ct);
            logger.LogError(ex, "Failed to update taste profile for user {UserId}", evt.UserId);
            return;
        }

        await cacheService.InvalidateUserCacheAsync(evt.UserId, ct);

        int boost = InteractionWeightPolicy.GetPopularityBoost(evt.InteractionType);
        if (boost > 0)
        {
            await cacheService.IncrementItemPopularityAsync(catalogItem.Id, boost, ct);
        }
    }
}
