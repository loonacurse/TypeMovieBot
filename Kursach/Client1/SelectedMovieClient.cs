using Newtonsoft.Json;
using System.Diagnostics;
using TelegramBotApi.Model1;

namespace Kursach.Client1
{
    internal class SelectedMovieClient
    {
        private HttpClient _httpClient;
        private static string _address;
        public SelectedMovieClient()
        {
            _address = Constants.address;
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(_address);
        }

        public async Task<List<Selected_Array>> GetSelectedMovieListAsync(long id)
        {
            var response = await _httpClient.GetAsync($"Movies/get_selected_list?id={id}");
            response.EnsureSuccessStatusCode();
            var content = response.Content.ReadAsStringAsync().Result;
            var result = JsonConvert.DeserializeObject<List<Selected_Array>>(content);
            return result;
        }
        public async Task<string> AddSelectedMovieAsync(string movie_name, long id)
        {
            var data = new { movie_name = movie_name};
            var jsonContent = new StringContent(JsonConvert.SerializeObject(data), System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"Movies/add_selected_movie?id={id}", jsonContent);
            if (response.IsSuccessStatusCode)
            {
                return "☆фільм успішно додано☆";
            }
            else
            {
                return "☆сталася помилка. спробуйте ще раз☆";
            }

        } 
        public async Task<string> DeleteSelectedMovieAsync(string movie_name, long id)
        {
            var response = await _httpClient.DeleteAsync($"Movies/delete_selected_movie?movie_name={movie_name}&id={id}");
            if (response.IsSuccessStatusCode)
            {
                return "☆фільм успішно видалено☆";
            }
            else
            {
                return "☆сталася помилка. спробуйте ще раз☆";
            }
        } 
        public async Task<string> UpdateSelectedMovieAsync(string movie_name, string new_movie_name, long id)
        {
            var response = await _httpClient.PutAsync($"Movies/update_selected_movie?movie_name={movie_name}&new_name={new_movie_name}&id={id}", null);
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
