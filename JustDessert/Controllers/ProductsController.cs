using JustDessert.Models;
using Microsoft.AspNetCore.Mvc;

namespace JustDessert.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ProductsController : ControllerBase
	{
		private readonly AppDbContext _context;

		public ProductsController(AppDbContext appDbContext)
		{
			_context = appDbContext;
		}

		[HttpGet("id")]
		public ActionResult<WebApiResult<Product>> Get([FromRoute] long id)
		{
			var product = _context.Products.SingleOrDefault(p => p.Id == id);
			return this.Ok(new WebApiResult<Product>(product));
		}

		[HttpGet]
		public ActionResult<WebApiResult<List<Product>>> GetAll()
		{
			var products = _context.Products.ToList();
			return this.Ok(new WebApiResult<List<Product>>(products));
		}

		[HttpPost]
		public ActionResult<WebApiResult<Product>> Post([FromBody] Product product)
		{
			if (product == null || product.Name == string.Empty)
			{
				return this.BadRequest(new WebApiResult<Product>("invalid data"));
			}

			_context.Products.Add(product);
			_context.SaveChanges();
			return this.Ok(new WebApiResult<Product>(product));
		}

		[HttpPut("id")]
		public ActionResult<WebApiResult<Product>> Put([FromRoute] long id, [FromBody] Product product)
		{
			if (product == null || product.Name == string.Empty)
			{
				return this.BadRequest(new WebApiResult<Product>("invalid data"));
			}

			var existing = _context.Products.SingleOrDefault(p => p.Id == id);
			if (existing == null)
			{
				return this.NotFound(new WebApiResult<Product>("data is not exist"));
			}

			existing.Name = product.Name;
			existing.Description = product.Description;
			existing.Inventory = product.Inventory;
			_context.Products.Update(existing);

			return this.Ok(new WebApiResult<Product>(existing));
		}

		[HttpDelete("id")]
		public ActionResult<WebApiResult<Product>> Delete([FromRoute] long id)
		{
			var existing = _context.Products.SingleOrDefault(p => p.Id == id);
			if (existing == null)
			{
				return this.NotFound(new WebApiResult<Product>("data is not exist"));
			}

			_context.Products.Remove(existing);
			return this.NoContent();
		}
	}
}
