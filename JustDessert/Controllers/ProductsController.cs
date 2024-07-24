using JustDessert.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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

		[HttpGet("{id}")]
		[Authorize]
		public ActionResult<WebApiResult<Product>> Get([FromRoute] long id)
		{
			var product = _context.Products.SingleOrDefault(p => p.Id == id);
			return this.Ok(new WebApiResult<Product>(product));
		}

		[HttpGet]
		[Authorize]
		public ActionResult<WebApiResult<List<Product>>> GetAll()
		{
			var products = _context.Products.ToList();
			return this.Ok(new WebApiResult<List<Product>>(products));
		}

		[HttpPost]
		[Authorize]
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

		[HttpPut("{id}")]
		[Authorize]
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
			_context.SaveChanges();

			return this.Ok(new WebApiResult<Product>(existing));
		}

		[HttpDelete("{id}")]
		[Authorize]
		public ActionResult<WebApiResult<Product>> Delete([FromRoute] long id)
		{
			var existing = _context.Products.SingleOrDefault(p => p.Id == id);
			if (existing == null)
			{
				return this.NotFound(new WebApiResult<Product>("data is not exist"));
			}

			_context.Products.Remove(existing);
			_context.SaveChanges();
			return this.NoContent();
		}

		[HttpPost("{id}/UploadFile")]
		[Authorize]
		public async Task<ActionResult<WebApiResult<FileUploadResult>>> FileUpload([FromRoute] long id, [FromForm] IFormFile file)
		{
			var existing = _context.Products.SingleOrDefault(p => p.Id == id);
			if (existing == null)
			{
				return this.NotFound(new WebApiResult<Product>("data is not exist"));
			}

			if (file.Length <= 0)
			{
				return this.BadRequest(new WebApiResult<Product>("no file to upload"));
			}

			var result = await this.UploadFile(file);
			if(!result.Success)
			{
				return this.BadRequest(new WebApiResult<Product>(result.Msg));
			}

			existing.ImageUrl = result.Url ?? string.Empty;
			_context.SaveChanges();
			return this.Ok(new WebApiResult<FileUploadResult>(result));
		}

		public async Task<FileUploadResult> UploadFile(IFormFile file)
		{
			try
			{
				string extension = Path.GetExtension(file.FileName);
				if (string.IsNullOrEmpty(extension))
				{
					extension = ".oct";
				}

				string container = extension.Substring(1);
				string fileName = new StringBuilder(Guid.NewGuid().ToString()).Append(extension).ToString();
				string directoryPath = Path.Combine("wwwroot/uploads", container);
				string fullPath = Path.Combine(directoryPath, fileName);
				Directory.CreateDirectory(directoryPath);
				using (var stream = new FileStream(fullPath, FileMode.Create))
				{
					await file.CopyToAsync(stream);
				}

				return new FileUploadResult
				{
					Success = true,
					FileName = fileName,
					Url = new StringBuilder("/uploads")
						.Append('/')
						.Append(container)
						.Append('/')
						.Append(fileName)
						.ToString()
				};
			}
			catch (Exception exception)
			{
				return new FileUploadResult
				{
					Success = false,
					Msg = "failed to save file"
				};
			}
		}
	}
}
