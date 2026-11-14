using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServicePulse.Application.DTOs;
using ServicePulse.Application.Services;
using ServicePulse.Api.Services;

namespace ServicePulse.Api.Controllers;

[Authorize(Roles = "Manager,Admin")]
[ApiController]
[Route("api/[controller]")]
public class ManagerReviewsController : ControllerBase
{
    private readonly IManagerReviewService _reviewService;
    private readonly AuthService _authService;

    public ManagerReviewsController(IManagerReviewService reviewService, AuthService authService)
    {
        _reviewService = reviewService;
        _authService = authService;
    }

    private Guid GetCurrentUserId()
    {
        var user = _authService.GetCurrentUser();
        return user?.Id ?? Guid.Empty;
    }

    [HttpPost]
    public async Task<IActionResult> CreateReview([FromBody] CreateManagerReviewRequest request)
    {
        if (request.CallId == Guid.Empty)
            return BadRequest(new { message = "Call ID is required." });

        if (string.IsNullOrWhiteSpace(request.ReviewStatus))
            return BadRequest(new { message = "Review status is required." });

        var validStatuses = new[] { "Pending", "Reviewed", "Approved", "Rejected" };
        if (!validStatuses.Contains(request.ReviewStatus))
            return BadRequest(new { message = "Invalid review status. Valid values: Pending, Reviewed, Approved, Rejected" });

        if (request.ManagerScore.HasValue && (request.ManagerScore < 0 || request.ManagerScore > 100))
            return BadRequest(new { message = "Manager score must be between 0 and 100." });

        try
        {
            var reviewerUserId = GetCurrentUserId();
            var review = await _reviewService.CreateReviewAsync(reviewerUserId, request);
            return CreatedAtAction(nameof(GetReview), new { id = review.Id }, review);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = "An error occurred while creating the review." });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetReview(Guid id)
    {
        var review = await _reviewService.GetReviewAsync(id);
        if (review == null)
            return NotFound();

        return Ok(review);
    }

    [HttpGet("call/{callId}")]
    public async Task<IActionResult> GetReviewsForCall(Guid callId)
    {
        var reviews = await _reviewService.GetReviewsForCallAsync(callId);
        return Ok(reviews);
    }

    [HttpGet("call/{callId}/latest")]
    public async Task<IActionResult> GetLatestReviewForCall(Guid callId)
    {
        try
        {
            var review = await _reviewService.GetLatestReviewForCallAsync(callId);
            return Ok(review);
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateReview(Guid id, [FromBody] UpdateManagerReviewRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.ReviewStatus))
            return BadRequest(new { message = "Review status is required." });

        var validStatuses = new[] { "Pending", "Reviewed", "Approved", "Rejected" };
        if (!validStatuses.Contains(request.ReviewStatus))
            return BadRequest(new { message = "Invalid review status. Valid values: Pending, Reviewed, Approved, Rejected" });

        if (request.ManagerScore.HasValue && (request.ManagerScore < 0 || request.ManagerScore > 100))
            return BadRequest(new { message = "Manager score must be between 0 and 100." });

        try
        {
            var reviewerUserId = GetCurrentUserId();
            var review = await _reviewService.UpdateReviewAsync(id, reviewerUserId, request);
            return Ok(review);
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = "An error occurred while updating the review." });
        }
    }
}