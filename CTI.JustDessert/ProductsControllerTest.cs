using JustDessert.Controllers;
using JustDessert.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CTI.JustDessert
{
	[TestClass]
	public class ProductsControllerTest
	{
		private AppDbContext _context;
		private ProductsController _productService;

		[TestInitialize]
		public void Initialize()
		{
			var options = new DbContextOptionsBuilder<AppDbContext>()
				.UseInMemoryDatabase(databaseName: "TestDatabase")
				.Options;
			_context = new AppDbContext(options);
			_productService = new ProductsController(_context);
		}


		[TestMethod]
		public void TestCreateProduct()
		{
			var product = new Product
			{
				Name = string.Empty,
				Description = "Test",
				Status = 0,
				Inventory = 5
			};

			var reponse = _productService.Post(product);
			Assert.IsNotNull(reponse.Result);
			Assert.IsInstanceOfType(reponse.Result, typeof(BadRequestObjectResult));

			product.Name = "Test";
			_productService.Post(product);
			var result = _context.Products.FirstOrDefault(p => p.Name == product.Name);
			Assert.IsNotNull(result);
			Assert.AreEqual(product.Name, result.Name);
			Assert.AreEqual(result.Description, result.Description);
		}


		[TestMethod]
		public void TestGetProduct()
		{
			var product = new Product
			{
				Name = "Test",
				Description = "Test",
				Status = 0,
				Inventory = 5
			};

			_productService.Post(product);
			var result = _productService.Get(product.Id);
			Assert.IsNotNull(result.Result);
			var productResult = ((OkObjectResult)result.Result).Value as WebApiResult<Product>;
			Assert.IsNotNull(productResult?.Result);

			Assert.AreEqual(product.Name, productResult.Result.Name);
			Assert.AreEqual(product.Description, productResult.Result.Description);
		}


		[TestMethod]
		public void TestGetListProduct()
		{
			for (var i = 0; i < 10; i++)
			{
				var product = new Product
				{
					Name = "Test " + i,
					Description = "this is " + i,
					Status = 0,
					Inventory = i
				};
				_productService.Post(product);
			}

			var result = _productService.GetAll();
			Assert.IsNotNull(result.Result);
			var productResult = ((OkObjectResult)result.Result).Value as WebApiResult<List<Product>>;
			Assert.IsNotNull(productResult?.Result);

			Assert.AreEqual(10, productResult.Result.Count);
		}


		[TestMethod]
		public void TestEditProduct()
		{
			var product = new Product
			{
				Name = "Test",
				Description = "Test",
				Status = 0,
				Inventory = 5
			};

			_productService.Post(product);
			var result = _context.Products.FirstOrDefault(p => p.Name == product.Name);
			Assert.IsNotNull(result);

			result.Description = "this is cake";
			_productService.Put(result.Id, result);

			result = _context.Products.FirstOrDefault(p => p.Name == product.Name);
			Assert.IsNotNull(result);
			Assert.AreEqual("this is cake", result.Description);
		}


		[TestMethod]
		public void TestDeleteProduct()
		{
			var product = new Product
			{
				Name = "Test",
				Description = "Test",
				Status = 0,
				Inventory = 5
			};

			var result = _productService.Post(product);
			var productResult = _context.Products.FirstOrDefault(p => p.Name == product.Name);
			Assert.IsNotNull(productResult);

			_productService.Delete(product.Id);
			productResult = _context.Products.FirstOrDefault(p => p.Name == product.Name);
			Assert.IsNull(productResult);
		}
	}
}