using Kursach;
using Newtonsoft.Json;
using System.Reflection;
using TelegramBotApi.Model1;

namespace TelegramBotApi
{
    public class GetActorMovieClient
    {
        private HttpClient _httpClient;
        private static string _address;
        public GetActorMovieClient()
        {
            _address = Constants.address;
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(_address);
        }

        public async Task<MovieActorModel> GetActorMovieAsync(string actor_name)
        {
            var response = await _httpClient.GetAsync($"Movies/get_movie_by_person?actor_name={actor_name}");
            response.EnsureSuccessStatusCode();
            var content = response.Content.ReadAsStringAsync().Result;
            var result = JsonConvert.DeserializeObject<MovieActorModel>(content);
            return result;

        }
    }
}
