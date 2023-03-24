using MinimalAuthAPI.Models;

namespace MinimalAuthAPI.Repositories
{
    public class MovieRepository
    {
        public static List<Movie> Movies = new()
        {
            new() {
                Id = 1,
                Title = "Eternals",
                Description = "The saga of the Eternals, a very overrated concept.",
                Rating = 6.8 
            },
            new() {
                Id = 2,
                Title = "Dune",
                Description = "Sand and sci-fi and planets and what not.",
                Rating = 8.2 
            },
            new() {
                Id = 3,
                Title = "The Harder They Fall",
                Description = "Outlaw discovers that an enemy of his pulled a prison break.",
                Rating = 6.6 
            },
            new() {
                Id = 4,
                Title = "Red Notice",
                Description = "An Interpol agent tracks the world's most wanted art thief.",
                Rating = 6.4 
            },
            new() {
                Id = 5,
                Title = "No Time to Die",
                Description = "James Bond has left active service. But for how long.",
                Rating = 7.4 
            },
        };
    }
}
