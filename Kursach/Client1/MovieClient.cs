using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramBotApi.Model1;

namespace Kursach.Client
{
    public class MovieClient
    {
        private HttpClient _httpClient;
        private static string _address;
        public MovieClient()
        {
            _address = Constants.address;
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(_address);
        }

        public async Task<Models> GetMovieNowPlayingAsync()
        {
            var response = await _httpClient.GetAsync($"Movies/cinema_movie");
            response.EnsureSuccessStatusCode();
            var content = response.Content.ReadAsStringAsync().Result;
            var result = JsonConvert.DeserializeObject<Models>(content);
            return result;
            
        }
    }
}
