using System.Text.Json.Serialization;
using api.Entities;

namespace api.DTO;

public class ReviewDTO
{
    public int OrderId { get; set; }
    public int Rating { get; set; }
    public DateTime CreateDateTime { get; set; }
    public string? Comment { get; set; }
    
    [JsonConstructor]
    public ReviewDTO() { }
    public ReviewDTO(Review review)
    {
        OrderId = review.OrderId;
        Rating = review.Rating;
        CreateDateTime = review.CreateDateTime;
        Comment = review.Comment;
    }
}