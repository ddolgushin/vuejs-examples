using System;
using System.Collections.Generic;

namespace RunTogether.Dal.Model
{
	public class User
	{
		private DateTime? _createdAt;

		public int Id { get; set; }
		public string Name { get; set; }
		public string Email { get; set; }
		public string PasswordHash { get; set; }
		public string PasswordSalt { get; set; }
		public string Picture { get; set; }
		public string PictureType { get; set; }
		public bool IsAdmin { get; set; }
		public DateTime CreatedAt {
			get => _createdAt.HasValue
				? _createdAt.Value
				: (_createdAt = DateTime.Now).Value;
			set => _createdAt = value;
		}
		public List<MarathonLink> MarathonLinks { get; set; }

		public User()
		{
			MarathonLinks = new List<MarathonLink>();
		}
	}
}
