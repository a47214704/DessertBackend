using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JustDessert.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ProductsController : ControllerBase
	{
		[HttpGet]
		public async Task<ActionResult> Get()
		{
			return this.Ok();
		}

		[HttpPost]
		public async Task<ActionResult> Post()
		{
			return this.Ok();
		}

		[HttpPut]
		public async Task<ActionResult> Put()
		{
			return this.Ok();
		}

		[HttpDelete]
		public async Task<ActionResult> Delete()
		{
			return this.Ok();
		}
	}
}
