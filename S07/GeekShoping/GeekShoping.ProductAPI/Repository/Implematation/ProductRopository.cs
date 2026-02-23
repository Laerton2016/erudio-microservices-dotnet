using AutoMapper;
using GeekShoping.ProductAPI.Data.ValueObjects;
using GeekShoping.ProductAPI.Model;
using GeekShoping.ProductAPI.Model.Context;
using Microsoft.EntityFrameworkCore;

namespace GeekShoping.ProductAPI.Repository.Implematation
{
    public class ProductRopository : IProductRopository
    {
        private readonly MySQLContext _context;
        private IMapper _mapper;


        public ProductRopository(MySQLContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductVO>> FindAll()
        {
            List<Product> products = await _context.Products.ToListAsync();
            return _mapper.Map<List<ProductVO>>(products);
        }

        public async Task<ProductVO> FindById(long id)
        {
            Product? product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            return _mapper.Map<ProductVO>(product);
        }


        public async Task<ProductVO> Update(ProductVO vo)
        {
            Product productEntity = _mapper.Map<Product>(vo);
            _context.Products.Update(productEntity);
            await _context.SaveChangesAsync();

            return _mapper.Map<ProductVO>(productEntity);
        }

        public async Task<ProductVO> Create(ProductVO vo)
        {
            Product productEntity = _mapper.Map<Product>(vo);
            _context.Products.Add(productEntity);
            await _context.SaveChangesAsync();

            return _mapper.Map<ProductVO>(productEntity);
        }

        public async Task<bool> Delete(long id)
        {
            try
            {
                Product? product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
                if (product == null) 
                    return false;
                
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }


    }
}
