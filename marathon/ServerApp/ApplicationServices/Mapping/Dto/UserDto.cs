namespace RunTogether.ApplicationServices.Mapping.Dto
{
	public class UserDto
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Email { get; set; }
		public string Password { get; set; }
		public string PasswordConfirm { get; set; }
		public string Picture { get; set; }
		public string PictureType { get; set; }

		public void Clear()
		{
			Id = 0;
			Name = string.Empty;
			Email = string.Empty;
			Password = string.Empty;
			PasswordConfirm = string.Empty;
			Picture = string.Empty;
			PictureType = string.Empty;
		}
	}
}
