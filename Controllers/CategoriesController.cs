using IShops.Dtos;
using IShops.Models;
using IShops.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IShops.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : Controller
    {
        private ICategoryRepository _categoryRepository;
        public CategoriesController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;

        }
        //api/categories
        [HttpGet]
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

        //api/categories/CategoryId
        [HttpGet("{CategoryId}", Name = "GetCategory")]
        [ProducesResponseType(400)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<CategoryDto>))]
        public IActionResult GetCategory(int CategoryId)
        {
            if (!_categoryRepository.CategoryExists(CategoryId))
                return NotFound();

            var category = _categoryRepository.GetCategory(CategoryId);
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var categoryDto = new CategoryDto()
            {
                Id = category.Id,
                Name = category.Name,
               
               

            };


            return Ok(categoryDto);
        }



        //api/categories
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Category))]
        [ProducesResponseType(400)]
        [ProducesResponseType(422)]
        [ProducesResponseType(500)]
        public IActionResult CreateProduct([FromBody]Category categoryToCreate)
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
