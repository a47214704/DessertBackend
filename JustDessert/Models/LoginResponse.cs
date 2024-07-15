namespace JustDessert.Models
{
	public class LoginResponse
	{
		public User User { get; set; }

		public string JwtToken { get; set; }
	}
}
