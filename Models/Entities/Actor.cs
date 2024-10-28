using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Fall2024_Assignment3_aeburton3.Models.Entities
{
    public class Actor
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = default!;

        public string? Gender { get; set; }

        [Required]
        public int Age { get; set; }

        public string? IMDBLink { get; set; }

        public byte[]? Photo { get; set; }

        // Relationship with Movie
        public virtual ICollection<MovieActor>? MovieActors { get; set; }
    }
}
