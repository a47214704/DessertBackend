namespace JustDessert.Models
{
	public class WebApiResult<T>
	{
		public string ErrorMessage { get; set; } = string.Empty;

		public T? Result { get; set; }

		public WebApiResult(T? result)
			: this("success", result)
		{
		}

		public WebApiResult(string error, T? result)
		{
			ErrorMessage = error ?? "success";
			Result = result;
		}

		public WebApiResult(string error)
		{
			ErrorMessage = error;
		}
	}
}
