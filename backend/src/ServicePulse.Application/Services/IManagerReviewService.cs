using ServicePulse.Application.DTOs;
using ServicePulse.Domain.Entities;

namespace ServicePulse.Application.Services;

public interface IManagerReviewService
{
    Task<ManagerReviewDto> CreateReviewAsync(Guid reviewerUserId, CreateManagerReviewRequest request);
    Task<ManagerReviewDto> UpdateReviewAsync(Guid reviewId, Guid reviewerUserId, UpdateManagerReviewRequest request);
    Task<ManagerReviewDto?> GetReviewAsync(Guid reviewId);
    Task<List<ManagerReviewDto>> GetReviewsForCallAsync(Guid callId);
    Task<ManagerReviewDto> GetLatestReviewForCallAsync(Guid callId);
}