using AutoMapper;
using BestProductManager.Api.Dtos.Products;
using BestProductManager.Api.Entities;
using BestProductManager.Services.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BestProductManager.Api.Controllers.Products
{
    /// <summary>
    /// API quản lý sản phẩm BestProductManager.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IMapper _mapper;
        private readonly ILogger<ProductsController> _logger;

        /// <summary>
        /// Khởi tạo ProductsController với dịch vụ sản phẩm và logger.
        /// </summary>
        public ProductsController(
            IProductService productService,
            ILogger<ProductsController> logger,
            IMapper mapper)
        {
            _productService = productService;
            _logger = logger;
            _mapper = mapper;
        }

        // GET api/products/getallproducts
        [HttpGet("getallproduct")]
        public async Task<IActionResult> GetAll()
        {
            var product = await _productService.GetAllProductsAsync();
            return Ok(new { success = true, data = product });
        }

        // GET api/products/searchproduct?5
        [HttpGet("searchproduct")]
        public async Task<IActionResult> SearchProduct([FromQuery] string keyword)
        {
            var product = await _productService.SearchProductsAsync(keyword ?? "");
            if (product == null) return NotFound(new { success = false, message = "Not Found" });
            return Ok(new { success = true, data = product });
        }

        // POST api/products/create
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] UpdProductDto dto)
        {
            var product = _mapper.Map<Product>(dto);
            var newId = await _productService.CreateAsync(product);
            var keyword = newId;
            var url = Url.Action(nameof(SearchProduct), "Products", new { keyword }, Request.Scheme);

            return Created(url, new { success = true, newId });
        }

        // PUT api/products/update/5
        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdProductDto dto)
        {
            var product = _mapper.Map<Product>(dto);
            var ok = await _productService.UpdateAsync(id, product);
            if (!ok) return NotFound(new { success = false, message = "Not Found" });
            return Ok(new { success = true });
        }

        // DELETE api/products/delete/5
        [HttpDelete("detete/{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _productService.DeleteAsync(id);
            if (!ok) return NotFound(new { success = false, message = "Not Found" });
            return Ok(new { success = true });
        }
    }
}
