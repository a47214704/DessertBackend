using JustDessert.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace JustDessert.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UsersController : ControllerBase
	{
		private readonly AppDbContext _context;
		private readonly IConfiguration _configuration;

		public UsersController(AppDbContext context, IConfiguration configuration)
		{
			_context = context;
			_configuration = configuration;
		}

		[HttpGet]
		[Authorize]
		public ActionResult<WebApiResult<List<User>>> Get()
		{
			var users = _context.Users.ToList();
			return this.Ok(new WebApiResult<List<User>>(users));
		}


		[HttpGet("{id}")]
		[Authorize]
		public ActionResult<WebApiResult<User>> Get(int id)
		{
			var user = _context.Users.SingleOrDefault(p => p.Id == id);
			return this.Ok(new WebApiResult<User>(user));
		}


		[HttpPost]
		public ActionResult<WebApiResult<User>> Post([FromBody] User user)
		{
			if (user == null || user.Name == string.Empty || user.Password == string.Empty)
			{
				return this.BadRequest(new WebApiResult<User>("invalid data"));
			}

			var existing = _context.Users.SingleOrDefault(p => p.Name == user.Name);
			if (existing != null)
			{
				return this.BadRequest(new WebApiResult<User>("user already exist"));
			}

			HashAlgorithm hashAlgorithm = SHA256.Create();
			var sha256 = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(user.Password) ?? throw new ArgumentNullException("data"));
			var passwordHash = BitConverter.ToString(sha256).ToLowerInvariant().Replace("-", string.Empty);

			user.Password = passwordHash;
			_context.Users.Add(user);
			_context.SaveChanges();
			return this.Ok(new WebApiResult<User>(user));
		}


		[HttpPost("Login")]
		public ActionResult<WebApiResult<LoginResponse>> Login([FromBody] LoginRequest user)
		{
			if (user.Name == string.Empty || user.Password == string.Empty)
			{
				return this.BadRequest(new WebApiResult<User>("invalid data"));
			}

			HashAlgorithm hashAlgorithm = SHA256.Create();
			var sha256 = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(user.Password) ?? throw new ArgumentNullException("data"));
			var passwordHash = BitConverter.ToString(sha256).ToLowerInvariant().Replace("-", string.Empty);

			var existing = _context.Users.SingleOrDefault(x => x.Name == user.Name && x.Password == passwordHash);
			if (existing == null)
			{
				return this.NotFound(new WebApiResult<User>("user not found or password fail"));
			}

			return this.HandleLoginSuccess(existing);
		}


		[HttpPut("{id}")]
		[Authorize]
		public ActionResult<WebApiResult<User>> Put(int id, [FromBody] User user)
		{
			if (user == null || user.Name == string.Empty)
			{
				return this.BadRequest(new WebApiResult<User>("invalid data"));
			}

			var existing = _context.Users.SingleOrDefault(p => p.Id == id);
			if (existing == null)
			{
				return this.NotFound(new WebApiResult<User>("data is not exist"));
			}

			existing.Email = user.Email;
			existing.Category = user.Category;
			_context.SaveChanges();

			return this.Ok(new WebApiResult<User>(existing));
		}


		[HttpDelete("{id}")]
		[Authorize]
		public ActionResult<WebApiResult<User>> Delete([FromRoute] long id)
		{
			var existing = _context.Users.SingleOrDefault(p => p.Id == id);
			if (existing == null)
			{
				return this.NotFound(new WebApiResult<User>("data is not exist"));
			}

			_context.Users.Remove(existing);
			_context.SaveChanges();
			return this.NoContent();
		}

		public ActionResult<WebApiResult<LoginResponse>> HandleLoginSuccess(User user)
		{
			var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
			var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

			var claims = new[]
			{
				new Claim(JwtRegisteredClaimNames.Sub, user.Name),
				new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
			};

			var token = new JwtSecurityToken(
				issuer: _configuration["Jwt:Issuer"],
				audience: _configuration["Jwt:Audience"],
				claims: claims,
				expires: DateTime.Now.AddDays(3),
				signingCredentials: credentials);

			var tokenHandler = new JwtSecurityTokenHandler();
			var tokenString = tokenHandler.WriteToken(token);
			var response = new LoginResponse
			{
				User = user,
				JwtToken = tokenString,
			};

			return this.Ok(new WebApiResult<LoginResponse>(response));
		}
	}
}
