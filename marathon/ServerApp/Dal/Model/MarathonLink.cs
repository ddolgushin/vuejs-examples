using System;
using System.Collections.Generic;

namespace RunTogether.Dal.Model
{
	public class MarathonLink
	{
		public int Id { get; set; }
		public Marathon Marathon { get; set; }
		public int MarathonId { get; set; }
		public User User { get; set; }
		public int UserId { get; set; }
		public MarathonRole Role { get; set; }
	}
}
