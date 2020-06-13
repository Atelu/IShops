using IShops.Data;
using IShops.Dtos;
using IShops.Models;
using IShops.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace IShops.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowAllOrigins")]
  //  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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
        public IActionResult GetProducts()
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
                    Image = country.Image,
                    CategoryId = country.CategoryId,

                });
            }

            return Ok(new
            {
                status = "SUCCESS",
                data = productsDto
            });
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
                Name = product.Name,
                Price = product.Price,
                Description = product.Description,
                Image = product.Image,
                UserId = product.UserId

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
        //api/products/productId
        [HttpPut("{productId}")]
        [ProducesResponseType(204)] // no content
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(422)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateProductAsync(int productId, [FromBody]ProductsModel updatedProductInfo)
        {
            if (updatedProductInfo == null)
                return BadRequest(ModelState);

            if (productId != updatedProductInfo.Id)
                return BadRequest(ModelState);

            if (!_productRepository.ProductExists(productId))
                return NotFound();

            //  if (!_productRepository.IsDuplicateProductName(productId, updatedProductInfo.Name))
            //  {
            //     ModelState.AddModelError("", $"Product {updatedProductInfo.Name} already exist");
            //     return StatusCode(422, ModelState);
            //  }
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

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_productRepository.UpdateProduct(updatedProductInfo))

            {
                ModelState.AddModelError("", $"Something Went Wrong Updating {updatedProductInfo.Name} ");
                return StatusCode(500, ModelState);

            }
           // return NoContent();
            return StatusCode(200, new
            {
                status = "SUCCESS",
                message = "Updated Successfully"
            });
        }

        //api/products/5
        [HttpDelete("producId")]
        [ProducesResponseType(204)] // no content
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [ProducesResponseType(422)]
        [ProducesResponseType(500)]
        public IActionResult DeleteProduct(int productId)
        {
            if (!_productRepository.ProductExists(productId))
                return NotFound();

            var productToDelete = _productRepository.GetProduct(productId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_productRepository.DeleteProduct(productToDelete))

            {
                ModelState.AddModelError("", $"Something Went Wrong Deleting {productToDelete.Name} ");
                return StatusCode(500, ModelState);

            }
            return NoContent();

         //   return StatusCode(200, new
          //  {
          //      status = "SUCCESS",
            //    message = "Deleted Successfully"
           // });
        }
    }
}
