using System;
using System.Collections.Generic;

namespace RunTogether.Dal.Model
{
	public partial class Marathon
	{
		public int Id { get; set; }
		public string Title { get; set; }
		public DateTime Date { get; set; }
		public string Description { get; set; }
		public float? Distance { get; set; }
		public string Route { get; set; }
		public List<MarathonLink> MarathonLinks { get; set; }
		public List<WaypointInfo> WaypointInfos { get; set; }

		public Marathon()
		{
			MarathonLinks = new List<MarathonLink>();
			WaypointInfos = new List<WaypointInfo>();
		}
	}
}
