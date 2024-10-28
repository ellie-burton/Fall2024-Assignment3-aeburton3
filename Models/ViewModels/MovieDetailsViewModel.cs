using Fall2024_Assignment3_aeburton3.Models.Entities;
using System.Collections.Generic;

namespace Fall2024_Assignment3_aeburton3.Models.ViewModels
{
    public class MovieDetailsViewModel
    {
        public Movie Movie { get; set; }
        public List<string> Reviews { get; set; }  // List of AI reviews
        public List<double> Sentiments { get; set; }  // Sentiment scores for each review
        public double OverallSentiment { get; set; }  // Overall sentiment score (average)
    }
}
