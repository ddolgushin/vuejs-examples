using System;
using System.Collections.Generic;

namespace RunTogether.Dal.Model
{
	// Сведения о точках маршрута марафона.
	public class WaypointInfo
	{
		public int Id { get; set; }
		public Marathon Marathon { get; set; }
		public int MarathonId { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public string Picture { get; set; }
		public string PictureType { get; set; }
		public string Location { get; set; }
	}
}
