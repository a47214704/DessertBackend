namespace JustDessert.Models
{
	public class FileUploadResult
	{
		public bool Success { get; set; }

		public string? FileName { get; set; }

		public string? Url { get; set; }

		public string Msg { get; set; } = "success";
	}
}
