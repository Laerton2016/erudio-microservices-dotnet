using System.Net.Http.Headers;
using System.Text.Json;

namespace GeekShoping.web.Utils
{
    public static class HttpClientExtensions
    {

        private static MediaTypeHeaderValue _mediaTypeHeaderValue = new MediaTypeHeaderValue("application/json");
        public static async Task<T> ReadContentAs<T>(this HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
                throw new ApplicationException($"Something went wrong calling the API: {response.ReasonPhrase}");

            var dataAsScring = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonSerializer.Deserialize<T>(dataAsScring, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public static Task<HttpResponseMessage> PostAsJson<T>(this HttpClient httpClient, string url, T date)
        {
            var dataAsString = JsonSerializer.Serialize(date);
            var content = new StringContent(dataAsString);
            content.Headers.ContentType = _mediaTypeHeaderValue;
            return httpClient.PostAsync(url, content);
        }

        public static Task<HttpResponseMessage> PutAsJson<T>(this HttpClient httpClient, string url, T date)
        {
            var dataAsString = JsonSerializer.Serialize(date);
            var content = new StringContent(dataAsString);
            content.Headers.ContentType = _mediaTypeHeaderValue;
            return httpClient.PutAsync(url, content);
        }
    }
}
