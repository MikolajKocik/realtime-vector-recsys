using MediatR;
using RecSys.Application.Abstractions.Output;
using RecSys.Application.Features.Recommendations;

namespace RecSys.Application.CQRS.Queries;

public sealed record GetRecommendationsQuery(
    string UserId,
    int TopK = 5
) : IRequest<Result<RecommendationResponseDto>>;
