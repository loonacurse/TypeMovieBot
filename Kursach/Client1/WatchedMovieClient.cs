using Newtonsoft.Json;
using TelegramBotApi.Model1;

namespace Kursach.Client1
{
    internal class WatchedMovieClient
    {
        private HttpClient _httpClient;
        private static string _address;
        public WatchedMovieClient()
        {
            _address = Constants.address;
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(_address);
        }

        public async Task<List<Movie>> GetWatchedMovieListAsync(long id)
        {
            var response = await _httpClient.GetAsync($"Movies/get_watched_list?id={id}");
            response.EnsureSuccessStatusCode();
            var content = response.Content.ReadAsStringAsync().Result;
            var result = JsonConvert.DeserializeObject<List<Movie>>(content);
            return result;
        }
        public async Task<string> AddWatchedMovieAsync(Movie movie,  long id)
        {
            var data = new { movie_name = movie.Movie_name, movie_rate = movie.Movie_rate, movie_comment = movie.Movie_comment };
            var jsonContent = new StringContent(JsonConvert.SerializeObject(data), System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"Movies/add_watched_movie?id={id}", jsonContent);
            if (response.IsSuccessStatusCode)
            {
                return "☆фільм успішно додано☆";
            }
            else
            {
                return "☆сталася помилка. спробуйте ще раз☆";
            }

        }
        public async Task<string> DeleteWatchedMovieAsync(string movie_name, long id)
        {
            var response = await _httpClient.DeleteAsync($"Movies/delete_watched_movie?movie_name={movie_name}&id={id}");
            if (response.IsSuccessStatusCode)
            {
                return "☆фільм успішно видалено☆";
            }
            else
            {
                return "☆сталася помилка. спробуйте ще раз☆";
            }
        }
        public async Task<string> UpdateWatchedMovieNameAsync(string movie_name, string new_movie_name, long id)
        {
            var response = await _httpClient.PutAsync($"Movies/update_watched_movie_name?movie_name={movie_name}&new_name={new_movie_name}&id={id}", null);
            if (response.IsSuccessStatusCode)
            {
                return "☆фільм успішно змінено☆";
            }
            else
            {
                return "☆сталася помилка. спробуйте ще раз☆";
            }
        }
        public async Task<string> UpdateWatchedMovieRateAsync(string movie_name, int new_rate, long id)
        {
            var response = await _httpClient.PutAsync($"Movies/update_watched_movie_rate?movie_name={movie_name}&new_rate={new_rate}&id={id}", null);
            if (response.IsSuccessStatusCode)
            {
                return "☆фільм успішно змінено☆";
            }
            else
            {
                return "☆сталася помилка. спробуйте ще раз☆";
            }
        }
        public async Task<string> UpdateWatchedMovieCommentAsync(string movie_name, string new_comment, long id)
        {
            var response = await _httpClient.PutAsync($"Movies/update_watched_movie_comment?movie_name={movie_name}&new_comment={new_comment}s&id={id}", null);
            if (response.IsSuccessStatusCode)
            {
                return "☆фільм успішно змінено☆";
            }
            else
            {
                return "☆сталася помилка. спробуйте ще раз☆";
            }
        }
    }
}
