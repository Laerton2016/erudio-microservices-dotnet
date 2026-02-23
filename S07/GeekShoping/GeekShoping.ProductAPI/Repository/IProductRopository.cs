using GeekShoping.ProductAPI.Data.ValueObjects;
using System.Collections;

namespace GeekShoping.ProductAPI.Repository
{
    public interface IProductRopository
    {
        Task<IEnumerable<ProductVO>> FindAll();
        Task<ProductVO> FindById(long id);
        Task<ProductVO> Create(ProductVO product);
        Task<ProductVO> Update(ProductVO product);
        Task<bool> Delete(long id);


    }
}
