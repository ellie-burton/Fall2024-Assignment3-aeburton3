namespace Fall2024_Assignment3_aeburton3.Models
{
    public class GenerateReviewsRequest
    {
        public string MovieTitle { get; set; }
        public int Count { get; set; } = 10; // Default to 10 if not provided
    }

}
