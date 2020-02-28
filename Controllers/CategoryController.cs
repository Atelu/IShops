using IShops.Data;
using IShops.Dtos;
using IShops.Models;
using IShops.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IShops.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowAllOrigins")]
    public class CategoryController :  Controller
    {
        private ICategoryRepository _categoryRepository;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;


        public CategoryController(ApplicationDbContext context, ICategoryRepository categoryRepository, UserManager<IdentityUser> userManager)
        {
            _categoryRepository = categoryRepository;
            _userManager = userManager;
            _context = context;

        }
        //api/categories
        [HttpGet]
        [ProducesResponseType(400)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<CategoryDto>))]
        public IActionResult GetCategories()
        {
            var cat = _categoryRepository.GetCategories().ToList();
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var categoryDto = new List<CategoryDto>();
            foreach (var category in cat)
            {
                categoryDto.Add(new CategoryDto
                {
                    Id = category.Id,
                    Name = category.Name,
                

                });
            }

            return Ok(new
            {
                status = "SUCCESS",
                data = categoryDto
            });
        }
        //api/categories
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Category))]
        [ProducesResponseType(400)]
        [ProducesResponseType(422)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateProductAsync([FromBody]Category categoryToCreate)
        {
            if (categoryToCreate == null)
                return BadRequest(ModelState);

            var cat = _categoryRepository.GetCategories()
                .Where(c => c.Name.Trim().ToUpper() == categoryToCreate.Name.Trim().ToUpper())
           .FirstOrDefault();

         

            if (cat != null)
            {
                ModelState.AddModelError("", $"Category {categoryToCreate.Name} already exist");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            

            if (!_categoryRepository.CreateCategory(categoryToCreate))

            {
                ModelState.AddModelError("", $"Something Went Wrong Saving {categoryToCreate.Name} ");
                return StatusCode(500, ModelState);
            }
            return CreatedAtRoute("GetCategory", new { CategoryId = categoryToCreate.Id }, categoryToCreate);

        }
    }
}
