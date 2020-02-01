using IShops.Data;
using IShops.Dtos;
using IShops.Models;
using IShops.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IShops.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowAllOrigins")]
   // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ProductsController : Controller
    {
        private IProductRepository _productRepository;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;


        public ProductsController(ApplicationDbContext context, IProductRepository productRepository, UserManager<IdentityUser> userManager)
        {
            _productRepository = productRepository;
            _userManager = userManager;
            _context = context;

        }
        //api/products
        [HttpGet]
        [ProducesResponseType(400)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ProductDto>))]
        public IActionResult GetCountries()
        {
            var countries = _productRepository.GetProducts().ToList();
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var productsDto = new List<ProductDto>();
            foreach (var country in countries)
            {
                productsDto.Add(new ProductDto
                {
                    Id = country.Id,
                    Name = country.Name,
                    UserId = country.UserId,
                    UserInfo = country.UserInfo,
                    Price = country.Price,
                    Description = country.Description,

                });
            }

            return Ok(productsDto);
        }

        //api/products/ProductId
        [HttpGet("{ProductId}", Name = "GetProduct")]
        [ProducesResponseType(400)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ProductDto>))]
        public IActionResult GetCounty(int ProductId)
        {
            if (!_productRepository.ProductExists(ProductId))
                return NotFound();

            var product = _productRepository.GetProduct(ProductId);
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var productDto = new ProductDto()
            {
                Id = product.Id,
                Name = product.Name

            };


            return Ok(productDto);
        }

        //api/products
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(ProductsModel))]
        [ProducesResponseType(400)]
        [ProducesResponseType(422)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateProductAsync([FromBody]ProductsModel productToCreate)
        {
            if (productToCreate == null)
                return BadRequest(ModelState);

            var product = _productRepository.GetProducts()
                .Where(c => c.Name.Trim().ToUpper() == productToCreate.Name.Trim().ToUpper())
           .FirstOrDefault();

            var user = _userManager.GetUserId(User);
            if (user == null)
            {
                return StatusCode(200, new
                {
                    status = 201,
                    message = "User not found"
                });
            }
            var users = await _context.UserInfo.AsNoTracking().SingleOrDefaultAsync(u => u.AccountId == user);
            if (users == null)
            {
                return Ok(new
                {
                    status = "Error",
                    message = "User Authentication error"
                });
            }

            if (product != null)
            {
                ModelState.AddModelError("", $"Product {productToCreate.Name} already exist");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            productToCreate.UserId = users.AccountId;

            if (!_productRepository.CreateProduct(productToCreate))

            {
                ModelState.AddModelError("", $"Something Went Wrong Saving {productToCreate.Name} ");
                return StatusCode(500, ModelState);
            }
            return CreatedAtRoute("GetProduct", new { ProductId = productToCreate.Id }, productToCreate);

        }

    }
}
