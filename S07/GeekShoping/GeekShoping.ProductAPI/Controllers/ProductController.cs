using GeekShoping.ProductAPI.Data.ValueObjects;
using GeekShoping.ProductAPI.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GeekShoping.ProductAPI.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private IProductRopository _repository;
        public ProductController(IProductRopository ropository)
        {
            _repository = ropository ?? throw new
                ArgumentNullException(nameof(ropository));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductVO>> FindById(long id)
        {
            var product = await _repository.FindById(id);

            if (product == null)
                return NotFound();

            return Ok(product);
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductVO>>> FindAll()
        {
            var products = await _repository.FindAll();
            return Ok(products);
        }

        [HttpPost]
        public async Task<ActionResult<ProductVO>> Create([FromBody] ProductVO productVO)
        {
            if (productVO == null)
                return BadRequest();
            var createdProduct = await _repository.Create(productVO);
            return Ok(createdProduct);

        }
        [HttpPut]
        public async Task<ActionResult<ProductVO>> Update([FromBody] ProductVO productVO)
        {
            if (productVO == null)
                return BadRequest();
            var updatedProduct = await _repository.Update(productVO);
            return Ok(updatedProduct);
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(long id)
        {
            var status = await _repository.Delete(id);
            if (!status)
                return BadRequest();
            return Ok(status);
        }
    }
}

