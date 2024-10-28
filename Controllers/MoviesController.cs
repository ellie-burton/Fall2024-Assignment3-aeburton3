using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Fall2024_Assignment3_aeburton3.Data;
using Fall2024_Assignment3_aeburton3.Models.Entities;
using Fall2024_Assignment3_aeburton3.Models.ViewModels;

namespace Fall2024_Assignment3_aeburton3.Controllers
{
    public class MoviesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly HttpClient _httpClient;

        public MoviesController(ApplicationDbContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:5001/api/MovieApi/"); // Replace with your API base URL
        }

        // GET: Movies
        public async Task<IActionResult> Index()
        {
            return View(await _context.Movies.ToListAsync());
        }

        // GET: Movies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies
                .Include(m => m.MovieActors)
                .ThenInclude(ma => ma.Actor)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null)
            {
                return NotFound();
            }

            // Call API to generate reviews
            var reviewsResponse = await _httpClient.PostAsJsonAsync("GenerateReviews", movie.Title);
            if (!reviewsResponse.IsSuccessStatusCode)
            {
                ViewBag.ErrorMessage = "Failed to retrieve AI-generated reviews.";
                return View(movie);
            }
            var reviews = await reviewsResponse.Content.ReadFromJsonAsync<List<string>>();

            // Call API to analyze sentiment
            var sentimentResponse = await _httpClient.PostAsJsonAsync("AnalyzeSentiment", reviews);
            if (!sentimentResponse.IsSuccessStatusCode)
            {
                ViewBag.ErrorMessage = "Failed to retrieve sentiment analysis.";
                return View(movie);
            }
            var sentiments = await sentimentResponse.Content.ReadFromJsonAsync<List<double>>();

            // Calculate overall sentiment score
            double overallSentiment = sentiments.Any() ? sentiments.Average() : 0;

            // Create ViewModel
            var viewModel = new MovieDetailsViewModel
            {
                Movie = movie,
                Reviews = reviews,
                Sentiments = sentiments,
                OverallSentiment = overallSentiment
            };

            return View(viewModel);
        }

        // Other actions (Index, Create, Edit, Delete) remain unchanged...
 

// GET: Movies/Create
public async Task<IActionResult> Create()
        {
            var viewModel = new MovieFormViewModel
            {
                AvailableActors = await _context.Actors.ToListAsync() // Fetch available actors from the database
            };

            return View(viewModel); // Pass MovieFormViewModel to the view
        }


        // POST: Movies/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MovieFormViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var movie = viewModel.ToEntity();

                _context.Movies.Add(movie);
                await _context.SaveChangesAsync();

                // Associate selected actors with the movie
                if (viewModel.SelectedActorIds != null)
                {
                    foreach (var actorId in viewModel.SelectedActorIds)
                    {
                        var movieActor = new MovieActor
                        {
                            MovieId = movie.Id,
                            ActorId = actorId
                        };
                        _context.MovieActors.Add(movieActor);
                    }
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }

            // If model state is invalid, repopulate the available actors list and return the view
            viewModel.AvailableActors = await _context.Actors.ToListAsync();
            return View(viewModel);
        }


        // GET: Movies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies
                .Include(m => m.MovieActors)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null)
            {
                return NotFound();
            }

            var viewModel = new MovieFormViewModel
            {
                Id = movie.Id,
                Title = movie.Title,
                Genre = movie.Genre,
                YearOfRelease = movie.YearOfRelease,
                IMDBLink = movie.IMDBLink,
                PosterUrl = movie.PosterUrl,
                SelectedActorIds = movie.MovieActors.Select(ma => ma.ActorId).ToList(),
                AvailableActors = await _context.Actors.ToListAsync()
            };

            return View(viewModel);
        }

        // POST: Movies/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MovieFormViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var movie = await _context.Movies
                    .Include(m => m.MovieActors)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (movie == null)
                {
                    return NotFound();
                }

                // Update movie details
                movie.Title = viewModel.Title;
                movie.Genre = viewModel.Genre;
                movie.YearOfRelease = viewModel.YearOfRelease;
                movie.IMDBLink = viewModel.IMDBLink;
                movie.PosterUrl = viewModel.PosterUrl;

                // Clear existing actor associations
                movie.MovieActors.Clear();

                // Update selected actors
                if (viewModel.SelectedActorIds != null)
                {
                    foreach (var actorId in viewModel.SelectedActorIds)
                    {
                        var movieActor = new MovieActor
                        {
                            MovieId = movie.Id,
                            ActorId = actorId
                        };
                        movie.MovieActors.Add(movieActor);
                    }
                }

                try
                {
                    _context.Update(movie);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MovieExists(movie.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            viewModel.AvailableActors = await _context.Actors.ToListAsync();
            return View(viewModel);
        }

        // GET: Movies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // POST: Movies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie != null)
            {
                _context.Movies.Remove(movie);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MovieExists(int id)
        {
            return _context.Movies.Any(e => e.Id == id);
        }
    }
}
