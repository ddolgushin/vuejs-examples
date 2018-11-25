using System;
using System.Collections.Generic;

namespace RunTogether.ApplicationServices.Mapping.Dto
{
	public class MarathonDto
	{
		public int Id { get; set; }
		public string Title { get; set; }
		public DateTime Date { get; set; }
		public string Description { get; set; }
		public float? Distance { get; set; }
		public string Route { get; set; }
		public List<WaypointInfoDto> WaypointInfos { get; set; }

		public MarathonDto()
		{
			WaypointInfos = new List<WaypointInfoDto>();
		}
	}
}
