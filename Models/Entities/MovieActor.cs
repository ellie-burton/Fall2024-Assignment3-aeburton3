namespace Fall2024_Assignment3_aeburton3.Models.Entities
{
    public class MovieActor
    {
        public int MovieId { get; set; }
        public Movie Movie { get; set; } = default!;

        public int ActorId { get; set; }
        public Actor Actor { get; set; } = default!;
    }
}
