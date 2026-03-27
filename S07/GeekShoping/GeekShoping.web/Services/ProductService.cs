using GeekShoping.web.Models;
using GeekShoping.web.Services.IServices;
using GeekShoping.web.Utils;

namespace GeekShoping.web.Services
{
    public class ProductService : IProductService
    {
        private readonly HttpClient _client;
        public const string BasePath = "api/v1/Product";

        public ProductService(HttpClient client)
        {
            _client = client;
        }
        public async Task<IEnumerable<ProductModel>> FindAll()
        {
            var response = await _client.GetAsync(BasePath);
            return await response.ReadContentAs<List<ProductModel>>();
        }

        public async Task<ProductModel> FindById(long id)
        {
            var response = await _client.GetAsync($"{BasePath}/{id}");
            return await response.ReadContentAs<ProductModel>();
        }
        public async Task<ProductModel> Create(ProductModel model)
        {
            var response = await _client.PostAsJsonAsync(BasePath, model);
            if (!response.IsSuccessStatusCode)
                throw new Exception("Something went wrong when calling API");

            return await response.ReadContentAs<ProductModel>();

        }

        public async Task<ProductModel> Update(ProductModel model)
        {
            var response = await _client.PutAsJsonAsync(BasePath, model);
            if (!response.IsSuccessStatusCode)
                throw new Exception("Something went wrong when calling API");

            return await response.ReadContentAs<ProductModel>();
        }

        public async Task<bool> Delete(long id)
        {
            var response = await _client.DeleteAsync($"{BasePath}/{id}");
            if (!response.IsSuccessStatusCode)
                throw new Exception("Something went wrong when calling API");

            return await response.ReadContentAs<bool>();
        }



    }
}
