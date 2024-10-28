using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace YourNamespace.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieApiController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _openAiApiKey;

        public MovieApiController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _openAiApiKey = configuration["OpenAI:ApiKey"];
        }

        [HttpGet("generate-review/{movieTitle}")]
        public async Task<IActionResult> GenerateReview(string movieTitle)
        {
            if (string.IsNullOrEmpty(_openAiApiKey))
            {
                return BadRequest("OpenAI API key is not configured.");
            }

            string prompt = $"Write a short review for the movie titled '{movieTitle}'.";

            var requestBody = new
            {
                model = "text-davinci-003", // Example model; adjust as needed
                prompt = prompt,
                max_tokens = 100
            };

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _openAiApiKey);
            var response = await client.PostAsync("https://api.openai.com/v1/completions",
                new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var reviewResponse = JsonConvert.DeserializeObject<dynamic>(content);
                string reviewText = reviewResponse.choices[0].text;

                return Ok(new { Review = reviewText });
            }

            return StatusCode((int)response.StatusCode, "Failed to generate review.");
        }
    }
}
