using GeekShoping.web.Models;
using GeekShoping.web.Services.IServices;
using GeekShoping.web.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GeekShoping.web.Controllers
{

    [Authorize]
    public class ProductController : Controller
    {
        private readonly ILogger<ProductController> _logger;
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<ProductController>();
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
        }

        
        public async Task<IActionResult> ProductIndex()
        {
            var products = await _productService.FindAll() ?? Enumerable.Empty<ProductModel>();
            return View(products);
        
        }

        [HttpPost]
        public async Task<IActionResult> ProductCreate(ProductModel model)
        {
            if (ModelState.IsValid)
            {
                var response = await _productService.Create(model);
                if (response != null)
                {
                    return RedirectToAction(nameof(ProductIndex));
                } 
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ProductUpdate(ProductModel model)
        {
            if (ModelState.IsValid)
            {
                var response = await _productService.Update(model);
                if (response != null)
                {
                    return RedirectToAction(nameof(ProductIndex));
                }
            }
            return View(model);
        }


        public async Task<IActionResult> ProductUpdate(int id)
        {
            var model = await _productService.FindById(id);
            if (model != null)
            {
                return View(model);
            }
            return NotFound();
        }


        public async Task<IActionResult> ProductCreate()
        {
            return View();
        }


        
        [HttpPost]
        [Authorize(Roles = Role.Admin)]
        public async Task<IActionResult> ProductDelete(int id)
        {
            var response = await _productService.Delete(id);
            if (response)
            {
                return RedirectToAction(nameof(ProductIndex));
            }
            return NoContent();
        }
    }
}
