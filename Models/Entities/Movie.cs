using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Fall2024_Assignment3_aeburton3.Models.Entities
{
    public class Movie
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = default!;

        [Required]
        public string Genre { get; set; } = default!;

        [Required]
        public int YearOfRelease { get; set; }

        public string? IMDBLink { get; set; }

        public string? PosterUrl { get; set; }

        // Relationship with Actor
        public virtual ICollection<MovieActor>? MovieActors { get; set; }
    }
}
