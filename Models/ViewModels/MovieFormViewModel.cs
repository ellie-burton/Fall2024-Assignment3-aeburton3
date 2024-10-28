using Fall2024_Assignment3_aeburton3.Models.Entities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Fall2024_Assignment3_aeburton3.Models.ViewModels
{
    public class MovieFormViewModel
    {
        [Required]
        public int Id { get; set; }  // Add this property for Edit operations

        [Required]
        public string Title { get; set; }

        [Required]
        public string Genre { get; set; }

        [Required]
        public int YearOfRelease { get; set; }

        public string? IMDBLink { get; set; }
        public string? PosterUrl { get; set; }

        // Multi-select for Actors
        public IEnumerable<int>? SelectedActorIds { get; set; }
        public IEnumerable<Actor>? AvailableActors { get; set; }

        // Convert ViewModel to Movie entity
        public Movie ToEntity()
        {
            return new Movie
            {
                Id = Id,  // Make sure the Id is set when converting to the entity
                Title = Title,
                Genre = Genre,
                YearOfRelease = YearOfRelease,
                IMDBLink = IMDBLink,
                PosterUrl = PosterUrl
            };
        }
    }
}
